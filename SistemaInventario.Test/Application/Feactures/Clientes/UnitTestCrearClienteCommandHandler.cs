using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AutoMapper;
using SistemaInventario.Application.Feactures.Clientes;
using SistemaInventario.Application.DTOs;
using SistemaInventario.Domain.Entities;
using SistemaInventario.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaInventario.Test.Application.Feactures.Clientes
{
    [TestClass]
    public class UnitTestCrearClienteCommandHandler
    {
        [TestMethod]
        public async Task Handle_CrearCliente_DevuelveClienteDto()
        {
            // Arrange
            var clienteRepoMock = new Mock<IClienteRepository>();
            var mapperMock = new Mock<IMapper>();
            var ciudadServiceMock = new Mock<CiudadService>(null, null, null);

            var ciudades = new List<CiudadDto>
            {
                new CiudadDto { id = 05001, code = "001", name = "Medellín", department = "Antioquia" }
            };
            ciudadServiceMock.Setup(s => s.ObtenerCiudadesAsync()).ReturnsAsync(ciudades);

            var command = new CrearClienteCommand
            {
                Nombre = "Juan",
                Apellido = "Pérez",
                TipoDocumento = "CC",
                NumeroDocumento = "123456789",
                Telefono = "1234567",
                Direccion = "Calle 1",
                Email = "juan@correo.com",
                CiudadId = 05001,
                IdTipoOrganizacion = 1,
                IdTributo = 2,
                IdTipoDocumentoIdentidad = 3,
                RazonSocial = "Empresa S.A."
            };

            Cliente? clienteAgregado = null;
            clienteRepoMock.Setup(r => r.AgregarAsync(It.IsAny<Cliente>()))
                .Callback<Cliente>(c => clienteAgregado = c)
                .Returns(Task.CompletedTask);

            var clienteDto = new ClienteDto { Nombre = "Juan", Apellido = "Pérez" };
            mapperMock.Setup(m => m.Map<ClienteDto>(It.IsAny<Cliente>())).Returns(clienteDto);

            var handler = new CrearClienteCommandHandler(
                clienteRepoMock.Object,
                mapperMock.Object,
                ciudadServiceMock.Object
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Juan", result.Nombre);
            Assert.AreEqual("Pérez", result.Apellido);
            Assert.IsNotNull(clienteAgregado);
            Assert.AreEqual("001", clienteAgregado.CodigoCiudad);
            Assert.AreEqual("Medellín", clienteAgregado.Ciudad);
            Assert.AreEqual("Antioquia", clienteAgregado.Departamento);
            Assert.AreEqual(05001, clienteAgregado.CiudadId);
        }

        [TestMethod]
        [ExpectedException(typeof(System.Exception))]
        public async Task Handle_CiudadNoEncontrada_LanzaExcepcion()
        {
            // Arrange
            var clienteRepoMock = new Mock<IClienteRepository>();
            var mapperMock = new Mock<IMapper>();
            var ciudadServiceMock = new Mock<CiudadService>(null, null, null);

            ciudadServiceMock.Setup(s => s.ObtenerCiudadesAsync()).ReturnsAsync(new List<CiudadDto>());

            var command = new CrearClienteCommand { CiudadId = 99999 };

            var handler = new CrearClienteCommandHandler(
                clienteRepoMock.Object,
                mapperMock.Object,
                ciudadServiceMock.Object
            );

            // Act
            await handler.Handle(command, CancellationToken.None);
        }
    }
}