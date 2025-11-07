using AnalisisVentasETL.Application.Repositories.Csv;
using AnalisisVentasETL.Domain.Entities.Csv;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace AnalisisVentasETL.Persistence.Sources.CSV.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<CustomerRepository> _logger;
        private readonly string _filePath;

        public CustomerRepository(IConfiguration configuration, ILogger<CustomerRepository> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _filePath = _configuration.GetSection("CsvSettings:CustomerFilePath").Value ?? string.Empty;
        }

        public async Task<List<Customer>> GetAll()
        {
            _logger.LogInformation("Reading customers from CSV file at {FilePath}", _filePath);
            List<Customer> customers = new List<Customer>();
            if (!File.Exists(_filePath))
            {
                _logger.LogWarning("The CSV file at {FilePath} does not exist.", _filePath);
                return customers;
            }
            try
            {
                using var reader = new StreamReader(_filePath);
                using var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture);
                await foreach (var record in csv.GetRecordsAsync<Customer>())
                {
                    customers.Add(record);
                }
                _logger.LogInformation("Successfully read {Count} customers from CSV.", customers.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while reading customers from CSV.");
                throw;
            }
            return customers;
        }
    }
}
