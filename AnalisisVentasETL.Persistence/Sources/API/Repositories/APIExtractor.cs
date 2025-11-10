using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace AnalisisVentasETL.Persistence.Sources.API.Repositories
{
    public class APIExtractor : IAPIExtractor
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<APIExtractor> _logger;
        private readonly string _baseUrl;

        public APIExtractor(HttpClient httpClient, ILogger<APIExtractor> logger, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
            _baseUrl = configuration["ApiSettings:BaseUrl"] ?? throw new ArgumentNullException("API Base URL is not configured.");
        }

        public async Task<List<T>> GetDataAsync<T>(string endpoint)
        {
            try
            {
                var fullUrl = $"{_baseUrl.TrimEnd('/')}/{endpoint.TrimStart('/')}";
                _logger.LogInformation("Solicitando datos desde API externa: {Url}", fullUrl);

                var response = await _httpClient.GetAsync(fullUrl);
                var contentType = response.Content.Headers.ContentType?.MediaType;

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error HTTP {StatusCode} al consumir {Url}", response.StatusCode, fullUrl);
                    return new List<T>();
                }

                // Verificar si la respuesta es JSON
                if (contentType == null || !contentType.Contains("json", StringComparison.OrdinalIgnoreCase))
                {
                    var contentPreview = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Respuesta no JSON recibida desde API externa: {Preview}", contentPreview.Substring(0, Math.Min(200, contentPreview.Length)));
                    return new List<T>();
                }

                var data = await response.Content.ReadFromJsonAsync<List<T>>();
                _logger.LogInformation("Datos recibidos correctamente desde {Url}", fullUrl);

                return data ?? new List<T>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consumir la API externa en {Endpoint}", endpoint);
                return new List<T>();
            }
        }
    }
}
