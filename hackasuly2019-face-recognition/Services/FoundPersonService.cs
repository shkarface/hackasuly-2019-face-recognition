using MissingPeople.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MissingPeople.Services
{
    public class FoundPersonService
    {
        public IMongoCollection<Person> _FoundPeople;
        public FoundPersonService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _FoundPeople = database.GetCollection<Person>(settings.FoundCollectionName);
        }

        public void Create(Person person) => _FoundPeople.InsertOne(person);
        public IPerson Get(string id) => _FoundPeople.Find(doc => doc.ID == id).First();
    }
}
