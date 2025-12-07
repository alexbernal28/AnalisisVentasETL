using AnalisisVentasETL.Application.Repositories.Dwh;
using AnalisisVentasETL.Domain.Entities.Dwh.Dimensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AnalisisVentasETL.Persistence.Destination.Repositories
{
    public class DimTimeRepository : IDimTimeRepository
    {
        private readonly DwhDBContext _context;

        public DimTimeRepository(DwhDBContext context)
        {
            _context = context;
        }

        public async Task SaveAll(DimTime[] dimTimes)
        {
            await _context.DimTimes.AddRangeAsync(dimTimes);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAll(DimTime[] dimTimes)
        {
            _context.DimTimes.RemoveRange(dimTimes);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAll()
        {
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM [Dimension].[DimTime]");
        }

        public async Task<List<DimTime>> GetAll()
        {
            return await _context.DimTimes.ToListAsync();
        }

        public async Task<bool> Exists(Expression<Func<DimTime, bool>> filter)
        {
            return await _context.DimTimes.AnyAsync(filter);
        }
    }
}
