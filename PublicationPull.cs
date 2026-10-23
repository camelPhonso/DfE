using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Spike.Services;

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
            var publishedVersion = await _http.GetLatestVersion("1");
            var isCurrent = _storage.IsVersionMatch(publishedVersion);
            
            if (isCurrent.Result)
            {
                _logger.LogInformation("Currently published version matched the data already stored: {version}", publishedVersion);
                return;
            }

            await _storage.UploadPublication(publishedVersion);

            var returnedDataSet = await _http.GetDataSet("1");
            await _storage.UploadDataSet(returnedDataSet);
        }
        catch (Exception e)
        {
            _logger.LogError("Error uploading to storage: {errorMessage}", e.Message);
        } 
        
        if (myTimer.ScheduleStatus is not null)
        {
            _logger.LogInformation("Next timer schedule at: {nextSchedule}", myTimer.ScheduleStatus.Next);
        }
    }
}
