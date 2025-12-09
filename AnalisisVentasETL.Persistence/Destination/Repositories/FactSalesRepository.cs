using AnalisisVentasETL.Application.Repositories.Dwh;
using AnalisisVentasETL.Domain.Entities.Dwh.Facts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace AnalisisVentasETL.Persistence.Destination.Repositories
{
    public class FactSalesRepository : IFactSalesRepository
    {
        private readonly DwhDBContext _context;
        private readonly ILogger<FactSalesRepository> _logger;

        public FactSalesRepository(DwhDBContext context, ILogger<FactSalesRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task BulkInsertAsync(IEnumerable<FactSales> entities)
        {
            var salesList = entities.ToList();
            _logger.LogInformation("Iniciando inserción masiva de {Count} registros en FactSales.", salesList.Count);

            // Insertar en lotes para mejorar el rendimiento

            const int batchSize = 1000;

            for (int i = 0; i < salesList.Count; i += batchSize)
            {
                var batch = salesList.Skip(i).Take(batchSize);
                await _context.FactSales.AddRangeAsync(batch);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Insertados {Count} registros en FactSales hasta ahora.", Math.Min(i + batchSize, salesList.Count));
            }
        }

        public async Task SaveAll(FactSales[] entities)
        {
            await _context.FactSales.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAll(FactSales[] entities)
        {
            _context.FactSales.RemoveRange(entities);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAll()
        {
            _logger.LogInformation("Limpiando todos los registros de FactSales.");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM [Fact].[FactSales]");
            _logger.LogInformation("Todos los registros de FactSales han sido eliminados.");
        }

        public async Task<List<FactSales>> GetAll()
        {
            return await _context.FactSales.ToListAsync();
        }

        public async Task<bool> Exists(Expression<Func<FactSales, bool>> filter)
        {
            return await _context.FactSales.AnyAsync(filter);
        }
    }
}
