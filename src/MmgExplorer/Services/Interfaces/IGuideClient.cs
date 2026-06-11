using MmgExplorer.Models;

namespace MmgExplorer.Services.Interfaces;

public interface IGuideClient
{
    Task<ApiResponse<List<Guide>>> GetGuides(CancellationToken cancellationToken = default);
    Task<ApiResponse<Guide>> GetGuideById(Guid id, CancellationToken cancellationToken = default);
}
