using MediatR;
using SistemaInventario.Domain.Entities;
using SistemaInventario.Application.Services;
using SistemaInventario.Domain.Interfaces;
using SistemaInventario.Application.DTOs;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using SistemaInventario.Application.Mappers;
using System.Text.Json;

namespace SistemaInventario.Application.Feactures.Facturas
{

    public class CrearFacturaFactusCommandHandler : IRequestHandler<CrearFacturaFactusCommand, string>
    {
        private readonly FactusFacturaService _factusFacturaService;
        private readonly IClienteRepository _clienteRepository;
        private readonly IProductoRepository _productoRepository;
        private readonly IFacturaRepository _facturaRepository; // Agregado

        public CrearFacturaFactusCommandHandler(
            FactusFacturaService factusFacturaService,
            IClienteRepository clienteRepository,
            IProductoRepository productoRepository,
            IFacturaRepository facturaRepository) // Modificado
        {
            _factusFacturaService = factusFacturaService;
            _clienteRepository = clienteRepository;
            _productoRepository = productoRepository;
            _facturaRepository = facturaRepository; // Agregado
        }

        public async Task<string> Handle(CrearFacturaFactusCommand request, CancellationToken cancellationToken)
        {
            // 1. Consultar el cliente
            var cliente = await _clienteRepository.ObtenerPorIdsync(request.ClienteId);
            if (cliente == null)
                throw new Exception("Cliente no encontrado.");

            // 2. Consultar los productos y armar los detalles completos
            var detallesCompletos = new List<DetalleFactura>();
            foreach (var detalleDto in request.Detalles)
            {
                var producto = await _productoRepository.ObtenerPorIdsync(detalleDto.ProductoId);
                if (producto == null)
                    throw new Exception($"Producto con ID {detalleDto.ProductoId} no encontrado.");

                var detalleCompleto = new DetalleFactura
                {
                    ProductoId = detalleDto.ProductoId,
                    Producto = producto,
                    Cantidad = detalleDto.Cantidad,
                    PrecioUnitario = detalleDto.PrecioUnitario,
                    TipoDescuento = detalleDto.TipoDescuento,
                    ValorDescuento = detalleDto.ValorDescuento,
                    
                };
                detallesCompletos.Add(detalleCompleto);
            }

            // 3. Generar referencia única si no se proporciona
            var referencia = string.IsNullOrWhiteSpace(request.Referencia)
                ? $"FAC-{DateTime.UtcNow:yyyyMMddHHmmssfff}-{Guid.NewGuid().ToString().Substring(0, 8)}"
                : request.Referencia;

            // 4. Fecha de vencimiento solo si es pago a crédito
            string? fechaVencimiento = null;
            if (request.FormaPago == "2")
                fechaVencimiento = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-dd");

            // 5. Establecimiento fijo
            var establecimientoFijo = new FactusEstablishment
            {
                name = "TECNOFRIO DISTRIBUCIONES S.A.S.",
                address = "CARRERA 99 65 265",
                phone_number = "(57) 3113740874",
                email = "administracion@tecnofriodistribuciones.com.co",
                municipality_id = "80" // Medellin
            };

            // 6. Mapear y enviar a Factus
            var factusRequest = FactusMapper.MapFacturaToFactusRequest(
                cliente,
                detallesCompletos,
                referencia,
                request.Observacion,
                fechaVencimiento, // Solo se envía si es crédito
                request.FormaPago,
                request.MetodoPago,
                establecimientoFijo,
                true // <-- Aquí decides si enviar el email o no
            );

            // Imprimir el JSON generado para depuración
            var json = JsonSerializer.Serialize(factusRequest, new JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine("JSON enviado a Factus:");
            Console.WriteLine(json);

            var result = await _factusFacturaService.CrearFacturaAsync(factusRequest);

            // 1. Procesa la respuesta de Factus (deserializa el JSON si es necesario)
            var factusResponse = JsonSerializer.Deserialize<FactusResponse>(result);

            // Asumiendo que factusResponse.data.items es una lista de items de la factura
            var factura = new Factura
            {
                NumeroFactura = factusResponse.data.bill.number,
                ClienteId = cliente.Id,
                Fecha = DateTime.UtcNow,
                FormaPago = request.FormaPago,
                MetodoPago = request.MetodoPago, // <-- NUEVO
                Observacion = request.Observacion,
                Referencia = factusResponse.data.bill.reference_code,
                Total = decimal.Parse(factusResponse.data.bill.total),
                FactusBillId = factusResponse.data.bill.id,
                Detalles = detallesCompletos.Select((d, i) => new DetalleFactura
                {
                    ProductoId = d.ProductoId,
                    Cantidad = decimal.TryParse(factusResponse.data.items[i].quantity, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var cantidadDecimal)
    ? (int)cantidadDecimal
    : 0,
                    PrecioUnitario = d.PrecioUnitario,
                    TipoDescuento = d.TipoDescuento,
                    ValorDescuento = d.ValorDescuento,
                    ValorIva = !string.IsNullOrEmpty(factusResponse.data.items[i].tax_amount)
                        ? decimal.Parse(factusResponse.data.items[i].tax_amount, System.Globalization.CultureInfo.InvariantCulture)
                        : 0,
                    Subtotal = d.PrecioUnitario * (decimal.TryParse(factusResponse.data.items[i].quantity, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var c) ? (int)c : 0)
                }).ToList()
            };

            await _facturaRepository.AgregarAsync(factura);

            // Descontar inventario de productos
            foreach (var detalle in factura.Detalles)
            {
                var producto = await _productoRepository.ObtenerPorIdsync(detalle.ProductoId);
                if (producto == null)
                    throw new Exception($"Producto con ID {detalle.ProductoId} no encontrado.");

                if (producto.CantidadStock < detalle.Cantidad)
                    throw new Exception($"Stock insuficiente para el producto {producto.Nombre}.");

                producto.CantidadStock -= detalle.Cantidad;
                await _productoRepository.ActualizarAsync(producto);
            }

            // 3. Retorna la respuesta original de Factus
            return result;
        }
    }
}