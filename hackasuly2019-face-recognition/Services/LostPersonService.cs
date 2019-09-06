using MissingPeople.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MissingPeople.Services
{
    public class LostPersonService
    {
        public IMongoCollection<Person> _LostPeople;
        public LostPersonService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _LostPeople = database.GetCollection<Person>(settings.LostCollectionName);
        }


        public void Create(Person person) => _LostPeople.InsertOne(person);
        public IPerson AddNewSimilarPerson(string id, SimilarPerson similarPerson)
        {
            var filter = Builders<Person>.Filter.Eq("_id", id);
            var update = Builders<Person>.Update.Push(doc => doc.SimilarPeople, similarPerson);
            return _LostPeople.FindOneAndUpdate(filter, update);
        }
        public Person Get(string id) => _LostPeople.Find(doc => doc.ID == id).FirstOrDefault();
    }
}
