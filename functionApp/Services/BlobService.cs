using System.Text.Json;
using System.Text.RegularExpressions;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using Spike.Data;

namespace Spike.Services;

public class BlobService : IStorageService
{
    private readonly BlobContainerClient _blobContainer;
    private readonly ILogger _logger;

    public BlobService(BlobServiceClient serviceClient, ILoggerFactory factory)
    {
        _blobContainer = serviceClient.GetBlobContainerClient("azure-webjobs-hosts");
        _logger = factory.CreateLogger<BlobService>();
    }

    public async Task UploadPublication(string publication)
    {
        try
        {
            var blob = _blobContainer.GetBlobClient("publications");
            await blob.UploadAsync(BinaryData.FromString(publication), overwrite: true);
        }
        catch
        {
            throw;
        }
    }

    public async Task UploadDataSet(string dataSet)
    {
        try
        {
            var blob = _blobContainer.GetBlobClient("data-sets");
            await blob.UploadAsync(BinaryData.FromString(dataSet), overwrite: true);
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> IsVersionMatch(string publishedVersion)
    {
        var blob = _blobContainer.GetBlobClient("publications");
        var blobContent = await blob.DownloadContentAsync();
        if(blobContent.Value.Content.IsEmpty)
        {
            return false;
        }
        
        PublicationsResponse? storedPublication = JsonSerializer.Deserialize<PublicationsResponse>(blobContent.Value.Content);
        if(storedPublication is null)
        {
            return false;
        }

        bool isMatch = string.Equals(publishedVersion, storedPublication.Results[0].LatestVersion.Version);

        return isMatch;
    }
}