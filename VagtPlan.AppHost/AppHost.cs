var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.ApiService>("apiservice")
    .WithHttpHealthCheck("/health")
    .WithUrlForEndpoint("https", url => { url.Url = "/scalar"; })
    .WithUrlForEndpoint("http", url => { url.Url = "/scalar"; });

builder.AddProject<Projects.VagtPlan_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
