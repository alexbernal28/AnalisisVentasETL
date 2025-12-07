using AnalisisVentasETL.Domain.Entities.Dwh.Facts;
using AnalisisVentasETL.Domain.Repository;

namespace AnalisisVentasETL.Application.Repositories.Dwh
{
    public interface IFactSalesRepository : IBaseDwhRepository<FactSales>
    {
        Task BulkInsertAsync(IEnumerable<FactSales> entities);
    }
}
