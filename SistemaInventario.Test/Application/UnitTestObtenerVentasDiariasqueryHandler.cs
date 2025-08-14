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
                        src.Producto.Id)); // Mapear ProductoId desde la relación
                // Si tienes mapeo para Factura y FacturaDto, agrégalo aquí
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
                Fecha = DateTime.UtcNow,  // Suponemos que es la fecha de hoy
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

            // Calcula el total esperado (2*500 + 1*300 = 1300)
            decimal totalEsperado = reciboPrueba.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario);
            var resultadoEsperado = (Recibos: (IEnumerable<Recibo>)new List<Recibo> { reciboPrueba }, Total: totalEsperado);

            // Ahora pasamos un argumento al método: It.IsAny<DateTime?>() o null.
            _reciboRepositoryMock.Setup(x => x.ObtenerVentasDiariasAsync(It.IsAny<DateTime?>()))
                .ReturnsAsync(resultadoEsperado);

            // Simula que no hay facturas diarias
            _facturaRepositoryMock.Setup(x => x.ObtenerFacturasDiariasAsync(It.IsAny<DateTime?>()))
                .ReturnsAsync(new List<Factura>());

            var query = new ObtenerVentasDiariasQuery();

            // Act
            var resultado = await _handler.Handle(query, CancellationToken.None);
            var reciboMapeado = resultado.Recibos.First();

            // Assert
            Assert.AreEqual(1300m, reciboMapeado.Total); // (2*500) + (1*300)
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

            var resultadoEsperado = (Recibos: (IEnumerable<Recibo>)new List<Recibo> { reciboVacio }, Total: 0m);

            _reciboRepositoryMock.Setup(x => x.ObtenerVentasDiariasAsync(It.IsAny<DateTime?>()))
                .ReturnsAsync(resultadoEsperado);

            _facturaRepositoryMock.Setup(x => x.ObtenerFacturasDiariasAsync(It.IsAny<DateTime?>()))
                .ReturnsAsync(new List<Factura>());

            // Act
            var resultado = await _handler.Handle(new ObtenerVentasDiariasQuery(), CancellationToken.None);

            // Assert
            Assert.AreEqual(0m, resultado.Recibos.First().Total);
        }
    }
}




