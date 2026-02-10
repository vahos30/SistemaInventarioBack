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
    public class UnitTestFacturaRepository
    {
        private AppDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new AppDbContext(options);
        }

        private Factura CrearFactura(Guid? clienteId = null)
        {
            return new Factura
            {
                Id = Guid.NewGuid(),
                ClienteId = clienteId ?? Guid.NewGuid(),
                Fecha = DateTime.UtcNow,
                FormaPago = "Efectivo",
                MetodoPago = "Contado",
                NumeroFactura = "F001",
                Observacion = "Obs",
                Referencia = "Ref",
                Total = 1000,
                Detalles = new List<DetalleFactura>()
            };
        }

        [TestMethod]
        public async Task AgregarAsync_AgregaFactura()
        {
            var context = GetDbContext(nameof(AgregarAsync_AgregaFactura));
            var repo = new FacturaRepository(context);
            var factura = CrearFactura();

            await repo.AgregarAsync(factura);

            Assert.AreEqual(1, context.Facturas.Count());
        }

        [TestMethod]
        public async Task ObtenerPorIdAsync_DevuelveFactura()
        {
            var context = GetDbContext(nameof(ObtenerPorIdAsync_DevuelveFactura));
            var cliente = new Cliente { Id = Guid.NewGuid(), Nombre = "Test", Apellido = "Test", Ciudad = "Test", CiudadId = 1, CodigoCiudad = "01", Departamento = "Test", Direccion = "Test", Email = "test@test.com", IdTipoDocumentoIdentidad = 1, IdTipoOrganizacion = 1, IdTributo = 1, NumeroDocumento = "123", RazonSocial = "Test", Telefono = "123", TipoDocumento = "CC" };
            context.Clientes.Add(cliente);
            var factura = CrearFactura(cliente.Id);
            context.Facturas.Add(factura);
            context.SaveChanges();
            var repo = new FacturaRepository(context);

            var result = await repo.ObtenerPorIdAsync(factura.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(factura.Id, result.Id);
        }

        [TestMethod]
        public async Task ObtenerFacturasAsync_DevuelveTodas()
        {
            var context = GetDbContext(nameof(ObtenerFacturasAsync_DevuelveTodas));
            var cliente1 = new Cliente { Id = Guid.NewGuid(), Nombre = "Test1", Apellido = "Test1", Ciudad = "Test", CiudadId = 1, CodigoCiudad = "01", Departamento = "Test", Direccion = "Test", Email = "test1@test.com", IdTipoDocumentoIdentidad = 1, IdTipoOrganizacion = 1, IdTributo = 1, NumeroDocumento = "1234", RazonSocial = "Test1", Telefono = "123", TipoDocumento = "CC" };
            var cliente2 = new Cliente { Id = Guid.NewGuid(), Nombre = "Test2", Apellido = "Test2", Ciudad = "Test", CiudadId = 2, CodigoCiudad = "02", Departamento = "Test", Direccion = "Test", Email = "test2@test.com", IdTipoDocumentoIdentidad = 1, IdTipoOrganizacion = 1, IdTributo = 1, NumeroDocumento = "5678", RazonSocial = "Test2", Telefono = "456", TipoDocumento = "CC" };
            context.Clientes.AddRange(cliente1, cliente2);
            context.Facturas.Add(CrearFactura(cliente1.Id));
            context.Facturas.Add(CrearFactura(cliente2.Id));
            context.SaveChanges();
            var repo = new FacturaRepository(context);

            var result = await repo.ObtenerFacturasAsync();

            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task ObtenerFacturasPorClienteAsync_DevuelveSoloDelCliente()
        {
            var context = GetDbContext(nameof(ObtenerFacturasPorClienteAsync_DevuelveSoloDelCliente));
            var clienteId = Guid.NewGuid();
            context.Facturas.Add(CrearFactura(clienteId));
            context.Facturas.Add(CrearFactura());
            context.SaveChanges();
            var repo = new FacturaRepository(context);

            var result = await repo.ObtenerFacturasPorClienteAsync(clienteId);

            Assert.AreEqual(1, result.Count());
            Assert.IsTrue(result.All(f => f.ClienteId == clienteId));
        }

        [TestMethod]
        public async Task ObtenerFacturasDiariasAsync_DevuelveFacturasDeHoy()
        {
            var context = GetDbContext(nameof(ObtenerFacturasDiariasAsync_DevuelveFacturasDeHoy));
            var hoy = DateTime.UtcNow.Date;
            context.Facturas.Add(new Factura { Id = Guid.NewGuid(), ClienteId = Guid.NewGuid(), Fecha = hoy, FormaPago = "Efectivo", MetodoPago = "Contado", NumeroFactura = "F001", Observacion = "Obs", Referencia = "Ref", Total = 1000, Detalles = new List<DetalleFactura>() });
            context.Facturas.Add(new Factura { Id = Guid.NewGuid(), ClienteId = Guid.NewGuid(), Fecha = hoy.AddDays(-1), FormaPago = "Efectivo", MetodoPago = "Contado", NumeroFactura = "F002", Observacion = "Obs", Referencia = "Ref", Total = 1000, Detalles = new List<DetalleFactura>() });
            context.SaveChanges();
            var repo = new FacturaRepository(context);

            var result = await repo.ObtenerFacturasDiariasAsync(hoy);

            Assert.AreEqual(1, result.Count());
        }

        [TestMethod]
        public async Task ObtenerFacturasPorFechaAsync_DevuelveFacturasEnRango()
        {
            var context = GetDbContext(nameof(ObtenerFacturasPorFechaAsync_DevuelveFacturasEnRango));
            var fechaInicio = DateTime.UtcNow.Date.AddDays(-2);
            var fechaFin = DateTime.UtcNow.Date;
            context.Facturas.Add(new Factura { Id = Guid.NewGuid(), ClienteId = Guid.NewGuid(), Fecha = fechaInicio, FormaPago = "Efectivo", MetodoPago = "Contado", NumeroFactura = "F001", Observacion = "Obs", Referencia = "Ref", Total = 1000, Detalles = new List<DetalleFactura>() });
            context.Facturas.Add(new Factura { Id = Guid.NewGuid(), ClienteId = Guid.NewGuid(), Fecha = fechaFin, FormaPago = "Efectivo", MetodoPago = "Contado", NumeroFactura = "F002", Observacion = "Obs", Referencia = "Ref", Total = 1000, Detalles = new List<DetalleFactura>() });
            context.SaveChanges();
            var repo = new FacturaRepository(context);

            var result = await repo.ObtenerFacturasPorFechaAsync(fechaInicio, fechaFin);

            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task EliminarAsync_EliminaFactura()
        {
            var context = GetDbContext(nameof(EliminarAsync_EliminaFactura));
            var factura = CrearFactura();
            context.Facturas.Add(factura);
            context.SaveChanges();
            var repo = new FacturaRepository(context);

            await repo.EliminarAsync(factura.Id);

            Assert.AreEqual(0, context.Facturas.Count());
        }

        [TestMethod]
        public async Task ActualizarAsync_ActualizaFactura()
        {
            var context = GetDbContext(nameof(ActualizarAsync_ActualizaFactura));
            var factura = CrearFactura();
            context.Facturas.Add(factura);
            context.SaveChanges();
            var repo = new FacturaRepository(context);

            factura.Observacion = "Actualizado";
            await repo.ActualizarAsync(factura);

            var actualizado = context.Facturas.First();
            Assert.AreEqual("Actualizado", actualizado.Observacion);
        }
    }
}
