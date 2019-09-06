using System;
using System.Collections.Generic;

namespace MissingPeople.Models
{
    public enum Gender
    {
        Male,
        Female
    }
    public enum PersonType
    {
        Lost,
        Found
    }
    public interface IPerson
    {
        string ID { get; set; }
        string Name { get; set; }
        string ImageURL { get; set; }
        string ContactPhone { get; set; }
        Gender Gender { get; set; }
        DateTime CreatedAt { get; set; }
        List<SimilarPerson> SimilarPeople { get; set; }
    }

    public class SimilarPerson
    {
        public float Similarity { get; set; }
        public string Name { get; set; }
        public string ImageURL { get; set; }
        public string ContactPhone { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
