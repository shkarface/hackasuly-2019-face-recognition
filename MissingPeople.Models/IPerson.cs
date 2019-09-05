using System;

namespace MissingPeople.Models
{
    public interface IPerson
    {
        string ID { get; set; }
        string Name { get; set; }
        string ImageURL { get; set; }
    }
}
