using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

public class FactusAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public FactusAuthService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<string> GetAccessTokenAsync()
    {
        var urlApi = _config["Factus:UrlApi"]; // Debe ser "https://api-sandbox.factus.com.co"
        var clientId = _config["Factus:ClientId"];
        var clientSecret = _config["Factus:ClientSecret"];
        var email = _config["Factus:Email"];
        var password = _config["Factus:Password"];

        var request = new HttpRequestMessage(HttpMethod.Post, $"{urlApi}/oauth/token");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var content = new MultipartFormDataContent
        {
            { new StringContent("password"), "grant_type" },
            { new StringContent(clientId), "client_id" },
            { new StringContent(clientSecret), "client_secret" },
            { new StringContent(email), "username" },
            { new StringContent(password), "password" }
        };
        request.Content = content;

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("access_token").GetString();
    }
}