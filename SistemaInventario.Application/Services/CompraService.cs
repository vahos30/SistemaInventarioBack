using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaInventario.Domain.Entities;
using SistemaInventario.Domain.Interfaces;

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

            // Actualizar stock de productos y calcular el total
            decimal total = 0;
            foreach (var detalle in compra.Detalles)
            {
                var producto = await _productoRepository.ObtenerPorIdsync(detalle.ProductoId);
                if (producto == null)
                    throw new Exception($"Producto con ID {detalle.ProductoId} no encontrado.");

                producto.CantidadStock += detalle.Cantidad;
                await _productoRepository.ActualizarAsync(producto);

                total += detalle.Cantidad * detalle.PrecioUnitario;
            }

            compra.Total = total;
            compra.Fecha = DateTime.UtcNow;

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
            await _compraRepository.EliminarAsync(id);
        }
    }
}
