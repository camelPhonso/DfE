
using Spike.Data;

namespace Spike.Services;

public interface IHttpService
{
    public Task<PublicationsResponse> GetLatestVersion(string publicationId);

    public Task<string> GetDataSet(string dataSetId);
}