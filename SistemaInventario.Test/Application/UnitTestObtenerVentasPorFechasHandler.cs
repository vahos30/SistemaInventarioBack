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
    public class UnitTestObtenerVentasPorFechasHandler
    {
        private Mock<IReciboRepository> _reciboRepositoryMock;
        private IMapper _mapper;
        private ObtenerVentasPorFechasHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _reciboRepositoryMock = new Mock<IReciboRepository>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Recibo, ReciboDto>()
                   .ForMember(dest => dest.Total, opt => opt.MapFrom(src =>
                       src.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario)));

                cfg.CreateMap<DetalleRecibo, DetalleReciboDto>()
                   .ForMember(dest => dest.ProductoId, opt => opt.MapFrom(src =>
                       src.Producto.Id));
            });

            _mapper = config.CreateMapper();
            _handler = new ObtenerVentasPorFechasHandler(_reciboRepositoryMock.Object, _mapper);
        }

        [TestMethod]
        public async Task Handle_DeberiaRetornarVentasEnRangoFechas()
        {
            // Arrange
            var fechaInicio = new DateTime(2024, 1, 1);
            var fechaFin = new DateTime(2024, 1, 31);
            var producto = new Producto { Id = Guid.NewGuid(), Nombre = "Monitor" };

            var recibosPrueba = new List<Recibo>
            {
                new Recibo
                {
                    Id = Guid.NewGuid(),
                    Fecha = new DateTime(2024, 1, 15),
                    Detalles = new List<DetalleRecibo>
                    {
                        new DetalleRecibo
                        {
                            Cantidad = 3,
                            PrecioUnitario = 200m,
                            Producto = producto
                        }
                    }
                }
            };

            _reciboRepositoryMock.Setup(x => x.ObtenerVentasPorFechaAsync(fechaInicio, fechaFin))
                                .ReturnsAsync(recibosPrueba);

            var query = new ObtenerVentasPorFechasQuery(fechaInicio, fechaFin);

            // Act
            var resultado = await _handler.Handle(query, CancellationToken.None);
            var reciboDto = resultado.First();

            // Assert
            // Verifica llamada al repositorio con parámetros correctos
            _reciboRepositoryMock.Verify(x => x.ObtenerVentasPorFechaAsync(fechaInicio, fechaFin), Times.Once);

            // Verifica mapeo básico
            Assert.AreEqual(recibosPrueba[0].Id, reciboDto.Id);
            Assert.AreEqual(600m, reciboDto.Total); // 3 * 200
            Assert.AreEqual(1, reciboDto.Detalles.Count);
        }

        [TestMethod]
        public async Task Handle_FechasInvertidas_DeberiaLanzarExcepcion()
        {
            // Arrange
            var fechaInicio = new DateTime(2024, 1, 31);
            var fechaFin = new DateTime(2024, 1, 1);

            var query = new ObtenerVentasPorFechasQuery(fechaInicio, fechaFin);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                _handler.Handle(query, CancellationToken.None));
        }

        [TestMethod]
        public async Task Handle_SinVentasEnRango_DeberiaRetornarListaVacia()
        {
            
            var fechaInicio = new DateTime(2024, 2, 1);
            var fechaFin = new DateTime(2024, 2, 28);

            _reciboRepositoryMock.Setup(x => x.ObtenerVentasPorFechaAsync(fechaInicio, fechaFin))
                                .ReturnsAsync(new List<Recibo>());

            var query = new ObtenerVentasPorFechasQuery(fechaInicio, fechaFin);

           
            var resultado = await _handler.Handle(query, CancellationToken.None);

            
            Assert.IsFalse(resultado.Any());
        }

        [TestMethod]
        public async Task Handle_DeberiaMapearCorrectamenteProductoId()
        {
            
            var producto = new Producto { Id = Guid.NewGuid() };
            var recibo = new Recibo
            {
                Detalles = new List<DetalleRecibo>
                {
                    new DetalleRecibo { Producto = producto }
                }
            };

            _reciboRepositoryMock.Setup(x => x.ObtenerVentasPorFechaAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                                .ReturnsAsync(new List<Recibo> { recibo });

            
            var resultado = await _handler.Handle(new ObtenerVentasPorFechasQuery(DateTime.Now, DateTime.Now), CancellationToken.None);

            
            Assert.AreEqual(producto.Id, resultado.First().Detalles.First().ProductoId);
        }
    }
}
