using MmgExplorer.Models;

namespace MmgExplorer;

public interface IMmgatClient
{
    Task<ApiSingleResponse> GetAsync(string id);
    Task<ApiMultipleResponse> GetAllAsync();

}
