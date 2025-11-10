using AnalisisVentasETL.Domain.Entities.Dwh.Dimensions;
using AnalisisVentasETL.Domain.Repository;

namespace AnalisisVentasETL.Application.Repositories.Dwh
{
    public interface IDimProductRepository : IBaseDwhRepository<DimProduct>
    {
        Task BulkInsertAsync(IEnumerable<DimProduct> products);
    }
}
