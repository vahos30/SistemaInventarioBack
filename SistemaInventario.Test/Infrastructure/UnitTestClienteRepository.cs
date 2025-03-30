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
    public class UnitTestClienteRepository
    {
        private DbContextOptions<AppDbContext> _options;
        private AppDbContext _context;
        private ClienteRepository _clienteRepository;

        [TestInitialize]
        public void Setup()
        {
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new AppDbContext(_options);
            _clienteRepository = new ClienteRepository(_context);
        }

        [TestMethod]
        public async Task AgregarAsync()
        {
            // Arrange
            var cliente = new Cliente { Nombre = "Cliente 1" };

            // Act
            await _clienteRepository.AgregarAsync(cliente);

            // Assert
            var clienteGuardado = await _context.Clientes.FirstOrDefaultAsync();


            Assert.AreEqual(cliente.Nombre, clienteGuardado?.Nombre); 
            Assert.AreEqual(cliente.Id, clienteGuardado?.Id);

        }

        [TestMethod]
        public async Task EliminarAsync()
        {
            // Arrange
            var cliente = new Cliente { Id = Guid.NewGuid() };
            await _context.Clientes.AddAsync(cliente);
            await _context.SaveChangesAsync();
            // Act
            await _clienteRepository.EliminarAsync(cliente.Id);
            // Assert
            var clienteEliminado = await _context.Clientes.FindAsync(cliente.Id);
            Assert.IsNull(clienteEliminado);
        }

        [TestMethod]
        public async Task ActualizarAsync()
        {
            // Arrange
            var cliente = new Cliente { Nombre = "Cliente 1" };
            await _context.Clientes.AddAsync(cliente);
            await _context.SaveChangesAsync();
            // Act
            cliente.Nombre = "Cliente 2";
            await _clienteRepository.ActualizarAsync(cliente);
            // Assert
            var clienteActualizado = await _context.Clientes.FindAsync(cliente.Id);
            Assert.AreEqual("Cliente 2", clienteActualizado.Nombre);
        }

        [TestMethod]
        public async Task ObtenerTodosAsync()
        {
            // Arrange
            var cliente1 = new Cliente { Nombre = "Cliente 1" };
            var cliente2 = new Cliente { Nombre = "Cliente 2" };
            await _context.Clientes.AddRangeAsync(cliente1, cliente2);
            await _context.SaveChangesAsync();
            // Act
            var clientes = await _clienteRepository.ObtenerTodosAsync();
            // Assert
            Assert.AreEqual(2, clientes.Count());
        }
    }
}