using System.Linq.Expressions;

namespace Core.Statements
{
    public interface IStatements<T>
    {
        Expression<Func<T, bool>> Criteria { get; }
        List<Expression<Func<T, object>>> Includes { get; }
    }
}
