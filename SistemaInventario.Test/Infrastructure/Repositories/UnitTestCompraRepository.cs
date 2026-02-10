using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SistemaInventario.Domain.Entities;
using SistemaInventario.Infrastructure.Persistence;
using SistemaInventario.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaInventario.Test.Infrastructure.Repositories
{
    [TestClass]
    public class UnitTestCompraRepository
    {
        private AppDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new AppDbContext(options);
        }

        private Proveedor CrearProveedor()
        {
            return new Proveedor
            {
                Id = Guid.NewGuid(),
                Nombre = "Proveedor Test",
                RazonSocial = "RS Test",
                NIT = Guid.NewGuid().ToString(),
                Telefono = "123456",
                Email = "proveedor@test.com",
                Activo = true
            };
        }

        private Producto CrearProducto()
        {
            return new Producto
            {
                Id = Guid.NewGuid(),
                Nombre = "Producto Test",
                Precio = 100,
                Referencia = "REF-001",
                Descripcion = "Desc",
                CantidadStock = 10,
                Activo = true
            };
        }

        private Compra CrearCompra(Guid? proveedorId = null, List<DetalleCompra>? detalles = null)
        {
            return new Compra
            {
                Id = Guid.NewGuid(),
                ProveedorId = proveedorId ?? Guid.NewGuid(),
                Fecha = DateTime.UtcNow,
                Total = 500,
                Estado = "Activa",
                Detalles = detalles ?? new List<DetalleCompra>()
            };
        }

        [TestMethod]
        public async Task AgregarAsync_AgregaCompra()
        {
            var context = GetDbContext(nameof(AgregarAsync_AgregaCompra));
            var proveedor = CrearProveedor();
            context.Proveedores.Add(proveedor);
            context.SaveChanges();

            var compra = CrearCompra(proveedor.Id);
            var repo = new CompraRepository(context);

            await repo.AgregarAsync(compra);

            Assert.AreEqual(1, context.Compras.Count());
        }

        [TestMethod]
        public async Task ObtenerPorIdAsync_DevuelveCompra()
        {
            var context = GetDbContext(nameof(ObtenerPorIdAsync_DevuelveCompra));
            var proveedor = CrearProveedor();
            context.Proveedores.Add(proveedor);
            context.SaveChanges();

            var compra = CrearCompra(proveedor.Id);
            context.Compras.Add(compra);
            context.SaveChanges();

            var repo = new CompraRepository(context);

            var result = await repo.ObtenerPorIdAsync(compra.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(compra.Id, result.Id);
        }

        [TestMethod]
        public async Task ObtenerTodasAsync_DevuelveTodas()
        {
            var context = GetDbContext(nameof(ObtenerTodasAsync_DevuelveTodas));
            var proveedor1 = CrearProveedor();
            var proveedor2 = CrearProveedor();
            context.Proveedores.AddRange(proveedor1, proveedor2);
            context.SaveChanges();

            context.Compras.Add(CrearCompra(proveedor1.Id));
            context.Compras.Add(CrearCompra(proveedor2.Id));
            context.SaveChanges();

            var repo = new CompraRepository(context);

            var result = await repo.ObtenerTodasAsync();

            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task ActualizarAsync_ActualizaCompra()
        {
            var context = GetDbContext(nameof(ActualizarAsync_ActualizaCompra));
            var proveedor = CrearProveedor();
            context.Proveedores.Add(proveedor);
            context.SaveChanges();

            var compra = CrearCompra(proveedor.Id);
            context.Compras.Add(compra);
            context.SaveChanges();

            var repo = new CompraRepository(context);

            compra.Estado = "Anulada";
            await repo.ActualizarAsync(compra);

            var actualizado = context.Compras.First();
            Assert.AreEqual("Anulada", actualizado.Estado);
        }

        [TestMethod]
        public async Task EliminarAsync_EliminaCompra()
        {
            var context = GetDbContext(nameof(EliminarAsync_EliminaCompra));
            var proveedor = CrearProveedor();
            context.Proveedores.Add(proveedor);
            context.SaveChanges();

            var compra = CrearCompra(proveedor.Id);
            context.Compras.Add(compra);
            context.SaveChanges();

            var repo = new CompraRepository(context);

            await repo.EliminarAsync(compra.Id);

            Assert.AreEqual(0, context.Compras.Count());
        }

        [TestMethod]
        public async Task ObtenerConDetallesYProveedorAsync_DevuelveComprasConDetallesYProveedor()
        {
            var context = GetDbContext(nameof(ObtenerConDetallesYProveedorAsync_DevuelveComprasConDetallesYProveedor));
            var proveedor = CrearProveedor();
            var producto = CrearProducto();
            context.Proveedores.Add(proveedor);
            context.Productos.Add(producto);
            context.SaveChanges();

            var detalle = new DetalleCompra
            {
                Id = Guid.NewGuid(),
                ProductoId = producto.Id,
                Cantidad = 2,
                PrecioUnitario = 100,
                SubTotal = 200
            };

            var compra = CrearCompra(proveedor.Id, new List<DetalleCompra> { detalle });
            context.Compras.Add(compra);
            context.SaveChanges();

            var repo = new CompraRepository(context);

            var result = await repo.ObtenerConDetallesYProveedorAsync();

            Assert.AreEqual(1, result.Count());
            Assert.IsNotNull(result.First().Proveedor);
            Assert.IsTrue(result.First().Detalles.Any());
        }
    }
}