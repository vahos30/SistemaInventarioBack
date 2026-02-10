using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SistemaInventario.Domain.Entities;
using SistemaInventario.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[TestClass]
public class UnitTestNotaCreditoRepository
{
    private AppDbContext GetDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        return new AppDbContext(options);
    }

    private NotaCredito CrearNotaCredito(List<DetalleNotaCredito>? detalles = null)
    {
        return new NotaCredito
        {
            Id = Guid.NewGuid(),
            FacturaId = Guid.NewGuid(),
            Fecha = DateTime.UtcNow,
            Motivo = "Devolución",
            NumeroNotaCredito = "NC-001",
            Total = 1000,
            Detalles = detalles ?? new List<DetalleNotaCredito>()
        };
    }

    [TestMethod]
    public async Task AgregarAsync_AgregaNotaCredito()
    {
        var context = GetDbContext(nameof(AgregarAsync_AgregaNotaCredito));
        var repo = new NotaCreditoRepository(context);
        var nota = CrearNotaCredito();

        await repo.AgregarAsync(nota);

        Assert.AreEqual(1, context.NotasCredito.Count());
    }

    [TestMethod]
    public async Task ObtenerPorIdAsync_DevuelveNotaCredito()
    {
        var context = GetDbContext(nameof(ObtenerPorIdAsync_DevuelveNotaCredito));
        var nota = CrearNotaCredito();
        context.NotasCredito.Add(nota);
        context.SaveChanges();
        var repo = new NotaCreditoRepository(context);

        var result = await repo.ObtenerPorIdAsync(nota.Id);

        Assert.IsNotNull(result);
        Assert.AreEqual(nota.Id, result.Id);
    }

    [TestMethod]
    public async Task ObtenerNotasCreditoAsync_DevuelveTodas()
    {
        var context = GetDbContext(nameof(ObtenerNotasCreditoAsync_DevuelveTodas));
        context.NotasCredito.Add(CrearNotaCredito());
        context.NotasCredito.Add(CrearNotaCredito());
        context.SaveChanges();
        var repo = new NotaCreditoRepository(context);

        var result = await repo.ObtenerNotasCreditoAsync();

        Assert.AreEqual(2, result.Count());
    }

    [TestMethod]
    public async Task ObtenerPorIdAsync_DevuelveDetalles()
    {
        var context = GetDbContext(nameof(ObtenerPorIdAsync_DevuelveDetalles));
        var detalle = new DetalleNotaCredito
        {
            Id = Guid.NewGuid(),
            Cantidad = 2,
            PrecioUnitario = 100,
            ProductoId = Guid.NewGuid(),
            NotaCreditoId = Guid.NewGuid()
        };
        var nota = CrearNotaCredito(new List<DetalleNotaCredito> { detalle });
        context.NotasCredito.Add(nota);
        context.SaveChanges();
        var repo = new NotaCreditoRepository(context);

        var result = await repo.ObtenerPorIdAsync(nota.Id);

        Assert.IsNotNull(result);
        Assert.IsTrue(result.Detalles.Any());
    }
}