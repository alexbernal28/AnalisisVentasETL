using AnalisisVentasETL.Application.Repositories.Csv;
using AnalisisVentasETL.Application.Repositories.DB;
using AnalisisVentasETL.Application.Repositories.Dwh;
using AnalisisVentasETL.Domain.Entities.API;
using AnalisisVentasETL.Domain.Entities.Db;
using AnalisisVentasETL.Domain.Entities.Dwh.Dimensions;
using AnalisisVentasETL.Persistence.Sources.API.Repositories;

namespace AnalisisVentasETL.WRKVentas.Loaders
{
    public class DimCustomerLoader
    {
        private readonly ICustomerRepository _csvRepository;
        private readonly IDatabaseExtractor _dbRepository;
        private readonly IAPIExtractor _apiRepository;
        private readonly IDimCustomer _dimCustomerRepository;
        private readonly ILogger<DimCustomerLoader> _logger;
        public DimCustomerLoader(
            ICustomerRepository csvRepository,
            IDatabaseExtractor dbRepository,
            IAPIExtractor apiRepository,
            IDimCustomer dimCustomerRepository,
            ILogger<DimCustomerLoader> logger)
        {
            _csvRepository = csvRepository;
            _dbRepository = dbRepository;
            _apiRepository = apiRepository;
            _dimCustomerRepository = dimCustomerRepository;
            _logger = logger;
        }
        public async Task LoadAsync()
        {
            _logger.LogInformation("Cargando DimCustomer...");
            await _dimCustomerRepository.DeleteAll();

            // EXTRAER
            var csv = await _csvRepository.GetAll();
            var db = await _dbRepository.GetAllAsync<CustomerDB>();
            var api = await _apiRepository.GetDataAsync<CustomerAPI>("api/customers");

            // MAPEAR
            var all = new List<DimCustomer>();

            all.AddRange(csv.Select(c => new DimCustomer
            {
                CustomerId = c.CustomerId,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email,
                Country = c.Country,
                Region = c.City,
                gender = "N/A",
                CustomerType = "Regular",
                UploadDate = DateTime.UtcNow
            }));

            all.AddRange(db.Select(c => new DimCustomer
            {
                CustomerId = c.CustomerId,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email,
                Country = c.Country,
                Region = c.City,
                gender = "N/A",
                CustomerType = "Regular",
                UploadDate = DateTime.UtcNow
            }));

            all.AddRange(api.Select(c => new DimCustomer
            {
                CustomerId = c.CustomerId,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email,
                Country = c.Country,
                Region = c.City,
                gender = "N/A",
                CustomerType = "Regular",
                UploadDate = DateTime.UtcNow
            }));

            var uniqueCustomers = all
                .GroupBy(c => c.CustomerId)
                .Select(g => g.First())
                .ToList();

            // CARGAR
            await _dimCustomerRepository.SaveAll(uniqueCustomers.ToArray());
            _logger.LogInformation("DimCustomer cargado correctamente ({Count} filas).", all.Count);
        }
    }
}
