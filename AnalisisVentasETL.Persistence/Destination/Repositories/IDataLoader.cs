namespace AnalisisVentasETL.Persistence.Destination.Repositories
{
    public interface IDataLoader<T>
    {
        Task LoadAsync(IEnumerable<T> data);
    }
}
