using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnalisisVentasETL.Application.Repositories.DB
{
    public interface IDatabaseExtractor
    {
        Task<List<T>> GetAllAsync<T>() where T : class;
    }
}
