using System.Net.Http.Json;
using MmgExplorer.Models;

namespace MmgExplorer;

public class MmgatClient : IMmgatClient
{
    private const string BaseUrl = "https://apidev.cdc.gov/mmgat/1.0.0/api/guide/";
    private readonly HttpClient _httpClient;

    public MmgatClient()
    {
        _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
    }

    public async Task<ApiMultipleResponse> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("allpublished");

            if (!response.IsSuccessStatusCode)
            {
                return new ApiMultipleResponse(
                    StatusCode: (int)response.StatusCode,
                    Result: new List<Guide>(),
                    Message: $"Request failed: {response.ReasonPhrase}");
            }

            var envelope = await response.Content.ReadFromJsonAsync<ApiMultipleResponse>();
            return envelope ?? new ApiMultipleResponse(
                StatusCode: 500,
                Result: new List<Guide>(),
                Message: "Empty response body");
        }
        catch (Exception ex)
        {
            return new ApiMultipleResponse(
                StatusCode: 500,
                Result: default!,
                Message: ex.Message);
        }
    }

    public async Task<ApiSingleResponse> GetAsync(string id)
    {
        var response = await _httpClient.GetAsync($"{id}");
        if (!response.IsSuccessStatusCode)
        {
            return new ApiSingleResponse(
                StatusCode: (int)response.StatusCode,
                Result: default!,
                Message: $"Request failed: {response.ReasonPhrase}");
        }
        var envelope = await response.Content.ReadFromJsonAsync<ApiSingleResponse>();
        return envelope ?? new ApiSingleResponse(
            StatusCode: 500,
            Result: default!,
            Message: "Empty response body");
    }
}
