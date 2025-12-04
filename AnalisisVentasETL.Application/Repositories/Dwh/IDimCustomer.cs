using AnalisisVentasETL.Domain.Entities.Dwh.Dimensions;
using AnalisisVentasETL.Domain.Repository;

namespace AnalisisVentasETL.Application.Repositories.Dwh
{
    public interface IDimCustomer : IBaseDwhRepository<DimCustomer>
    {
        Task BulkInsertAsync(IEnumerable<DimCustomer> products);
    }
}
