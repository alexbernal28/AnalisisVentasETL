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

        public Task<bool> Exists(Expression<Func<DimProduct, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public Task<List<DimProduct>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task RemoveAll(DimProduct[] entities)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAll()
        {
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM [Dimension].[DimProduct]");
        }

        public Task SaveAll(DimProduct[] entities)
        {
            throw new NotImplementedException();
        }
    }
}
