using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SistemaInventario.Application.Feactures.Clientes;
using SistemaInventario.Application.Mapping;
using SistemaInventario.Domain.Entities;
using SistemaInventario.Domain.Interfaces;

namespace SistemaInventario.Test.Application
{
    [TestClass]
    public class UnitTestCrearClienteCommandHandler
    {
        private Mock<IClienteRepository> _clienteRepositoryMock;
        private IMapper _mapper;
        private CrearClienteCommandHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            //configuramos el mock para IclienteRepository
            _clienteRepositoryMock = new Mock<IClienteRepository>();

            //configuramos AutoMapper usando el MappingProfile de la aplicacion
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            _mapper = config.CreateMapper();

            //creamos la instancia del handler inyectandq el mock y el mapper
            _handler = new CrearClienteCommandHandler(_clienteRepositoryMock.Object, _mapper);

        }

        [TestMethod]
        public async Task Handle_ValidCommand_ShouldCreateCliente()
        {
            //Arrange
            var command = new CrearClienteCommand
            {
                Nombre = "Cliente 1",
                NumeroDocumento = "123456",
                Direccion = "Calle 123",
                Telefono = "1234567890",
                Email = "prueba@prueba.com"
            };
            
            //configuramos el mock para que el metodo AgregarAsync se complete sin errores
            _clienteRepositoryMock.
                Setup(repo => repo.AgregarAsync(It.IsAny<Cliente>())).
                Returns(Task.CompletedTask);

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(command.Nombre, result.Nombre);
            Assert.AreEqual(command.NumeroDocumento, result.NumeroDocumento);
            Assert.AreEqual(command.Direccion, result.Direccion);
            Assert.AreEqual(command.Telefono, result.Telefono);
            Assert.AreEqual(command.Email, result.Email);

            //verificamos que el metodo AgregarAsync fue llamado una vez
            _clienteRepositoryMock.Verify(repo => repo.AgregarAsync(It.IsAny<Cliente>()), Times.Once);
        }
    }
}
