using AnalisisVentasETL.Domain.Entities.Dwh.Facts;
using AnalisisVentasETL.Domain.Repository;

namespace AnalisisVentasETL.Application.Repositories.Dwh
{
    public interface IFactSales : IBaseDwhRepository<FactSales>
    {
    }
}
