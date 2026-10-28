using Spike.Data;

namespace Spike.Services;

public interface IStorageService
{
    public Task UploadPublication(PublicationDto publication);

    public Task UploadDataSet(string dataSet);

    public Task<bool> IsVersionMatch(string publishedVersion);
}