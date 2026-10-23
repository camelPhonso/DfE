using Moq;
using Spike.Services;
using Azure.Storage.Blobs;
using Azure;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging.Abstractions;

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
        var blobData = BinaryData.FromString("1.2");

        var downloadResult = BlobsModelFactory.BlobDownloadResult(
            content: blobData
        );

        var response = Response.FromValue(
            downloadResult,
            Mock.Of<Response>()
        );

        var mockBlobClient = new Mock<BlobClient>();

        mockBlobClient
            .Setup(x => x.DownloadContentAsync())
            .ReturnsAsync(response);

        var mockContainerClient = new Mock<BlobContainerClient>();

        mockContainerClient
            .Setup(x => x.GetBlobClient(It.IsAny<string>()))
            .Returns(mockBlobClient.Object);

        _mockClient
            .Setup(x => x.GetBlobContainerClient(It.IsAny<string>()))
            .Returns(mockContainerClient.Object);

        var service = new BlobService(
            _mockClient.Object,
            NullLoggerFactory.Instance);

        // Act
        bool returned = await service.IsVersionMatch("1.2");

        // Assert
        mockBlobClient.Verify(c => c.DownloadContentAsync(), Times.Once());
        Assert.True(returned);
    }
}