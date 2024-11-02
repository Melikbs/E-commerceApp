using Core.Entities;
using Core.Statements;

namespace Core.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<T> GetByIdAsync(int id);
        Task<IReadOnlyList<T>> ListAllAsync();
        Task<T> GetEntityWithState(IStatements<T> state);
        Task<IReadOnlyList<T>> ListAsync(IStatements<T> state);

    }
}
