using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Spike.Services;
using Spike.Data;

namespace Spike.PublicationPull;

public class PublicationPull
{
    private readonly ILogger _logger;
    private readonly IStorageService _storage;
    private readonly IHttpService _http;

    public PublicationPull(ILoggerFactory loggerFactory, IStorageService storage, IHttpService httpClient)
    {
        _logger = loggerFactory.CreateLogger<PublicationPull>();
        _storage = storage;
        _http = httpClient;
    }

    [Function("PublicationPull")]
    public async Task Run([TimerTrigger("%TIMER_TRIGGER%")] TimerInfo myTimer)
    {
        _logger.LogInformation("C# Timer trigger function executed at: {executionTime}", DateTime.Now);

        try
        {
            PublicationsResponse publishedVersion = await _http.GetLatestVersion("1");
            bool isCurrent = await _storage.IsVersionMatch(publishedVersion.Results[0].LatestVersion.Version);

            if (isCurrent)
            {
                _logger.LogInformation("Published version matched the records in storage - {version}. Ending process.", publishedVersion.Results[0].LatestVersion.Version);
                return;
            }
            else
            {
                _logger.LogInformation("Processing new published version, {version}", publishedVersion.Results[0].LatestVersion.Version);
                await _storage.UploadPublication(publishedVersion.Results[0]);
                
                string returnedDataSet = await _http.GetDataSet("1");
                await _storage.UploadDataSet(returnedDataSet);
            }

        }
        catch (Exception e)
        {
            _logger.LogError("Error processing new Publication: {errorMessage}", e.Message);
        } 
        
        if (myTimer.ScheduleStatus is not null)
        {
            _logger.LogInformation("Next timer schedule at: {nextSchedule}", myTimer.ScheduleStatus.Next);
        }
    }
}
