using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hackasuly2019_face_recognition.Services
{
    public class FoundPersonService
    {
        public IMongoCollection<BsonDocument> _FoundPeople;
        public FoundPersonService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _FoundPeople = database.GetCollection<BsonDocument>(settings.FoundCollectionName);
        }
    }
}
