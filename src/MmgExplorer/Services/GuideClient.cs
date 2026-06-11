using MmgExplorer.Models;
using MmgExplorer.Services.Interfaces;

namespace MmgExplorer.Services;

public class GuideClient : IGuideClient
{

    private readonly IApiClient apiClient;
    private readonly ILogger<GuideClient> logger;
    // Relative to HttpClient.BaseAddress: no leading slash (a rooted path would
    // discard the base path), and the base address must end with a trailing slash.
    private readonly string urlPrefix = "guide/";

    public GuideClient(IApiClient client, ILogger<GuideClient> logr)
    {
        apiClient = client;
        logger = logr;
    }
    public async Task<ApiResponse<Guide>> GetGuideById(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var url = $"{urlPrefix}{id}";
            var response = await apiClient.GetAsync(url);
            if ((!response.IsSuccessStatusCode))
            {
                logger.LogWarning("Failed to retrieve guide {Id}. Status code: {StatusCode}", id, response.StatusCode);
                return ApiResponse<Guide>.ErrorResult($"Failed to retrieve guide {id}. Status code: {response.StatusCode}");
            }
            var envelope = await response.Content.ReadFromJsonAsync<MmgatResponse<Guide>>(cancellationToken);
            return envelope?.Result is not null
                ? ApiResponse<Guide>.SuccessResult(envelope.Result)
                : ApiResponse<Guide>.ErrorResult("Failed to deserialize guide data from response.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while retrieving guide {Id}.", id);
            return ApiResponse<Guide>.ErrorResult($"An error occurred while retrieving guide {id}: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<Guide>>> GetGuides(CancellationToken cancellationToken = default)
    {
        try
        {
            var url = $"{urlPrefix}allpublished";
            var response = await apiClient.GetAsync(url);
            if ((!response.IsSuccessStatusCode))
            {
                logger.LogWarning("Failed to retrieve guides. Status code: {StatusCode} {url}", response.StatusCode, url);
                return ApiResponse<List<Guide>>.ErrorResult($"Failed to retrieve guides. Status code: {response.StatusCode} {url}");
            }
            var envelope = await response.Content.ReadFromJsonAsync<MmgatResponse<List<Guide>>>(cancellationToken);
            return envelope?.Result is not null
                ? ApiResponse<List<Guide>>.SuccessResult(envelope.Result)
                : ApiResponse<List<Guide>>.ErrorResult("Failed to deserialize guides data from response.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while retrieving guides.");
            return ApiResponse<List<Guide>>.ErrorResult($"An error occurred while retrieving guides: {ex.Message}");
        }
    }
}
