using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MissingPeople.Models
{
    public class Person : IPerson
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ID { get; set; }
        public string Name { get; set; }
        public string ImageURL { get; set; }
    }
}
