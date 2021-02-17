using ExpressionParser.Data;
using System.Threading.Tasks;

namespace ExpressionParser.Repo
{
    public interface IExpressionRepo
    {
        Task<Expression> GetExpression(string expression);
        Task<Expression> AddExpression(Expression expression);
    }
}
