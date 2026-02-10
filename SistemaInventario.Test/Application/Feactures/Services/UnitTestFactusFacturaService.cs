using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using SistemaInventario.Application.Services;
using Moq.Protected;

public class FakeFactusAuthService : FactusAuthService
{
    public FakeFactusAuthService() : base(null!, null!) { }
    public override Task<string> GetAccessTokenAsync()
    {
        return Task.FromResult("fake-token");
    }
}

[TestClass]
public class UnitTestFactusFacturaService
{
    private Mock<IConfiguration> _configMock = null!;
    private HttpClient _httpClient = null!;
    private FactusFacturaService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        var fakeAuthService = new FakeFactusAuthService();

        // Mock de configuración
        _configMock = new Mock<IConfiguration>();
        var sectionMock = new Mock<IConfigurationSection>();
        sectionMock.Setup(s => s.Value).Returns("https://api-factus.test");
        _configMock.Setup(c => c["Factus:UrlApi"]).Returns("https://api-factus.test");

        // Mock de HttpClient usando HttpMessageHandler
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"result\":\"ok\"}")
            });

        _httpClient = new HttpClient(handlerMock.Object);

        _service = new FactusFacturaService(_httpClient, fakeAuthService, _configMock.Object);
    }

    [TestMethod]
    public async Task CrearFacturaAsync_RespuestaExitosa_RetornaContenido()
    {
        // Arrange
        var facturaRequest = new { Cliente = "Test", Total = 1000 };

        // Act
        var result = await _service.CrearFacturaAsync(facturaRequest);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Contains("ok"));
    }
}
