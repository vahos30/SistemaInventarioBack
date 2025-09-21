using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

public class FactusFacturaService
{
    private readonly HttpClient _httpClient;
    private readonly FactusAuthService _authService;
    private readonly IConfiguration _config;

    public FactusFacturaService(HttpClient httpClient, FactusAuthService authService, IConfiguration config)
    {
        _httpClient = httpClient;
        _authService = authService;
        _config = config;
    }

    public async Task<string> CrearFacturaAsync(object facturaRequest)
    {
        var token = await _authService.GetAccessTokenAsync();

        var request = new HttpRequestMessage(HttpMethod.Post, $"{_config["Factus:UrlApi"]}/v1/bills/validate");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var json = JsonSerializer.Serialize(facturaRequest, new JsonSerializerOptions { WriteIndented = true });
        Console.WriteLine("JSON FINAL ENVIADO A FACTUS:");
        Console.WriteLine(json);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            // Imprime el error detallado de Factus en consola/log
            Console.WriteLine("Error Factus:");
            Console.WriteLine(responseBody);
            throw new HttpRequestException($"Error Factus: {response.StatusCode} - {responseBody}");
        }

        return responseBody;
    }
}