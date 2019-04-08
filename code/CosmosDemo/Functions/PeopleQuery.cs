using System.Collections.Generic;
using System.Linq;
using CosmosDemo.Data;
using CosmosDemo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace CosmosDemo.Functions
{
    public static class PeopleQuery
    {
        [FunctionName("Query_People")]
        public static IActionResult Query(
            [HttpTrigger(AuthorizationLevel.Function,
                "get",
                Route = "jurisdiction/{jurisdiction}/people")]
            HttpRequest req,
            [CosmosDB(
                Databases.MyApp.Name,
                Databases.MyApp.Collections.People.Name,
                ConnectionStringSetting = Databases.MyApp.ConnectionStringPropertyName,
                SqlQuery = "SELECT * FROM People c WHERE c.partitionKey = {jurisdiction}",
                CreateIfNotExists = true)]
            IEnumerable<Person> people)
        {
            return new OkObjectResult(people.Select(PersonResponseModel.FromData).ToArray());
        }
    }
}