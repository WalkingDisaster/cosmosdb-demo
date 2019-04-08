using CosmosDemo.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CosmosDemo.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class CreatePersonResponseModel
    {
        public string Id { get; set; }

        public static CreatePersonResponseModel FromData(Person person)
        {
            return new CreatePersonResponseModel
            {
                Id = person.Id
            };
        }
    }
}