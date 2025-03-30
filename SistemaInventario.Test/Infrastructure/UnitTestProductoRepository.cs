using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SistemaInventario.Domain.Entities;
using SistemaInventario.Infrastructure.Persistence;
using SistemaInventario.Infrastructure.Repositories;

namespace SistemaInventario.Test.Infrastructure
{
    [TestClass]
    public class UnitTestProductoRepository
    {
        private DbContextOptions<AppDbContext> _options;
        private AppDbContext _context;
        private ProductoRepository _productoRepository;

        [TestInitialize]
        public void Setup()
        {
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new AppDbContext(_options);
            _productoRepository = new ProductoRepository(_context);
        }

        [TestMethod]
        public async Task ObtenerProductosActivosAsync_DeberiaFiltrarSoloActivos() 
        {
            // Arrange
            await _context.Productos.AddRangeAsync(
                new Producto { Activo = true },
                new Producto { Activo = false }
                );
            await _context.SaveChangesAsync();

            // Act
            var productosActivos = await _productoRepository.ObtenerProductosActivosAsync();

            // Assert
            Assert.AreEqual(1, productosActivos.Count());
            Assert.IsTrue(productosActivos.All(p => p.Activo));

        }

        [TestMethod]
        public async Task AgregarAsync_DeberiaGuardarProducto()
        {
            // Arrange
            var producto = new Producto { Id = new Guid(),Nombre = "Laptop", Precio= 2000000,
            Descripcion = "asus 15p", Activo= true, CantidadStock = 2};

            // Act
            await _productoRepository.AgregarAsync(producto);
            var productoGuardado = await _context.Productos.FirstOrDefaultAsync();

            // Assert
            Assert.IsNotNull(productoGuardado);
            Assert.AreEqual("Laptop", productoGuardado.Nombre);
            Assert.AreEqual(2000000, productoGuardado.Precio);
            Assert.AreEqual("asus 15p", productoGuardado.Descripcion);
            Assert.AreEqual(true, productoGuardado.Activo);
            Assert.AreEqual(2, productoGuardado.CantidadStock);

        }

        [TestMethod]
        public async Task EliminarAsync_DeberiaEliminarProductoExistente()
        {
            // Arrange
            var producto = new Producto { Id = Guid.NewGuid() };
            await _context.Productos.AddAsync(producto);
            await _context.SaveChangesAsync();

            // Act
            await _productoRepository.EliminarAsync(producto.Id);
            var productoEliminado = await _context.Productos.FindAsync(producto.Id);

            // Assert
            Assert.IsNull(productoEliminado);
        }

        [TestMethod]
        public async Task ActualizarAsync_DeberiaModificarProducto()
        {
            // Arrange
            var producto = new Producto { Nombre = "Teclado" };
            await _context.Productos.AddAsync(producto);
            await _context.SaveChangesAsync();

            // Act
            producto.Nombre = "Teclado Mecánico";
            await _productoRepository.ActualizarAsync(producto);
            var productoActualizado = await _context.Productos.FindAsync(producto.Id);

            // Assert
            Assert.AreEqual("Teclado Mecánico", productoActualizado.Nombre);
        }

        [TestMethod]
        public async Task ObtenerPorIdsync_DeberiaRetornarProductoCorrecto()
        {
            // Arrange
            var productoId = Guid.NewGuid();
            await _context.Productos.AddAsync(new Producto { Id = productoId });
            await _context.SaveChangesAsync();

            // Act
            var producto = await _productoRepository.ObtenerPorIdsync(productoId);

            // Assert
            Assert.IsNotNull(producto);
            Assert.AreEqual(productoId, producto.Id);
        }

    }
}
