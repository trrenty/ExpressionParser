using ExpressionParser.Data;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace ExpressionParser.Repo
{
    public class ExpressionRepo : IExpressionRepo
    {
        protected IMongoCollection<Expression> _collection;

        public ExpressionRepo(IExpressionsDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _collection = database.GetCollection<Expression>(settings.ExpressionsCollectionName);
        }
        public async Task<Expression> AddExpression(Expression expression)
        {
            {
                if (expression == null)
                {
                    throw new ArgumentNullException(typeof(Expression).Name + " object is null");
                }
                await _collection.InsertOneAsync(expression);
                return expression;
            }
        }

        public async Task<Expression> GetExpression(string expression)
        {

            var found = await _collection
                            .Find(e => e.Expresie == expression)
                            .FirstOrDefaultAsync();
            return found;
        }
    }
}
