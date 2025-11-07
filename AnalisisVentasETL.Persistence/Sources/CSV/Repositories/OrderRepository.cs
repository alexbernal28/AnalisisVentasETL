using AnalisisVentasETL.Application.Repositories.Csv;
using AnalisisVentasETL.Domain.Entities.Csv;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace AnalisisVentasETL.Persistence.Sources.CSV.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<OrderRepository> _logger;
        private readonly string _filePath;

        public OrderRepository(IConfiguration configuration, ILogger<OrderRepository> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _filePath = _configuration.GetSection("CsvSettings:OrderFilePath").Value ?? string.Empty;
        }

        public async Task<List<Order>> GetAll()
        {
            _logger.LogInformation("Reading orders from CSV file at {FilePath}", _filePath);
            List<Order> orders = new List<Order>();
            if (!File.Exists(_filePath))
            {
                _logger.LogWarning("The CSV file at {FilePath} does not exist.", _filePath);
                return orders;
            }
            try
            {
                using var reader = new StreamReader(_filePath);
                using var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture);
                await foreach (var record in csv.GetRecordsAsync<Order>())
                {
                    orders.Add(record);
                }
                _logger.LogInformation("Successfully read {Count} orders from CSV.", orders.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while reading orders from CSV.");
                throw;
            }
            return orders;
        }
    }
}
