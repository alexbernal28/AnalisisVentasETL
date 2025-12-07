using AnalisisVentasETL.Domain.Entities.Dwh.Dimensions;
using AnalisisVentasETL.Domain.Repository;

namespace AnalisisVentasETL.Application.Repositories.Dwh
{
    public interface IDimCustomerRepository : IBaseDwhRepository<DimCustomer>
    {
        Task BulkInsertAsync(IEnumerable<DimCustomer> products);
    }
}
