using AnalisisVentasETL.Application.Repositories.Dwh;
using AnalisisVentasETL.Domain.Entities.Dwh.Dimensions;

namespace AnalisisVentasETL.WRKVentas.Loaders
{
    public class DimDataSourceLoader
    {
        private readonly IDimDataSource _dimDataSourceRepository;
        private readonly ILogger<DimDataSourceLoader> _logger;

        public DimDataSourceLoader(IDimDataSource dimDataSourceRepository, ILogger<DimDataSourceLoader> logger)
        {
            _dimDataSourceRepository = dimDataSourceRepository;
            _logger = logger;
        }

        public async Task LoadAsync()
        {
            _logger.LogInformation("Iniciando carga de DimDataSource...");

            // Eliminar datos existentes
            await _dimDataSourceRepository.DeleteAll();
            _logger.LogInformation("Datos existentes eliminados de DimDataSource.");

            // Crear nuevas entradas
            var dataSources = new[]
            {
                new DimDataSource {
                    SourceType = "CSV",
                    Description = "Archivos CSV de productos, clientes y ventas",
                    UploadDate = DateTime.UtcNow
                },
                new DimDataSource {
                    SourceType = "API",
                    Description = "API REST externa de productos y clientes",
                    UploadDate = DateTime.UtcNow
                },
                new DimDataSource {
                    SourceType = "Database",
                    Description = "Base de datos transaccional SQL Server (SALES)",
                    UploadDate = DateTime.UtcNow
                }
            };

            // Guardar nuevas entradas
            await _dimDataSourceRepository.SaveAll(dataSources);
            _logger.LogInformation("DimDataSource cargado correctamente ({Count} filas).", dataSources.Length);
        }
    }
}
