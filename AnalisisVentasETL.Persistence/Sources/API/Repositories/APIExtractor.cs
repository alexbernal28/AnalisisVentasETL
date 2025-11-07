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
            _baseUrl = configuration["ApiSettings::BaseUrl"] ?? throw new ArgumentNullException("API Base URL is not configured.");
        }

        public async Task<List<T>> GetDataAsync<T>(string endpoint)
        {
            try
            {
                var fullUrl = $"{_baseUrl.TrimEnd('/')}/{endpoint.TrimStart('/')}";
                _logger.LogInformation("Solicitando datos desde API externa: {Url}", fullUrl);

                var data = await _httpClient.GetFromJsonAsync<List<T>>(fullUrl);

                if (data == null)
                {
                    _logger.LogWarning("La respuesta de la API en {Url} fue vacía.", fullUrl);
                    return new List<T>();
                }

                _logger.LogInformation("Datos recibidos correctamente desde {Url}", fullUrl);
                return data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consumir la API externa en {Endpoint}", endpoint);
                return new List<T>();
            }
        }
    }
}
