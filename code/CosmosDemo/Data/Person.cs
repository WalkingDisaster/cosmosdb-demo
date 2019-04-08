using Microsoft.Azure.Documents;
using Newtonsoft.Json;

namespace CosmosDemo.Data
{
    public class Person : Document
    {
        [JsonProperty("partitionKey")] public string PartitionKey { get; set; }

        [JsonProperty("name")] public PersonName Name { get; set; }
    }

    public class PersonName
    {
        [JsonProperty("prefix", NullValueHandling = NullValueHandling.Ignore)]
        public string Prefix { get; set; }

        [JsonProperty("first")] public string FirstName { get; set; }

        [JsonProperty("middle", NullValueHandling = NullValueHandling.Ignore)]
        public string MiddleName { get; set; }

        [JsonProperty("last")] public string LastName { get; set; }

        [JsonProperty("suffix", NullValueHandling = NullValueHandling.Ignore)]
        public string Suffix { get; set; }

        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }
    }
}