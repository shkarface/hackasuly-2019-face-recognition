using MissingPeople.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MissingPeople.Services
{
    public class PersonService
    {
        public IMongoCollection<Person> _LostPeople;
        public IMongoCollection<Person> _FoundPeople;

        public PersonService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _LostPeople = database.GetCollection<Person>(settings.LostCollectionName);
            _FoundPeople = database.GetCollection<Person>(settings.FoundCollectionName);
        }


        public void Create(Person person, PersonType personType)
        {
            person.ID = Guid.NewGuid().ToString();
            person.SimilarPeople = new List<SimilarPerson>();
            person.CreatedAt = DateTime.Now;
            (personType == PersonType.Lost ? _LostPeople : _FoundPeople).InsertOne(person);
        }
        public IPerson AddNewSimilarPerson(string id, SimilarPerson similarPerson, PersonType personType)
        {
            var filter = Builders<Person>.Filter.Eq("_id", id);
            var update = Builders<Person>.Update.Push(doc => doc.SimilarPeople, similarPerson);
            return (personType == PersonType.Lost ? _LostPeople : _FoundPeople).FindOneAndUpdate(filter, update);
        }
        public Person Get(string id, PersonType personType) => (personType == PersonType.Lost ? _LostPeople : _FoundPeople).Find(doc => doc.ID == id).FirstOrDefault();
    }
}
