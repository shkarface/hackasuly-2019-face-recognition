using MissingPeople.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hackasuly2019_face_recognition.Services
{
    public class LostPersonService
    {
        public IMongoCollection<IPerson> _LostPeople;
        public LostPersonService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _LostPeople = database.GetCollection<IPerson>(settings.LostCollectionName);
        }

        public async Task CreateAsync(IPerson person) => await _LostPeople.InsertOneAsync(person);
        public async Task<IPerson> GetAsync(string id) => await _LostPeople.Find(doc => doc.ID == id).FirstAsync();
    }
}
