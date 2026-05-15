using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace HumanResource.MVC.Services;

public class HrApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public HrApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApiResult<JsonElement?>> GetAsync(string path, string? token)
    {
        return await SendAsync(HttpMethod.Get, path, null, token);
    }

    public async Task<ApiResult<JsonElement?>> PostAsync(string path, object payload, string? token)
    {
        return await SendAsync(HttpMethod.Post, path, payload, token);
    }

    public async Task<ApiResult<JsonElement?>> PutAsync(string path, object payload, string? token)
    {
        return await SendAsync(HttpMethod.Put, path, payload, token);
    }

    public async Task<ApiResult<JsonElement?>> DeleteAsync(string path, string? token)
    {
        return await SendAsync(HttpMethod.Delete, path, null, token);
    }

    private async Task<ApiResult<JsonElement?>> SendAsync(
        HttpMethod method,
        string path,
        object? payload,
        string? token)
    {
        try
        {
            using var request = new HttpRequestMessage(method, path);

            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            if (payload is not null)
            {
                var json = JsonSerializer.Serialize(payload, _jsonOptions);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            using var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return ApiResult<JsonElement?>.Failure(ReadError(content), (int)response.StatusCode);
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                return ApiResult<JsonElement?>.Success(null, (int)response.StatusCode);
            }

            using var document = JsonDocument.Parse(content);
            return ApiResult<JsonElement?>.Success(document.RootElement.Clone(), (int)response.StatusCode);
        }
        catch (HttpRequestException)
        {
            return ApiResult<JsonElement?>.Failure(
                "The API is not reachable. Start HumanResource.API and try again.",
                0);
        }
        catch (TaskCanceledException)
        {
            return ApiResult<JsonElement?>.Failure("The API request timed out.", 0);
        }
    }

    private static string ReadError(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return "The API returned an error.";
        }

        try
        {
            using var document = JsonDocument.Parse(content);
            var root = document.RootElement;

            if (root.ValueKind == JsonValueKind.Object)
            {
                if (root.TryGetProperty("message", out var message))
                {
                    return message.ToString();
                }

                if (root.TryGetProperty("title", out var title))
                {
                    var validationErrors = ReadValidationErrors(root);
                    return string.IsNullOrWhiteSpace(validationErrors)
                        ? title.ToString()
                        : $"{title} {validationErrors}";
                }
            }
        }
        catch (JsonException)
        {
            return content;
        }

        return content;
    }

    private static string ReadValidationErrors(JsonElement root)
    {
        if (!root.TryGetProperty("errors", out var errors)
            || errors.ValueKind != JsonValueKind.Object)
        {
            return string.Empty;
        }

        var messages = new List<string>();
        foreach (var property in errors.EnumerateObject())
        {
            if (property.Value.ValueKind != JsonValueKind.Array)
            {
                continue;
            }

            messages.AddRange(property.Value.EnumerateArray()
                .Select(message => message.ToString())
                .Where(message => !string.IsNullOrWhiteSpace(message)));
        }

        return messages.Count == 0 ? string.Empty : string.Join(" ", messages);
    }
}
