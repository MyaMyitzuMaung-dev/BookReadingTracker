using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;

namespace BookReadingTracker.MVC.Services;

public class ApiService
{
    private readonly HttpClient _http;
    private readonly IHttpContextAccessor _ctx;

    public ApiService(HttpClient http, IHttpContextAccessor ctx)
    {
        _http = http;
        _ctx = ctx;
    }

    private void AddAuthHeader(HttpRequestMessage req)
    {
        var token = _ctx.HttpContext?.User.FindFirst("JwtToken")?.Value;
        if (!string.IsNullOrEmpty(token))
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private static async Task<string?> ReadErrorAsync(HttpResponseMessage resp)
    {
        var body = await resp.Content.ReadAsStringAsync();
        try
        {
            var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("message", out var msg))
                return msg.GetString();
        }
        catch { }
        return body;
    }

    public async Task<T?> GetAsync<T>(string url) where T : class
    {
        var req = new HttpRequestMessage(HttpMethod.Get, url);
        AddAuthHeader(req);
        var resp = await _http.SendAsync(req);

        if ((int)resp.StatusCode >= 400)
            throw new ApiException(await ReadErrorAsync(resp) ?? "Request failed", resp.StatusCode);

        return await resp.Content.ReadFromJsonAsync<T>();
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest data)
        where TResponse : class
    {
        var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = JsonContent.Create(data) };
        AddAuthHeader(req);
        var resp = await _http.SendAsync(req);

        if ((int)resp.StatusCode >= 400)
            throw new ApiException(await ReadErrorAsync(resp) ?? "Request failed", resp.StatusCode);

        return await resp.Content.ReadFromJsonAsync<TResponse>();
    }

    public async Task<TResponse?> PutAsync<TRequest, TResponse>(string url, TRequest data)
        where TResponse : class
    {
        var req = new HttpRequestMessage(HttpMethod.Put, url) { Content = JsonContent.Create(data) };
        AddAuthHeader(req);
        var resp = await _http.SendAsync(req);

        if ((int)resp.StatusCode >= 400)
            throw new ApiException(await ReadErrorAsync(resp) ?? "Request failed", resp.StatusCode);

        return await resp.Content.ReadFromJsonAsync<TResponse>();
    }

    public async Task<T?> DeleteAsync<T>(string url) where T : class
    {
        var req = new HttpRequestMessage(HttpMethod.Delete, url);
        AddAuthHeader(req);
        var resp = await _http.SendAsync(req);

        if ((int)resp.StatusCode >= 400)
            throw new ApiException(await ReadErrorAsync(resp) ?? "Request failed", resp.StatusCode);

        return await resp.Content.ReadFromJsonAsync<T>();
    }

    public async Task<TResponse?> PostPublicAsync<TRequest, TResponse>(string url, TRequest data)
        where TResponse : class
    {
        var resp = await _http.PostAsJsonAsync(url, data);

        if ((int)resp.StatusCode >= 400)
            throw new ApiException(await ReadErrorAsync(resp) ?? "Request failed", resp.StatusCode);

        return await resp.Content.ReadFromJsonAsync<TResponse>();
    }
}

public class ApiException : Exception
{
    public System.Net.HttpStatusCode StatusCode { get; }

    public ApiException(string message, System.Net.HttpStatusCode statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}
