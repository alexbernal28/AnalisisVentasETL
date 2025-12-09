using AnalisisVentasETL.Application.Repositories.Dwh;
using AnalisisVentasETL.Domain.Entities.Dwh.Dimensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AnalisisVentasETL.Persistence.Destination.Repositories
{
    public class DimDataSourceRepository : IDimDataSourceRepository
    {
        private readonly DwhDBContext _context;
        public DimDataSourceRepository(DwhDBContext context)
        {
            _context = context;
        }
        public async Task SaveAll(DimDataSource[] dimDataSources)
        {
            await _context.DimDataSources.AddRangeAsync(dimDataSources);
            await _context.SaveChangesAsync();
        }
        public async Task RemoveAll(DimDataSource[] dimDataSources)
        {
            _context.DimDataSources.RemoveRange(dimDataSources);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAll()
        {
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM [Dimension].[DimDataSource]");
        }

        public async Task<List<DimDataSource>> GetAll()
        {
            return await _context.DimDataSources.ToListAsync();
        }
        public async Task<bool> Exists(Expression<Func<DimDataSource, bool>> filter)
        {
            return await _context.DimDataSources.AnyAsync(filter);
        }
    }
}
