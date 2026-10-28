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

    public async Task UploadPublication(PublicationDto publication)
    {
        try
        {
            var blob = _blobContainer.GetBlobClient("publications");
            await blob.UploadAsync(BinaryData.FromString(JsonSerializer.Serialize(publication)), overwrite: true);
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
        
        PublicationDto? storedPublication = JsonSerializer.Deserialize<PublicationDto>(blobContent.Value.Content.ToString(), new JsonSerializerOptions{ PropertyNameCaseInsensitive = true });
        if(storedPublication is null)
        {
            return false;
        }

        bool isMatch = string.Equals(publishedVersion, storedPublication.LatestVersion.Version);

        return isMatch;
    }
}