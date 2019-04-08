using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace CosmosDemo.Functions
{
    public static class PeopleObserver
    {
        [FunctionName("Watch_People")]
        public static async Task Watch(
            [CosmosDBTrigger(
                Databases.MyApp.Name,
                Databases.MyApp.Collections.People.Name,
                ConnectionStringSetting = Databases.MyApp.ConnectionStringPropertyName,
                LeaseCollectionName = Databases.MyApp.Collections.People.LeaseName,
                CreateLeaseCollectionIfNotExists = true)]
            IReadOnlyList<Document> input,
            [Blob(
                "archive/people",
                FileAccess.Write,
                Connection = "Archive_ConnectionString")]
            CloudBlobDirectory output,
            ILogger log,
            CancellationToken cancellationToken)
        {
            if (input != null && input.Count > 0)
            {
                log.LogInformation("Documents modified " + input.Count);
                foreach (var doc in input)
                {
                    var partitionKey = doc.GetPropertyValue<string>("partitionKey");
                    var id = doc.Id;
                    var timestamp = doc.Timestamp.ToString("yyyyMMdd-HHmmss");
                    var blobName = $"{partitionKey}/{id}/{timestamp}.json";

                    var blob = output.GetBlockBlobReference(blobName);
                    await blob.UploadTextAsync(
                        JsonConvert.SerializeObject(doc),
                        Encoding.UTF8,
                        AccessCondition.GenerateIfNotExistsCondition(),
                        new BlobRequestOptions(),
                        new OperationContext(),
                        cancellationToken);
                    log.LogInformation($"Created blob {blob.Name}");
                }
            }
        }
    }
}