using AnalisisVentasETL.Application.Repositories.Csv;
using AnalisisVentasETL.Application.Repositories.DB;
using AnalisisVentasETL.Application.Repositories.Dwh;
using AnalisisVentasETL.Domain.Entities.API;
using AnalisisVentasETL.Domain.Entities.Db;
using AnalisisVentasETL.Domain.Entities.Dwh.Dimensions;
using AnalisisVentasETL.Persistence.Sources.API.Repositories;

namespace AnalisisVentasETL.WRKVentas.Loaders
{
    public class DimProductLoader
    {
        private readonly IProductRepository _csvRepository;
        private readonly IDatabaseExtractor _dbRepository;
        private readonly IAPIExtractor _apiRepository;
        private readonly IDimProductRepository _dimProductRepository;
        private readonly ILogger<DimProductLoader> _logger;

        public DimProductLoader(
            IProductRepository csvRepository,
            IDatabaseExtractor dbRepository,
            IAPIExtractor apiRepository,
            IDimProductRepository dimProductRepository,
            ILogger<DimProductLoader> logger)
        {
            _csvRepository = csvRepository;
            _dbRepository = dbRepository;
            _apiRepository = apiRepository;
            _dimProductRepository = dimProductRepository;
            _logger = logger;
        }

        public async Task LoadAsync()
        {
            _logger.LogInformation("Cargando DimProduct...");

            await _dimProductRepository.DeleteAll();

            // EXTRAER
            var csv = await _csvRepository.GetAll();
            var db = await _dbRepository.GetAllAsync<ProductDB>();
            var api = await _apiRepository.GetDataAsync<ProductAPI>("api/products");

            // MAPEAR
            var all = new List<DimProduct>();

            all.AddRange(csv.Select(p => new DimProduct
            {
                ProductID = p.ProductID,
                Name = p.ProductName,
                Category = p.Category,
                UnitPrice = p.Price,
                Stock = p.Stock,
                UploadDate = DateTime.UtcNow
            }));

            all.AddRange(db.Select(p => new DimProduct
            {
                ProductID = p.ProductID,
                Name = p.ProductName,
                Category = p.Category,
                UnitPrice = p.Price,
                Stock = p.Stock,
                UploadDate = DateTime.UtcNow
            }));

            all.AddRange(api.Select(p => new DimProduct
            {
                ProductID = p.ProductID,
                Name = p.ProductName,
                Category = p.Category,
                UnitPrice = p.Price,
                Stock = p.Stock,
                UploadDate = DateTime.UtcNow
            }));

            // QUITAR DUPLICADOS
            var unique = all
                .GroupBy(p => p.ProductID)
                .Select(g => g.First())
                .ToList();

            await _dimProductRepository.SaveAll(unique.ToArray());

            _logger.LogInformation("DimProduct cargado correctamente ({Count} registros únicos).", unique.Count);
        }
    }
}
