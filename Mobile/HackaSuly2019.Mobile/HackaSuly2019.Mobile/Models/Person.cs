using MissingPeople.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HackaSuly2019.Mobile.Models
{
    public class Person : IPerson
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string ImageURL { get; set; }
        public string ContactPhone { get; set; }
        public Gender Gender { get; set; }
        public SimilarPerson[] SimilarPeople { get; set; }
    }
}
