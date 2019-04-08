using CosmosDemo.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CosmosDemo.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class PersonResponseModel : PersonModel
    {
        public string Id { get; set; }

        public string Jurisdiction { get; set; }

        public static PersonModel FromData(Person data)
        {
            return new PersonResponseModel
            {
                Id = data.Id,
                Jurisdiction = data.PartitionKey,
                Prefix = data.Name.Prefix,
                FirstName = data.Name.FirstName,
                MiddleName = data.Name.MiddleName,
                LastName = data.Name.LastName,
                Suffix = data.Name.Suffix,
                Title = data.Name.Title
            };
        }
    }
}