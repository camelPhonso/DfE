using System.Text.Json;

namespace Spike.Services;

public class DfEService : IHttpService
{
    private readonly HttpClient _client;

    public DfEService(IHttpClientFactory factory)
    {
        _client = factory.CreateClient("DfE");    
    }

    public async Task<string> GetLatestVersion(string publicationId)
    {
        try
        {
            var apiResponse = await _client.GetAsync($"/publications/{publicationId}/data-set");
            var jsonResponse = await apiResponse.Content.ReadAsStringAsync();

            var jsonBody = JsonDocument.Parse(jsonResponse);
            
            return jsonBody.RootElement
                .GetProperty("results")[0]
                .GetProperty("latestVersion")
                .GetProperty("version")
                .GetString() ?? string.Empty;
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

            return jsonResponse;
        }
        catch
        {
            throw;
        }
    }
}