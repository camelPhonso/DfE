using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Azure;
using Spike.Services;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services.AddHttpClient("DfE", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["DFE_API_BASE_URL"]!);
});

builder.Services.AddAzureClients(clientBuilder =>
{
   clientBuilder.AddBlobServiceClient(builder.Configuration["AzureWebJobsStorage"]); 
});

builder.Services.AddScoped<IStorageService, BlobService>();
builder.Services.AddScoped<IHttpService, DfEService>();

if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING")))
{
    builder.Services.AddOpenTelemetry()
        .UseFunctionsWorkerDefaults()
        .UseAzureMonitorExporter();
}

builder.Build().Run();
