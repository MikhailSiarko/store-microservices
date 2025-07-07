namespace Store.Compose.Aspire;

public static class CosmosDb
{
    public static IResourceBuilder<AzureCosmosDBResource> AddCosmosDb(this IDistributedApplicationBuilder builder)
    {
        return builder
            .AddAzureCosmosDB("CosmosDb")
            .RunAsEmulator(x =>
                {
                    x.WithContainerName("store.cosmos.db.server")
                        .WithEnvironment("AZURE_COSMOS_EMULATOR_IP_ADDRESS_OVERRIDE", "127.0.0.1")
                        .WithContainerRuntimeArgs("-p", "8081:8081", "-p", "10250-10255:10250-10255")
                        .WithOtlpExporter();
                }
            );
    }
}