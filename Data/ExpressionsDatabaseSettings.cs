using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressionParser.Data
{
        public class ExpressionsDatabaseSettings : IExpressionsDatabaseSettings
    {
            public string ExpressionsCollectionName { get; set; }
            public string ConnectionString { get; set; }
            public string DatabaseName { get; set; }
        }

        public interface IExpressionsDatabaseSettings
    {
            string ExpressionsCollectionName { get; set; }
            string ConnectionString { get; set; }
            string DatabaseName { get; set; }
        }
    
}
