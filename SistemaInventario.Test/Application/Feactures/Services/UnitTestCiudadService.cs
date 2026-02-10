using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;


[TestClass]
public class UnitTestCiudadService
{
    [TestMethod]
    public async Task ObtenerCiudadesAsync_ReturnsCiudadesList()
    {
        // Arrange
        var ciudades = new List<CiudadDto> { new CiudadDto { department = "Antioquia", name = "Medellín" } };
        var apiResponse = new CiudadApiResponse { data = ciudades };
        var json = System.Text.Json.JsonSerializer.Serialize(apiResponse);

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json)
            });

        var httpClient = new HttpClient(handlerMock.Object);

        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["Factus:UrlApi"]).Returns("http://fakeapi");

        var authServiceMock = new Mock<FactusAuthService>(httpClient, configMock.Object);
        authServiceMock.Setup(a => a.GetAccessTokenAsync()).ReturnsAsync("fake-token");

        var service = new CiudadService(httpClient, configMock.Object, authServiceMock.Object);

        // Act
        var result = await service.ObtenerCiudadesAsync();

        // Assert
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Medellín", result[0].name);
    }

    [TestMethod]
    public void FiltrarPorDepartamento_ReturnsFilteredList()
    {
        // Arrange
        var ciudades = new List<CiudadDto>
        {
            new CiudadDto { department = "Antioquia", name = "Medellín" },
            new CiudadDto { department = "Cundinamarca", name = "Bogotá" }
        };
        var service = new CiudadService(null, null, null);

        // Act
        var result = service.FiltrarPorDepartamento(ciudades, "Antioquia");

        // Assert
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Medellín", result[0].name);
    }
}
