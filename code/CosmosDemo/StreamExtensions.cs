using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;

namespace CosmosDemo
{
    public static class StreamExtensions
    {
        private static readonly JsonSerializer Serializer = new JsonSerializer();

        public static Task<T> ToObjectAsync<T>(
            this Stream stream,
            CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                using (var streamReader = new StreamReader(stream))
                using (var reader = new JsonTextReader(streamReader))
                {
                    return Serializer.Deserialize<T>(reader);
                }
            }, cancellationToken);
        }

        public static async Task<T> ConvertAsync<T>(
            this Document source,
            CancellationToken cancellationToken = default)
        {
            using (var ms = new MemoryStream())
            using (var reader = new StreamReader(ms))
            {
                source.SaveTo(ms);
                ms.Position = 0;
                return await ms.ToObjectAsync<T>(cancellationToken);
            }
        }
    }
}