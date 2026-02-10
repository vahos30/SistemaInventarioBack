using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SistemaInventario.Domain.Entities;
using SistemaInventario.Infrastructure.Persistence;
using SistemaInventario.Infrastructure.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaInventario.Test.Infrastructure.Repositories
{
    [TestClass]
    public class UnitTestProveedorRepository
    {
        private AppDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new AppDbContext(options);
        }

        private Proveedor CrearProveedor(string? nit = null)
        {
            return new Proveedor
            {
                Id = Guid.NewGuid(),
                Nombre = "Proveedor Test",
                RazonSocial = "RS Test",
                NIT = nit ?? Guid.NewGuid().ToString(),
                Telefono = "123456",
                Email = "proveedor@test.com",
                Activo = true
            };
        }

        [TestMethod]
        public async Task AgregarAsync_AgregaProveedor()
        {
            var context = GetDbContext(nameof(AgregarAsync_AgregaProveedor));
            var repo = new ProveedorRepository(context);
            var proveedor = CrearProveedor();

            await repo.AgregarAsync(proveedor);

            Assert.AreEqual(1, context.Proveedores.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task AgregarAsync_ProveedorConNitDuplicado_LanzaExcepcion()
        {
            var context = GetDbContext(nameof(AgregarAsync_ProveedorConNitDuplicado_LanzaExcepcion));
            var repo = new ProveedorRepository(context);
            var nit = "NIT-123";
            var proveedor1 = CrearProveedor(nit);
            var proveedor2 = CrearProveedor(nit);

            await repo.AgregarAsync(proveedor1);
            await repo.AgregarAsync(proveedor2); // Debe lanzar excepción
        }

        [TestMethod]
        public async Task ActualizarAsync_ActualizaProveedor()
        {
            var context = GetDbContext(nameof(ActualizarAsync_ActualizaProveedor));
            var repo = new ProveedorRepository(context);
            var proveedor = CrearProveedor();
            context.Proveedores.Add(proveedor);
            context.SaveChanges();

            proveedor.Nombre = "Actualizado";
            await repo.ActualizarAsync(proveedor);

            var actualizado = context.Proveedores.First();
            Assert.AreEqual("Actualizado", actualizado.Nombre);
        }

        [TestMethod]
        public async Task EliminarAsync_EliminaProveedor()
        {
            var context = GetDbContext(nameof(EliminarAsync_EliminaProveedor));
            var repo = new ProveedorRepository(context);
            var proveedor = CrearProveedor();
            context.Proveedores.Add(proveedor);
            context.SaveChanges();

            await repo.EliminarAsync(proveedor.Id);

            Assert.AreEqual(0, context.Proveedores.Count());
        }

        [TestMethod]
        public async Task ObtenerPorIdAsync_DevuelveProveedor()
        {
            var context = GetDbContext(nameof(ObtenerPorIdAsync_DevuelveProveedor));
            var repo = new ProveedorRepository(context);
            var proveedor = CrearProveedor();
            context.Proveedores.Add(proveedor);
            context.SaveChanges();

            var result = await repo.ObtenerPorIdAsync(proveedor.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(proveedor.Id, result.Id);
        }

        [TestMethod]
        public async Task ObtenerTodosAsync_DevuelveTodos()
        {
            var context = GetDbContext(nameof(ObtenerTodosAsync_DevuelveTodos));
            var repo = new ProveedorRepository(context);
            context.Proveedores.Add(CrearProveedor());
            context.Proveedores.Add(CrearProveedor());
            context.SaveChanges();

            var result = await repo.ObtenerTodosAsync();

            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task ObtenerPorNitAsync_DevuelveProveedor()
        {
            var context = GetDbContext(nameof(ObtenerPorNitAsync_DevuelveProveedor));
            var repo = new ProveedorRepository(context);
            var nit = "NIT-TEST";
            var proveedor = CrearProveedor(nit);
            context.Proveedores.Add(proveedor);
            context.SaveChanges();

            var result = await repo.ObtenerPorNitAsync(nit);

            Assert.IsNotNull(result);
            Assert.AreEqual(nit, result.NIT);
        }

        [TestMethod]
        public async Task EliminarPorNitAsync_EliminaProveedor()
        {
            var context = GetDbContext(nameof(EliminarPorNitAsync_EliminaProveedor));
            var repo = new ProveedorRepository(context);
            var nit = "NIT-ELIMINAR";
            var proveedor = CrearProveedor(nit);
            context.Proveedores.Add(proveedor);
            context.SaveChanges();

            await repo.EliminarPorNitAsync(nit);

            Assert.AreEqual(0, context.Proveedores.Count());
        }

        [TestMethod]
        public async Task ActualizarPorNitAsync_ActualizaProveedor()
        {
            var context = GetDbContext(nameof(ActualizarPorNitAsync_ActualizaProveedor));
            var repo = new ProveedorRepository(context);
            var nit = "NIT-ACTUALIZAR";
            var proveedor = CrearProveedor(nit);
            context.Proveedores.Add(proveedor);
            context.SaveChanges();

            var proveedorActualizado = CrearProveedor(nit);
            proveedorActualizado.Nombre = "Nuevo Nombre";
            proveedorActualizado.RazonSocial = "Nueva RS";
            proveedorActualizado.Telefono = "999999";
            proveedorActualizado.Email = "nuevo@email.com";
            proveedorActualizado.Activo = false;

            await repo.ActualizarPorNitAsync(nit, proveedorActualizado);

            var actualizado = context.Proveedores.First();
            Assert.AreEqual("Nuevo Nombre", actualizado.Nombre);
            Assert.AreEqual("Nueva RS", actualizado.RazonSocial);
            Assert.AreEqual("999999", actualizado.Telefono);
            Assert.AreEqual("nuevo@email.com", actualizado.Email);
            Assert.IsFalse(actualizado.Activo);
        }
    }
}