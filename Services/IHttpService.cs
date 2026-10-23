
namespace Spike.Services;

public interface IHttpService
{
    public Task<string> GetLatestVersion(string publicationId);

    public Task<string> GetDataSet(string dataSetId);
}