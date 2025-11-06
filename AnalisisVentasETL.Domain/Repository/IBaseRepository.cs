using System.Linq.Expressions;

namespace AnalisisVentasETL.Domain.Repository
{
    public interface IBaseRepository<T> where T : class
    {
        Task Save(T entity);

        Task Update(T entity);

        Task Remove(T entity);

        Task<List<T>> GetAll();

        Task<List<T>> GetAll(Expression<Func<T, bool>> filter);

        Task<T> GetById(Guid id);

        Task<bool> Exists(Expression<Func<T, bool>> filter);
    }
}
