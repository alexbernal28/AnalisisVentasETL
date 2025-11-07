using AnalisisVentasETL.Application.Repositories.Csv;
using AnalisisVentasETL.Domain.Entities.Csv;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Globalization;
using CsvHelper;

namespace AnalisisVentasETL.Persistence.Sources.CSV.Repositories
{
    public sealed class ProductRepository : IProductRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProductRepository> _logger;
        private readonly string _filePath;

        public ProductRepository(IConfiguration configuration, ILogger<ProductRepository> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _filePath = _configuration.GetSection("CsvSettings:ProductFilePath").Value ?? string.Empty;
        }

        public async Task<List<Product>> GetAll()
        {
            _logger.LogInformation("Reading products from CSV file at {FilePath}", _filePath);
            List<Product> products = new List<Product>();

            if (!File.Exists(_filePath))
            {
                _logger.LogWarning("The CSV file at {FilePath} does not exist.", _filePath);
                return products;
            }

            try
            {
                using var reader = new StreamReader(_filePath);

                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                await foreach (var record in csv.GetRecordsAsync<Product>())
                {
                    products.Add(record);
                }

                _logger.LogInformation("Successfully read {Count} products from CSV.", products.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while reading products from CSV.");
                throw;
            }
            return products;
        }
    }
}
