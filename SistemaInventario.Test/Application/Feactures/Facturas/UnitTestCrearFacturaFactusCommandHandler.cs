using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SistemaInventario.Application.Feactures.Facturas;
using SistemaInventario.Domain.Entities;
using SistemaInventario.Application.Services;
using SistemaInventario.Domain.Interfaces;
using SistemaInventario.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using Moq.Protected;

namespace SistemaInventario.Test.Application.Feactures.Facturas
{
    [TestClass]
    public class UnitTestCrearFacturaFactusCommandHandler
    {
        private Mock<IClienteRepository> _mockClienteRepo = null!;
        private Mock<IProductoRepository> _mockProductoRepo = null!;
        private Mock<IFacturaRepository> _mockFacturaRepo = null!;
        private CrearFacturaFactusCommandHandler _handler = null!;

        [TestInitialize]
        public void Setup()
        {
            // Mock HttpClient para devolver una respuesta dummy
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync((HttpRequestMessage request, CancellationToken token) =>
                {
                    // Inicializa con un valor por defecto
                    string content = "{}";
                    if (request.RequestUri != null && (
                            request.RequestUri.ToString().Contains("token") ||
                            request.RequestUri.ToString().Contains("auth") ||
                            request.RequestUri.ToString().Contains("login")
                        ))
                    {
                        content = "{\"access_token\":\"dummy_token\"}";
                    }
                    else if (request.RequestUri != null)
                    {
                        content = "{\"status\":\"success\",\"message\":\"ok\",\"data\":{\"bill\":{\"id\":1,\"number\":\"F001\",\"reference_code\":\"REF123\",\"total\":\"1000\",\"observation\":\"obs\"},\"items\":[{\"name\":\"Producto 1\",\"quantity\":\"2\",\"price\":\"500\",\"tax_rate\":\"19\",\"tax_amount\":\"95\"}]}}";
                    }
                    // Siempre retorna un string válido
                    return new HttpResponseMessage
                    {
                        StatusCode = System.Net.HttpStatusCode.OK,
                        Content = new StringContent(content)
                    };
                });

            var httpClient = new HttpClient(handlerMock.Object);

            // Mock IConfiguration para FactusAuthService y FactusFacturaService
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c["Factus:UrlApi"]).Returns("http://dummy-url");
            mockConfig.Setup(c => c["Factus:ClientId"]).Returns("dummy-client-id");
            mockConfig.Setup(c => c["Factus:ClientSecret"]).Returns("dummy-client-secret");
            mockConfig.Setup(c => c["Factus:Email"]).Returns("dummy@mail.com");         // <-- Añade esto
            mockConfig.Setup(c => c["Factus:Password"]).Returns("dummy-password");      // <-- Y esto

            // Instancia real de FactusAuthService con dependencias mockeadas
            var factusAuthService = new FactusAuthService(httpClient, mockConfig.Object);

            // Instancia real de FactusFacturaService con dependencias mockeadas
            var factusService = new FactusFacturaService(httpClient, factusAuthService, mockConfig.Object);

            _mockClienteRepo = new Mock<IClienteRepository>();
            _mockProductoRepo = new Mock<IProductoRepository>();
            _mockFacturaRepo = new Mock<IFacturaRepository>();

            _handler = new CrearFacturaFactusCommandHandler(
                factusService,
                _mockClienteRepo.Object,
                _mockProductoRepo.Object,
                _mockFacturaRepo.Object
            );
        }

        [TestMethod]
        public async Task Handle_ValidCommand_ShouldCreateFacturaAndReturnFactusJson()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var productoId = Guid.NewGuid();

            var cliente = new Cliente
            {
                Id = clienteId,
                Nombre = "Juan",
                Apellido = "Pérez",
                NumeroDocumento = "123",
                IdTipoOrganizacion = 1,
                IdTributo = 1,
                IdTipoDocumentoIdentidad = 1,
                CiudadId = 80,
                Direccion = "Calle 1",
                Email = "test@mail.com",
                Telefono = "123"
            };

            var producto = new Producto
            {
                Id = productoId,
                Nombre = "Producto 1",
                CantidadStock = 10,
                Descripcion = "Desc",
                Referencia = "Ref"
            };

            _mockClienteRepo.Setup(r => r.ObtenerPorIdsync(clienteId)).ReturnsAsync(cliente);
            _mockProductoRepo.Setup(r => r.ObtenerPorIdsync(productoId)).ReturnsAsync(producto);
            _mockFacturaRepo.Setup(r => r.AgregarAsync(It.IsAny<Factura>())).Returns(Task.CompletedTask);
            _mockProductoRepo.Setup(r => r.ActualizarAsync(It.IsAny<Producto>())).Returns(Task.CompletedTask);

            var command = new CrearFacturaFactusCommand
            {
                ClienteId = clienteId,
                Detalles = new List<DetalleFacturaFactusDto>
                {
                    new DetalleFacturaFactusDto
                    {
                        ProductoId = productoId,
                        Cantidad = 2,
                        PrecioUnitario = 500,
                        TipoDescuento = string.Empty,
                        ValorDescuento = null
                    }
                },
                Referencia = string.Empty,
                Observacion = "obs",
                FormaPago = "1",
                MetodoPago = "10"
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.Contains("\"status\":\"success\""));
            _mockFacturaRepo.Verify(r => r.AgregarAsync(It.IsAny<Factura>()), Times.Once);
            _mockProductoRepo.Verify(r => r.ActualizarAsync(It.IsAny<Producto>()), Times.Once);
        }
    }
}