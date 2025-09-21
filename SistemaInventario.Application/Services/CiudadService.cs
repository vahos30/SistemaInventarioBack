using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;

public class CiudadService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly FactusAuthService _authService;

    public CiudadService(HttpClient httpClient, IConfiguration config, FactusAuthService authService)
    {
        _httpClient = httpClient;
        _config = config;
        _authService = authService;
    }

    public async Task<List<CiudadDto>> ObtenerCiudadesAsync(string name = "")
    {
        var token = await _authService.GetAccessTokenAsync();
        var url = $"{_config["Factus:UrlApi"]}/v1/municipalities?name={name}";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<CiudadApiResponse>(json);
        return result?.data ?? new List<CiudadDto>();
    }

    public List<CiudadDto> FiltrarPorDepartamento(List<CiudadDto> ciudades, string departamento)
    {
        return ciudades
            .Where(c => c.department.Equals(departamento, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
}