namespace BaraoPsicologia.Domain.Interfaces.Repositories
{
    public interface IRepositoryCore<T> where T : class
    {
        Task<T?> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();

        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);

        Task<bool> ExistsAsync(Guid id);
    }
}
