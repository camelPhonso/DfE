using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Spike.Data;

namespace Spike.Services;

public class DfEService : IHttpService
{
    private readonly HttpClient _client;
    private readonly ILogger _logger;

    public DfEService(IHttpClientFactory factory, ILoggerFactory loggerFactory)
    {
        _client = factory.CreateClient("DfE");
        _logger = loggerFactory.CreateLogger<DfEService>();
    }

    public async Task<PublicationsResponse> GetLatestVersion(string publicationId)
    {
        try
        {
            var apiResponse = await _client.GetAsync($"/publications/{publicationId}/data-set");
            PublicationsResponse? publicationResponse = await apiResponse.Content.ReadFromJsonAsync<PublicationsResponse>();

            if ( publicationResponse is null)
            {
                throw new Exception("Invalid Publication data returned.");
            }

            return publicationResponse;
        }
        catch
        {
            throw;
        }
    }

    public async Task<string> GetDataSet(string dataSetId)
    {
        try
        {
            var apiResponse = await _client.GetAsync($"/data-sets/{dataSetId}/meta");
            var jsonResponse = await apiResponse.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(jsonResponse))
            {
                throw new Exception("Invalid Data Set returned.");
            }

            return jsonResponse;
        }
        catch
        {
            throw;
        }
    }
}