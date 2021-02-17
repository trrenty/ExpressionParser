

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExpressionParser.Data
{
    public class Expression
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("expression")]
        public string Expresie { get; set; }
        [BsonElement("result")]
        public string Result { get; set; }
    }
}
