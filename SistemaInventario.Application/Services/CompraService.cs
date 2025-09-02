using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaInventario.Domain.Entities;
using SistemaInventario.Domain.Interfaces;
using SistemaInventario.Application.DTOs;

namespace SistemaInventario.Application.Services
{
    public class CompraService
    {
        private readonly ICompraRepository _compraRepository;
        private readonly IProductoRepository _productoRepository;

        public CompraService(ICompraRepository compraRepository, IProductoRepository productoRepository)
        {
            _compraRepository = compraRepository;
            _productoRepository = productoRepository;
        }

        // Obtener todas las compras
        public async Task<IEnumerable<Compra>> ObtenerTodasAsync()
        {
            return await _compraRepository.ObtenerTodasAsync();
        }

        // Obtener una compra por ID
        public async Task<Compra?> ObtenerPorIdAsync(Guid id)
        {
            return await _compraRepository.ObtenerPorIdAsync(id);
        }

        // Crear una compra (registrar)
        public async Task CrearAsync(Compra compra)
        {
            // Validar detalles
            if (compra.Detalles == null || !compra.Detalles.Any())
                throw new ArgumentException("La compra debe tener al menos un detalle.");

            decimal total = 0;
            foreach (var detalle in compra.Detalles)
            {
                var producto = await _productoRepository.ObtenerPorIdsync(detalle.ProductoId);
                if (producto == null)
                    throw new Exception($"Producto con ID {detalle.ProductoId} no encontrado.");

                producto.CantidadStock += detalle.Cantidad;
                producto.Activo = producto.CantidadStock > 0; // <-- Actualiza el estado Activo
                await _productoRepository.ActualizarAsync(producto);

                detalle.SubTotal = detalle.Cantidad * detalle.PrecioUnitario;
                total += detalle.SubTotal;
            }

            compra.Total = total;

            // Convertir la fecha a la zona horaria de Colombia antes de guardar
            var colombiaZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
            var fechaColombia = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, colombiaZone);
            compra.Fecha = fechaColombia;

            await _compraRepository.AgregarAsync(compra);
        }

        // Actualizar una compra
        public async Task ActualizarAsync(Compra compra)
        {
            // Puedes agregar lógica para validar o recalcular si es necesario
            await _compraRepository.ActualizarAsync(compra);
        }

        // Eliminar una compra por ID
        public async Task EliminarAsync(Guid id)
        {
            var compra = await _compraRepository.ObtenerPorIdAsync(id);
            if (compra != null)
            {
                foreach (var detalle in compra.Detalles)
                {
                    var producto = await _productoRepository.ObtenerPorIdsync(detalle.ProductoId);
                    producto.CantidadStock -= detalle.Cantidad;
                    producto.Activo = producto.CantidadStock > 0; // <-- Actualiza el estado Activo
                    await _productoRepository.ActualizarAsync(producto);
                }
            }
            await _compraRepository.EliminarAsync(id);
        }

        // Anular una compra por ID
        public async Task AnularAsync(Guid id)
        {
            var compra = await _compraRepository.ObtenerPorIdAsync(id);
            if (compra == null)
                throw new Exception("Compra no encontrada.");

            // Actualiza el estado de la compra a 'Anulada'
            compra.Estado = "Anulada";

            await _compraRepository.ActualizarAsync(compra);
        }

        // Anular parcialmente una compra
        public async Task AnularParcialAsync(CompraAnulacionParcialDto dto)
        {
            var compra = await _compraRepository.ObtenerPorIdAsync(dto.CompraId);
            if (compra == null)
                throw new Exception("Compra no encontrada.");

            foreach (var detalleAnulacion in dto.Detalles)
            {
                var detalleCompra = compra.Detalles.FirstOrDefault(d => d.ProductoId == detalleAnulacion.ProductoId);
                if (detalleCompra == null)
                    throw new Exception($"Producto {detalleAnulacion.ProductoId} no encontrado en la compra.");

                if (detalleAnulacion.CantidadAAnular > detalleCompra.Cantidad)
                    throw new Exception("No se puede anular más de lo comprado.");

                var producto = await _productoRepository.ObtenerPorIdsync(detalleAnulacion.ProductoId);
                producto.CantidadStock -= detalleAnulacion.CantidadAAnular;
                producto.Activo = producto.CantidadStock > 0; // <-- Actualiza el estado Activo
                await _productoRepository.ActualizarAsync(producto);

                detalleCompra.MotivoDevolucion = detalleAnulacion.MotivoDevolucion;
                detalleCompra.Cantidad -= detalleAnulacion.CantidadAAnular;
                detalleCompra.SubTotal = detalleCompra.Cantidad * detalleCompra.PrecioUnitario;
            }

            compra.Detalles = compra.Detalles.Where(d => d.Cantidad > 0).ToList();
            compra.Total = compra.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario);
            compra.Estado = "Anulada Parcial";

            await _compraRepository.ActualizarAsync(compra);
        }
    }
}
