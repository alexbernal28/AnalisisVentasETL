using Microsoft.Extensions.Logging;

namespace AnalisisVentasETL.Persistence.Destination.Repositories
{
    public class DataLoader<T> : IDataLoader<T> where T : class
    {
        private readonly DwhDBContext _context;
        private readonly ILogger<DataLoader<T>> _logger;
        public DataLoader(DwhDBContext context, ILogger<DataLoader<T>> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task LoadAsync(IEnumerable<T> data)
        {
            try
            {
                _logger.LogInformation("Cargando {Count} registros al Data Warehouse ({EntityName})...",
                    data is ICollection<T> collection ? collection.Count : 0,
                    typeof(T).Name);

                _context.Set<T>().AddRange(data);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Carga completada exitosamente en {EntityName}.", typeof(T).Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar datos en el DWH para {EntityName}.", typeof(T).Name);
                throw;
            }
        }
    }
}
