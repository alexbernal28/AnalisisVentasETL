using AnalisisVentasETL.Application.Repositories.Csv;
using AnalisisVentasETL.Domain.Entities.Csv;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace AnalisisVentasETL.Persistence.Sources.CSV.Repositories
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<OrderDetailRepository> _logger;
        private readonly string _filePath;

        public OrderDetailRepository(IConfiguration configuration, ILogger<OrderDetailRepository> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _filePath = _configuration.GetSection("CsvSettings:OrderDetailFilePath").Value ?? string.Empty;
        }

        public async Task<List<OrderDetail>> GetAll()
        {
            _logger.LogInformation("Reading order details from CSV file at {FilePath}", _filePath);
            List<OrderDetail> orderDetails = new List<OrderDetail>();
            if (!File.Exists(_filePath))
            {
                _logger.LogWarning("The CSV file at {FilePath} does not exist.", _filePath);
                return orderDetails;
            }
            try
            {
                using var reader = new StreamReader(_filePath);
                using var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture);
                await foreach (var record in csv.GetRecordsAsync<OrderDetail>())
                {
                    orderDetails.Add(record);
                }
                _logger.LogInformation("Successfully read {Count} order details from CSV.", orderDetails.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while reading order details from CSV.");
                throw;
            }
            return orderDetails;
        }
    }
}
