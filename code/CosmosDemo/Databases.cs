namespace CosmosDemo
{
    public static class Databases
    {
        public static class MyApp
        {
            public const string ConnectionStringPropertyName = "CosmosDb_ConnectionString";
            public const string Name = "MyApp";

            public static class Collections
            {
                public static class People
                {
                    public const string Name = "People";
                    public const string LeaseName = "People_Leases";
                    public const string Id = "id";
                    public const string PartitionKey = "partitionKey";
                }
            }
        }
    }
}