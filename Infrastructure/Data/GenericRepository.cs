using Core.Entities;
using Core.Interfaces;
using Core.Statements;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Data
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly StoreContext _context;
        public GenericRepository(StoreContext context)
        {
            _context = context;
        }



        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }



        public async Task<IReadOnlyList<T>> ListAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }
        public async Task<T> GetEntityWithState(IStatements<T> state)
        {
            return await ApplyStatement(state).FirstOrDefaultAsync();
        }
        public async Task<IReadOnlyList<T>> ListAsync(IStatements<T> state)
        {
            return await ApplyStatement(state).ToListAsync();
        }


        private IQueryable<T> ApplyStatement(IStatements<T> state)
        {
            return StatementEvaluator<T>.GetQuery(_context.Set<T>().AsQueryable(), state);
        }
    }
}
