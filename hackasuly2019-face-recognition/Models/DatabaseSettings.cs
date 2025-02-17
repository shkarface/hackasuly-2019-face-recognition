﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MissingPeople
{

    public class DatabaseSettings : IDatabaseSettings
    {
        public string LostCollectionName { get; set; }
        public string FoundCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string FlaskAPI { get; set; }
    }

    public interface IDatabaseSettings
    {
        string LostCollectionName { get; set; }
        string FoundCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
        string FlaskAPI { get; set; }
    }
}
