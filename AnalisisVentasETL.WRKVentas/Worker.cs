using AnalisisVentasETL.Application.Dtos;
using AnalisisVentasETL.Application.Repositories.Csv;
using AnalisisVentasETL.Application.Repositories.DB;
using AnalisisVentasETL.Application.Repositories.Dwh;
using AnalisisVentasETL.Domain.Entities.API;
using AnalisisVentasETL.Domain.Entities.Csv;
using AnalisisVentasETL.Domain.Entities.Db;
using AnalisisVentasETL.Domain.Entities.Dwh.Dimensions;
using AnalisisVentasETL.Persistence.Sources.API.Repositories;
using AnalisisVentasETL.WRKVentas.Loaders;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Linq;

namespace AnalisisVentasETL.WRKVentas
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("=== ETL Worker iniciado ===");

            // Crear un scope para los servicios Scoped
            using var scope = _scopeFactory.CreateScope();

            // Obtener todos los repositorios necesarios
            //var productCsvRepo = scope.ServiceProvider.GetRequiredService<IProductRepository>();
            //var customerCsvRepo = scope.ServiceProvider.GetRequiredService<ICustomerRepository>();
            //var orderCsvRepo = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
            //var orderDetailCsvRepo = scope.ServiceProvider.GetRequiredService<IOrderDetailRepository>();
            
            //var databaseExtractor = scope.ServiceProvider.GetRequiredService<IDatabaseExtractor>();
            //var apiExtractor = scope.ServiceProvider.GetRequiredService<IAPIExtractor>();
            
            //var dimProductRepo = scope.ServiceProvider.GetRequiredService<IDimProductRepository>();
            //var dimCustomerRepo = scope.ServiceProvider.GetRequiredService<IDimCustomer>();
            //var dimTimeRepo = scope.ServiceProvider.GetRequiredService<IDimTime>();
            //var dimDataSourceRepo = scope.ServiceProvider.GetRequiredService<IDimDataSource>();

            var dsLoader = scope.ServiceProvider.GetRequiredService<DimDataSourceLoader>();
            var dimProductLoader = scope.ServiceProvider.GetRequiredService<DimProductLoader>();
            var dimCustomerLoader = scope.ServiceProvider.GetRequiredService<DimCustomerLoader>();
            var dimTimeLoader = scope.ServiceProvider.GetRequiredService<DimTimeLoader>();

            try
            {
                // CARGAR DimDataSource (Fuentes de Datos)
                _logger.LogInformation("--- PASO 1: Cargando DimDataSource ---");

                await dsLoader.LoadAsync();

                // CARGAR DimProduct
                _logger.LogInformation("--- PASO 2: Procesando DimCustomer ---");

                await dimCustomerLoader.LoadAsync();

                // EXTRAER Y CARGAR DimCustomer
                _logger.LogInformation("--- PASO 3: Procesando DimProduct ---");

                await dimProductLoader.LoadAsync();

                // EXTRAER Y CARGAR DimTime
                _logger.LogInformation("--- PASO 4: Procesando DimTime ---");

                await dimTimeLoader.LoadAsync();

                // RESUMEN FINAL
                _logger.LogInformation("========================================");
                _logger.LogInformation("=== ETL FINALIZADO EXITOSAMENTE ===");
                //_logger.LogInformation("Dimensiones cargadas:");
                //_logger.LogInformation("  - DimDataSource: {Count}", dataSources.Count);
                //_logger.LogInformation("  - DimProduct: {Count}", uniqueProducts.Count);
                //_logger.LogInformation("  - DimCustomer: {Count}", uniqueCustomers.Length);
                //_logger.LogInformation("  - DimTime: {Count}", dimTimes.Length);
                _logger.LogInformation("========================================");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR CRÍTICO durante la ejecución del ETL");
                throw;
            }
        }
    }
}
