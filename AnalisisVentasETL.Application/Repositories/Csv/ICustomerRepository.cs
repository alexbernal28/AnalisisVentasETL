using AnalisisVentasETL.Domain.Entities.Csv;
using AnalisisVentasETL.Domain.Repository;

namespace AnalisisVentasETL.Application.Repositories.Csv
{
    public interface ICustomerRepository : IBaseCsvRepository<Customer>
    {
    }
}
