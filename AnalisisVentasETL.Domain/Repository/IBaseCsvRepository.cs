namespace AnalisisVentasETL.Domain.Repository
{
    public interface IBaseCsvRepository<T>
    {
        Task<List<T>> GetAll();
    }
}
