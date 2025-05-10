var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.application_ApiService>("apiservice");

builder.AddProject<Projects.application_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
