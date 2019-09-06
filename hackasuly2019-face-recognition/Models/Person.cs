using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MissingPeople.Models
{
    public class Person : IPerson
    {
        [BsonId]
        public string ID { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [BsonElement("name")]
        public string Name { get; set; }

        [Required]
        [BsonElement("imageURL")]
        public string ImageURL { get; set; }

        [Required]
        [BsonElement("contactPhone")]
        public string ContactPhone { get; set; }

        [Required]
        [BsonElement("gender")]
        public Gender Gender { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("similarPeople")]
        public List<SimilarPerson> SimilarPeople { get; set; } = new List<SimilarPerson>();
    }
}
