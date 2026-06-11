namespace MmgExplorer.Services.Interfaces;

/// <summary>
/// Defines the contract for a client that communicates with an external API.
/// </summary>
/// <remarks>Implementations of this interface provide methods for interacting with remote services or endpoints.
/// The specific operations and usage details are defined by the implementing class.</remarks>
public interface IApiClient
{
    Task<HttpResponseMessage> GetAsync(string endpoint);
    Task<HttpResponseMessage> PostAsync(string endpoint, HttpContent content);
    Task<HttpResponseMessage> PutAsync(string endpoint, HttpContent content);
    Task<HttpResponseMessage> DeleteAsync(string endpoint);
}
