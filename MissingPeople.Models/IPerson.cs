using System;

namespace MissingPeople.Models
{
    public enum Gender
    {
        Male,
        Female
    }
    public interface IPerson
    {
        string ID { get; set; }
        string Name { get; set; }
        string ImageURL { get; set; }
        string ContactPhone { get; set; }
        Gender Gender { get; set; }
        SimilarPerson[] SimilarPeople { get; set; }
    }

    public class SimilarPerson
    {
        public float Similarity { get; set; }
        public string ImageURL { get; set; }
        public string ContactPhone { get; set; }
    }
}
