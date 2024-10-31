using System.Linq.Expressions;

namespace Core.Statements
{
    public class BaseStatement<T> : IStatements<T>

    {
        public BaseStatement()
        {

        }
        public BaseStatement(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;

        }

        public Expression<Func<T, bool>> Criteria { get; }

        public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();
        protected void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }
    }
}
