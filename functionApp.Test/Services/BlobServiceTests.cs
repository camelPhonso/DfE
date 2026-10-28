using Moq;
using Spike.Services;
using Azure.Storage.Blobs;
using Azure;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging.Abstractions;
using System.Text.Json.Serialization;
using System.Text.Json;
using Spike.Data;

namespace functionApp.Test;

public class BlobServiceTests
{
    private readonly Mock<BlobServiceClient> _mockClient;

    public BlobServiceTests()
    {
        _mockClient = new Mock<BlobServiceClient>();
    }

    [Fact]
    public async Task IsVersionMatch_GivenAVersionMatchingTheCurrentlyStored_ReturnsTrue()
    {
        // Arrange
        var testPublication = new PublicationsResponse(new List<PublicationDto>
        {
            new PublicationDto("test", "Completely Fake", new VersionDto("v1.3", 2))
        });
        var blobData = BinaryData.FromString(JsonSerializer.Serialize(testPublication));
        var downloadResult = BlobsModelFactory.BlobDownloadResult(content: blobData);
        var response = Response.FromValue(downloadResult, Mock.Of<Response>());

        var mockBlobClient = new Mock<BlobClient>();
        mockBlobClient.Setup(x => x.DownloadContentAsync()).ReturnsAsync(response);

        var mockContainerClient = new Mock<BlobContainerClient>();
        mockContainerClient.Setup(x => x.GetBlobClient(It.IsAny<string>())).Returns(mockBlobClient.Object);

        _mockClient.Setup(x => x.GetBlobContainerClient(It.IsAny<string>())).Returns(mockContainerClient.Object);

        var service = new BlobService(_mockClient.Object, NullLoggerFactory.Instance);

        // Act
        bool returned = await service.IsVersionMatch(testPublication.Results[0].LatestVersion.Version);

        // Assert
        mockBlobClient.Verify(c => c.DownloadContentAsync(), Times.Once());
        Assert.True(returned);
    }

    [Fact]
    public async Task IsVersionMatch_WhenStorageIsEmpty_ReturnsFalse()
    {
        // Arrange
        var blobData = BinaryData.FromString(""); // binary data stream with length zero to simulate an empty blob
        var downloadResult = BlobsModelFactory.BlobDownloadResult(content: blobData);
        var response = Response.FromValue(downloadResult, Mock.Of<Response>());

        var mockBlobClient = new Mock<BlobClient>();
        mockBlobClient.Setup(b => b.DownloadContentAsync()).ReturnsAsync(response);

        var mockContainerClient = new Mock<BlobContainerClient>();
        mockContainerClient.Setup(c => c.GetBlobClient(It.IsAny<string>())).Returns(mockBlobClient.Object);

        _mockClient.Setup(m => m.GetBlobContainerClient(It.IsAny<string>())).Returns(mockContainerClient.Object);

        var service = new BlobService(_mockClient.Object, NullLoggerFactory.Instance);
    
        // Act
        var returned = await service.IsVersionMatch("any string, no matter what");
    
        // Assert
        mockBlobClient.Verify(c => c.DownloadContentAsync(), Times.Once());
        Assert.False(returned);
    }
}