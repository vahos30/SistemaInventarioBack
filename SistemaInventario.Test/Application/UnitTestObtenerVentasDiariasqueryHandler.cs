using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SistemaInventario.Application.Feactures.Reportes;
using SistemaInventario.Application.DTOs;
using SistemaInventario.Domain.Entities;
using SistemaInventario.Domain.Interfaces;

namespace SistemaInventario.Test.Application
{
    [TestClass]
    public class UnitTestObtenerVentasDiariasQueryHandler
    {
        private Mock<IReciboRepository> _reciboRepositoryMock;
        private Mock<IFacturaRepository> _facturaRepositoryMock;
        private IMapper _mapper;
        private ObtenerVentasDiariasHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _reciboRepositoryMock = new Mock<IReciboRepository>();
            _facturaRepositoryMock = new Mock<IFacturaRepository>();

            // Configuración completa de AutoMapper
            var config = new MapperConfiguration(cfg =>
            {
                // Mapeo principal del Recibo
                cfg.CreateMap<Recibo, ReciboDto>()
                    .ForMember(dest => dest.Total, opt => opt.MapFrom(src =>
                        src.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario)));

                // Mapeo del detalle
                cfg.CreateMap<DetalleRecibo, DetalleReciboDto>()
                    .ForMember(dest => dest.ProductoId, opt => opt.MapFrom(src =>
                        src.Producto.Id));

                // Mapeo para Factura y FacturaDto
                cfg.CreateMap<Factura, FacturaDto>()
                    .ForMember(dest => dest.Total, opt => opt.MapFrom(src =>
                        src.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario)));
                cfg.CreateMap<DetalleFactura, DetalleFacturaDto>()
                    .ForMember(dest => dest.ProductoId, opt => opt.MapFrom(src =>
                        src.Producto.Id));
            });

            _mapper = config.CreateMapper();
            _handler = new ObtenerVentasDiariasHandler(_reciboRepositoryMock.Object, _facturaRepositoryMock.Object, _mapper);
        }

        [TestMethod]
        public async Task Handle_DeberiaRetornarVentasDiariasMapeadas()
        {
            // Arrange
            var productoEjemplo = new Producto
            {
                Id = Guid.NewGuid(),
                Nombre = "Laptop"
            };

            var reciboPrueba = new Recibo
            {
                Id = Guid.NewGuid(),
                Fecha = DateTime.UtcNow,
                Detalles = new List<DetalleRecibo>
                {
                    new DetalleRecibo
                    {
                        Id = Guid.NewGuid(),
                        Cantidad = 2,
                        PrecioUnitario = 500m,
                        Producto = productoEjemplo
                    },
                    new DetalleRecibo
                    {
                        Id = Guid.NewGuid(),
                        Cantidad = 1,
                        PrecioUnitario = 300m,
                        Producto = productoEjemplo
                    }
                }
            };

            _reciboRepositoryMock.Setup(x => x.ObtenerVentasPorFechaAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new List<Recibo> { reciboPrueba });

            _facturaRepositoryMock.Setup(x => x.ObtenerFacturasPorFechaAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new List<Factura>());

            var query = new ObtenerVentasDiariasQuery();

            // Act
            var resultado = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsTrue(resultado.Recibos.Any(), "No se retornaron recibos mapeados.");
            var reciboMapeado = resultado.Recibos.First();

            Assert.AreEqual(1300m, reciboMapeado.Total);
            Assert.AreEqual(reciboPrueba.Id, reciboMapeado.Id);
            Assert.AreEqual(reciboPrueba.Fecha, reciboMapeado.Fecha);
            Assert.AreEqual(2, reciboMapeado.Detalles.Count);
            var primerDetalle = reciboMapeado.Detalles[0];
            Assert.AreEqual(productoEjemplo.Id, primerDetalle.ProductoId);
            Assert.AreEqual(500m, primerDetalle.PrecioUnitario);
            Assert.AreEqual(2, primerDetalle.Cantidad);
        }

        [TestMethod]
        public async Task Handle_ReciboSinDetalles_DeberiaRetornarTotalCero()
        {
            // Arrange
            var reciboVacio = new Recibo
            {
                Id = Guid.NewGuid(),
                Fecha = DateTime.UtcNow,
                Detalles = new List<DetalleRecibo>()
            };

            _reciboRepositoryMock.Setup(x => x.ObtenerVentasPorFechaAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new List<Recibo> { reciboVacio });

            _facturaRepositoryMock.Setup(x => x.ObtenerFacturasPorFechaAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new List<Factura>());

            // Act
            var resultado = await _handler.Handle(new ObtenerVentasDiariasQuery(), CancellationToken.None);

            // Assert
            Assert.IsTrue(resultado.Recibos.Any(), "No se retornaron recibos mapeados.");
            Assert.AreEqual(0m, resultado.Recibos.First().Total);
        }

        [TestMethod]
        public async Task Handle_DeberiaRetornarFacturasDiariasMapeadas()
        {
            // Arrange
            var productoEjemplo = new Producto
            {
                Id = Guid.NewGuid(),
                Nombre = "Monitor"
            };

            var facturaPrueba = new Factura
            {
                Id = Guid.NewGuid(),
                Fecha = DateTime.UtcNow,
                Detalles = new List<DetalleFactura>
                {
                    new DetalleFactura
                    {
                        Id = Guid.NewGuid(),
                        Cantidad = 1,
                        PrecioUnitario = 800m,
                        Producto = productoEjemplo
                    }
                }
            };

            _reciboRepositoryMock.Setup(x => x.ObtenerVentasPorFechaAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new List<Recibo>());

            _facturaRepositoryMock.Setup(x => x.ObtenerFacturasPorFechaAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new List<Factura> { facturaPrueba });

            var query = new ObtenerVentasDiariasQuery();

            // Act
            var resultado = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsTrue(resultado.Facturas.Any(), "No se retornaron facturas mapeadas.");
            var facturaMapeada = resultado.Facturas.First();
            Assert.AreEqual(facturaPrueba.Id, facturaMapeada.Id);
            Assert.AreEqual(facturaPrueba.Fecha, facturaMapeada.Fecha);
            Assert.AreEqual(800m, facturaMapeada.Total);
            Assert.AreEqual(1, facturaMapeada.Detalles.Count);
            var primerDetalle = facturaMapeada.Detalles[0];
            Assert.AreEqual(productoEjemplo.Id, primerDetalle.ProductoId);
            Assert.AreEqual(800m, primerDetalle.PrecioUnitario);
            Assert.AreEqual(1, primerDetalle.Cantidad);
        }
    }
}




