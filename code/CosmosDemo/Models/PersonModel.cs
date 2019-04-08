using System;
using CosmosDemo.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CosmosDemo.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class PersonModel
    {
        public string Prefix { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string Suffix { get; set; }

        public string Title { get; set; }

        public Person ToData(string jurisdiction)
        {
            return new Person
            {
                Id = Guid.NewGuid().ToString(),
                PartitionKey = jurisdiction,
                Name = new PersonName
                {
                    Prefix = Prefix,
                    FirstName = FirstName,
                    MiddleName = MiddleName,
                    LastName = LastName,
                    Suffix = Suffix,
                    Title = Title
                }
            };
        }

        public Person Map(Person existing)
        {
            if (existing.Name == null) existing.Name = new PersonName();
            existing.Name.Prefix = Prefix;
            existing.Name.FirstName = FirstName;
            existing.Name.MiddleName = MiddleName;
            existing.Name.LastName = LastName;
            existing.Name.Suffix = Suffix;
            existing.Name.Title = Title;
            return existing;
        }
    }
}