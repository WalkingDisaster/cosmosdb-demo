using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using CosmosDemo.Data;
using CosmosDemo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace CosmosDemo.Functions
{
    public static class PersonCrud
    {
        [FunctionName("Create_Person")]
        public static async Task<IActionResult> Create(
            [HttpTrigger(
                AuthorizationLevel.Function,
                "put",
                Route = "jurisdiction/{jurisdiction}/person")]
            HttpRequest req,
            string jurisdiction,
            [CosmosDB(
                Databases.MyApp.Name,
                Databases.MyApp.Collections.People.Name,
                ConnectionStringSetting = Databases.MyApp.ConnectionStringPropertyName,
                Id = Databases.MyApp.Collections.People.Id,
                PartitionKey = Databases.MyApp.Collections.People.PartitionKey,
                CreateIfNotExists = true)]
            IAsyncCollector<Person> peopleCollection,
            ILogger log,
            CancellationToken cancellationToken)
        {
            var requestModel = await req.Body.ToObjectAsync<PersonModel>(cancellationToken);
            return await UpsertPerson(
                req.Path.ToString(),
                requestModel,
                null,
                jurisdiction,
                peopleCollection,
                log,
                cancellationToken);
        }

        [FunctionName("Update_Person")]
        public static async Task<IActionResult> Update(
            [HttpTrigger(
                AuthorizationLevel.Function,
                "put",
                Route = "jurisdiction/{jurisdiction}/person/{id}")]
            HttpRequest req,
            string jurisdiction,
            string id,
            [CosmosDB(
                Databases.MyApp.Name,
                Databases.MyApp.Collections.People.Name,
                ConnectionStringSetting = Databases.MyApp.ConnectionStringPropertyName,
                Id = "{id}",
                PartitionKey = "{jurisdiction}",
                CreateIfNotExists = true)]
            Person existing,
            [CosmosDB(
                Databases.MyApp.Name,
                Databases.MyApp.Collections.People.Name,
                ConnectionStringSetting = Databases.MyApp.ConnectionStringPropertyName,
                Id = Databases.MyApp.Collections.People.Id,
                PartitionKey = Databases.MyApp.Collections.People.PartitionKey,
                CreateIfNotExists = true)]
            IAsyncCollector<Person> peopleCollection,
            ILogger log,
            CancellationToken cancellationToken)
        {
            if (existing == null)
                return new BadRequestErrorMessageResult(
                    $"Person with jurisdiction \"{jurisdiction}\" and ID \"{id}\" not found. Cannot update.");
            var requestModel = await req.Body.ToObjectAsync<PersonModel>(cancellationToken);
            return await UpsertPerson(
                req.Path.ToString(),
                requestModel,
                existing,
                jurisdiction,
                peopleCollection,
                log,
                cancellationToken);
        }

        [FunctionName("Get_Person")]
        public static IActionResult Read(
            [HttpTrigger(
                AuthorizationLevel.Function,
                "get",
                Route = "jurisdiction/{jurisdiction}/person/{id}")]
            HttpRequest req,
            [CosmosDB(
                Databases.MyApp.Name,
                Databases.MyApp.Collections.People.Name,
                ConnectionStringSetting = Databases.MyApp.ConnectionStringPropertyName,
                Id = "{id}",
                PartitionKey = "{jurisdiction}",
                CreateIfNotExists = true)]
            Person person)
        {
            return new OkObjectResult(person);
        }

        [FunctionName("Delete_Person")]
        public static async Task<IActionResult> Delete(
            [HttpTrigger(
                AuthorizationLevel.Function,
                "delete",
                Route = "jurisdiction/{jurisdiction}/person/{id}")]
            HttpRequest req,
            string jurisdiction,
            string id,
            [CosmosDB(
                Databases.MyApp.Name,
                Databases.MyApp.Collections.People.Name,
                ConnectionStringSetting = Databases.MyApp.ConnectionStringPropertyName,
                Id = "{id}",
                PartitionKey = "{jurisdiction}",
                CreateIfNotExists = true)]
            Person toDelete,
            [CosmosDB(ConnectionStringSetting = Databases.MyApp.ConnectionStringPropertyName)]
            IDocumentClient client,
            ILogger log,
            CancellationToken cancellationToken)
        {
            if (toDelete == null)
                return new BadRequestErrorMessageResult(
                    $"Person with jurisdiction \"{jurisdiction}\" and ID \"{id}\" not found. Cannot Delete.");

            await client.DeleteDocumentAsync(toDelete.SelfLink, new RequestOptions
            {
                PartitionKey = new PartitionKey(jurisdiction)
            }, cancellationToken);
            return new AcceptedResult();
        }

        private static async Task<IActionResult> UpsertPerson(
            string path,
            PersonModel model,
            Person existing,
            string jurisdiction,
            IAsyncCollector<Person> peopleCollection,
            ILogger log,
            CancellationToken cancellationToken)
        {
            Person record;
            IActionResult response;
            if (existing == null)
            {
                log.LogInformation("Creating new person record.");
                record = model.ToData(jurisdiction);
                await peopleCollection.AddAsync(record, cancellationToken);
                response = new CreatedResult(path, new CreatePersonResponseModel
                {
                    Id = record.Id
                });
            }
            else
            {
                log.LogInformation("Updating existing person record.");
                record = model.Map(existing);
                await peopleCollection.AddAsync(record, cancellationToken);
                response = new AcceptedResult();
            }

            return response;
        }
    }
}