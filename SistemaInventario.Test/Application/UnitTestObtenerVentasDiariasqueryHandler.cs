using System;
using System.Collections.Generic;
using System.Linq;
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
        private IMapper _mapper;
        private ObtenerVentasDiariasHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _reciboRepositoryMock = new Mock<IReciboRepository>();

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
            });

            _mapper = config.CreateMapper();
            _handler = new ObtenerVentasDiariasHandler(_reciboRepositoryMock.Object, _mapper);
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

            _reciboRepositoryMock.Setup(x => x.ObtenerVentasDiariasAsync())
                .ReturnsAsync(new List<Recibo> { reciboPrueba });

            var query = new ObtenerVentasDiariasQuery();

            // Act
            var resultado = await _handler.Handle(query, CancellationToken.None);
            var reciboMapeado = resultado.First();

            // Assert
            // Verificar total
            Assert.AreEqual(1300m, reciboMapeado.Total); // (2*500) + (1*300)

            // Verificar mapeo básico
            Assert.AreEqual(reciboPrueba.Id, reciboMapeado.Id);
            Assert.AreEqual(reciboPrueba.Fecha, reciboMapeado.Fecha);

            // Verificar detalles
            Assert.AreEqual(2, reciboMapeado.Detalles.Count);

            // Verificar primer detalle
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

            _reciboRepositoryMock.Setup(x => x.ObtenerVentasDiariasAsync())
                .ReturnsAsync(new List<Recibo> { reciboVacio });

            // Act
            var resultado = await _handler.Handle(new ObtenerVentasDiariasQuery(), CancellationToken.None);

            // Assert
            Assert.AreEqual(0m, resultado.First().Total);
        }
    }
}
