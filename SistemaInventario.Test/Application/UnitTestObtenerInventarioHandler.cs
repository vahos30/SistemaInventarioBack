using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SistemaInventario.Application.DTOs;
using SistemaInventario.Application.Feactures.Reportes;
using SistemaInventario.Domain.Entities;
using SistemaInventario.Domain.Interfaces;

namespace SistemaInventario.Test.Application
{
    [TestClass]
    public class UnitTestObtenerInventarioHandler
    {
        private Mock<IProductoRepository> _productoRepositoryMock;
        private IMapper _mapper;
        private ObtenerInventarioHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _productoRepositoryMock = new Mock<IProductoRepository>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Producto, ProductoDto>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                    .ForMember(dest => dest.CantidadStock, opt => opt.MapFrom(src => src.CantidadStock))
                    .ForMember(dest => dest.Precio, opt => opt.MapFrom(src => src.Precio))
                    .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => src.Activo));
            });

            _mapper = config.CreateMapper();
            _handler = new ObtenerInventarioHandler(_productoRepositoryMock.Object, _mapper);
        }

        [TestMethod]
        public async Task Handle_DeberiaRetornarInventarioCompleto()
        {
            // Arrange
            var productosPrueba = new List<Producto>
            {
                new Producto
                {
                    Id = Guid.NewGuid(),
                    Nombre = "Laptop HP",
                    CantidadStock = 15,
                    Precio = 1500.99m,
                    Activo = true
                },
                new Producto
                {
                    Id = Guid.NewGuid(),
                    Nombre = "Teclado Mecánico",
                    CantidadStock = 25,
                    Precio = 89.99m,
                    Activo = true
                }
            };

            _productoRepositoryMock.Setup(x => x.ObtenerProductosActivosAsync())
                                  .ReturnsAsync(productosPrueba);

            // Act
            var resultado = await _handler.Handle(new ObtenerInventarioQuery(), CancellationToken.None);
            var listaResultados = resultado.ToList();

            // Assert
            _productoRepositoryMock.Verify(x => x.ObtenerProductosActivosAsync(), Times.Once);
            Assert.AreEqual(2, listaResultados.Count);
            Assert.IsTrue(listaResultados.All(p => p.Activo));
        }

        [TestMethod]
        public async Task Handle_InventarioVacio_DeberiaRetornarListaVacia()
        {
            // Arrange
            _productoRepositoryMock.Setup(x => x.ObtenerProductosActivosAsync())
                                  .ReturnsAsync(new List<Producto>());

            // Act
            var resultado = await _handler.Handle(new ObtenerInventarioQuery(), CancellationToken.None);

            // Assert
            Assert.IsFalse(resultado.Any());
        }

        [TestMethod]
        public async Task Handle_ProductosInactivos_NoDeberiaIncluirlos()
        {
            // Arrange
            var productosPrueba = new List<Producto>
            {
                new Producto { Activo = true, CantidadStock = 10 },
                new Producto { Activo = false, CantidadStock = 5 }
            };

            // Configurar el mock para devolver solo productos activos
            _productoRepositoryMock.Setup(x => x.ObtenerProductosActivosAsync())
                                  .ReturnsAsync(productosPrueba.Where(p => p.Activo).ToList());

            // Act
            var resultado = await _handler.Handle(new ObtenerInventarioQuery(), CancellationToken.None);

            // Assert
            Assert.AreEqual(1, resultado.Count());
            Assert.IsTrue(resultado.All(p => p.Activo));
        }
    }
}
