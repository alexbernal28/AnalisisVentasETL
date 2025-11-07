using AnalisisVentasETL.Application.Dtos;
using AnalisisVentasETL.Application.Repositories.Csv;
using AnalisisVentasETL.Application.Repositories.DB;
using AnalisisVentasETL.Application.Repositories.Dwh;
using AnalisisVentasETL.Domain.Entities.API;
using AnalisisVentasETL.Domain.Entities.Csv;
using AnalisisVentasETL.Domain.Entities.Db;
using AnalisisVentasETL.Domain.Entities.Dwh.Dimensions;
using AnalisisVentasETL.Persistence.Sources.API.Repositories;
using System.Linq;

namespace AnalisisVentasETL.WRKVentas
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IProductRepository _productCsvRepository;
        private readonly IDatabaseExtractor _databaseExtractor;
        private readonly IAPIExtractor _apiExtractor;
        private readonly IDimProductRepository _dimProductRepository;

        public Worker(
            ILogger<Worker> logger,
            IProductRepository productCsvRepository,
            IDatabaseExtractor databaseExtractor,
            IAPIExtractor apiExtractor,
            IDimProductRepository dimProductRepository
        )
        {
            _logger = logger;
            _productCsvRepository = productCsvRepository;
            _databaseExtractor = databaseExtractor;
            _apiExtractor = apiExtractor;
            _dimProductRepository = dimProductRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("=== ETL Worker iniciado ===");

            try
            {
                // 1️ EXTRACT (Desde CSV)
                _logger.LogInformation("Extrayendo productos desde CSV...");
                var csvProducts = await _productCsvRepository.GetAll();
                _logger.LogInformation($"Productos CSV extraídos: {csvProducts.Count()}");

                // 2️ EXTRACT (Desde BD)
                _logger.LogInformation("Extrayendo datos desde base de datos transaccional...");
                var dbProductsRaw = await _databaseExtractor.GetAllAsync<ProductDB>();
                var dbProducts = dbProductsRaw.Select(p => new Product
                {
                    ProductID = p.ProductID,
                    ProductName = p.ProductName,
                    Category = p.Category,
                    Price = p.Price,
                    Stock = p.Stock,
                }).ToList();
                _logger.LogInformation($"Productos BD extraídos: {dbProducts.Count()}");

                // 3️ EXTRACT (Desde API)
                _logger.LogInformation("Extrayendo datos desde API externa...");
                var apiProductsRaw = await _apiExtractor.GetDataAsync<ProductAPI>("api/products");
                _logger.LogInformation($"Productos API extraídos: {apiProductsRaw.Count()}");

                // 4️ TRANSFORM (Unificación simple)
                var csvMapped = csvProducts.Select(p => new ProductUnifiedDto
                {
                    ProductID = p.ProductID,
                    ProductName = p.ProductName,
                    Category = p.Category,
                    Price = p.Price,
                    Stock = p.Stock,
                    Source = "CSV"
                }).ToList();

                var dbMapped = dbProductsRaw.Select(p => new ProductUnifiedDto
                {
                    ProductID = p.ProductID,
                    ProductName = p.ProductName,
                    Category = p.Category,
                    Price = p.Price,
                    Stock = p.Stock,
                    Source = "DB"
                }).ToList();

                var apiMapped = apiProductsRaw.Select(p => new ProductUnifiedDto
                {
                    ProductID = p.ProductID,
                    ProductName = p.ProductName,
                    Category = p.Category,
                    Price = p.Price,
                    Stock = p.Stock,
                    Source = "API"
                }).ToList();


                var allProducts = csvMapped
                    .Concat(dbMapped)
                    .Concat(apiMapped)
                    .ToList();

                var unique = allProducts.DistinctBy(p => p.ProductID).ToList();

                // 5️ LOAD (Carga en DWH)
                var dimProducts = unique.Select(p => new DimProduct
                {
                    ProductID = p.ProductID,
                    Name = p.ProductName,
                    Category = p.Category,
                    UnitPrice = p.Price,
                    Stock = p.Stock,
                    UploadDate = DateTime.UtcNow
                }).ToList();

                await _dimProductRepository.BulkInsertAsync(dimProducts);

                _logger.LogInformation("Carga completada en la Dimensión Producto (DWH).");
                _logger.LogInformation("=== ETL finalizado correctamente ===");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante la ejecución del proceso ETL.");
            }
        }
    }
}
