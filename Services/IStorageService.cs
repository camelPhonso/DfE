namespace Spike.Services;

public interface IStorageService
{
    public Task UploadPublication(string publication);

    public Task UploadDataSet(string dataSet);

    public Task<bool> IsVersionMatch(string publishedVersion);
}