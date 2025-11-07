using System.Linq.Expressions;

namespace AnalisisVentasETL.Domain.Repository
{
    public interface IBaseDwhRepository<T> where T : class
    {
        Task SaveAll(T[] entities);

        Task RemoveAll(T[] entities);

        Task<List<T>> GetAll();

        Task<bool> Exists(Expression<Func<T, bool>> filter);
    }
}
