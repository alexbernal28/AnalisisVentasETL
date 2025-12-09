using AnalisisVentasETL.Application.Repositories.Dwh;
using AnalisisVentasETL.Domain.Entities.Dwh.Dimensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AnalisisVentasETL.Persistence.Destination.Repositories
{
    public class DimCustomerRepository : IDimCustomerRepository
    {
        private readonly DwhDBContext _context;

        public DimCustomerRepository(DwhDBContext context)
        {
            _context = context;
        }

        public async Task BulkInsertAsync(IEnumerable<DimCustomer> customers)
        {
            _context.DimCustomers.RemoveRange(_context.DimCustomers);

            await _context.DimCustomers.AddRangeAsync(customers);

            await _context.SaveChangesAsync();
        }

        public async Task SaveAll(DimCustomer[] dimCustomers)
        {
            await _context.DimCustomers.AddRangeAsync(dimCustomers);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAll(DimCustomer[] dimCustomers)
        {
            _context.DimCustomers.RemoveRange(dimCustomers);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAll()
        {
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM [Dimension].[DimCustomer]");
        }

        public async Task<List<DimCustomer>> GetAll()
        {
            return await _context.DimCustomers.ToListAsync();
        }

        public async Task<bool> Exists(Expression<Func<DimCustomer, bool>> filter)
        {
            return await _context.DimCustomers.AnyAsync(filter);
        }
    }
}
