using AnalisisVentasETL.Application.Repositories.Dwh;
using AnalisisVentasETL.Domain.Entities.Dwh.Dimensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AnalisisVentasETL.Persistence.Destination.Repositories
{
    public class DimProductRepository : IDimProductRepository
    {
        private readonly DwhDBContext _context;

        public DimProductRepository(DwhDBContext context)
        {
            _context = context;
        }

        public async Task BulkInsertAsync(IEnumerable<DimProduct> products)
        {
            _context.DimProducts.RemoveRange(_context.DimProducts);

            await _context.DimProducts.AddRangeAsync(products);

            await _context.SaveChangesAsync();
        }

        public async Task<bool> Exists(Expression<Func<DimProduct, bool>> filter)
        {
            return await _context.DimProducts.AnyAsync(filter);
        }

        public async Task<List<DimProduct>> GetAll()
        {
            return await _context.DimProducts.ToListAsync();
        }

        public async Task RemoveAll(DimProduct[] entities)
        {
            _context.DimProducts.RemoveRange(entities);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAll()
        {
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM [Dimension].[DimProduct]");
        }

        public async Task SaveAll(DimProduct[] entities)
        {
            await _context.DimProducts.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }
    }
}
