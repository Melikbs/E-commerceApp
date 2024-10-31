using Core.Entities;
using Core.Statements;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class StatementEvaluator<TEntity> where TEntity : BaseEntity
    {
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputquery, IStatements<TEntity> state)
        {
            var query = inputquery;
            if (state.Criteria != null)
            {
                query = query.Where(state.Criteria);
            }
            query = state.Includes.Aggregate(query, (current, include) => current.Include(include));
            return query;
        }
    }
}