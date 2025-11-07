using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnalisisVentasETL.Persistence.Sources.API.Repositories
{
    public interface IAPIExtractor
    {
        Task<List<T>> GetDataAsync<T>(string endpoint);
    }
}
