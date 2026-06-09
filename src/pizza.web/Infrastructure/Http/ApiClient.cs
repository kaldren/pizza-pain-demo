using System.Net.Http.Json;

namespace pizza.web.Infrastructure.Http;

/// <summary>
/// Centralized typed <see cref="HttpClient"/> wrapper for talking to the Pizza Pain HTTP API
/// (<c>src/pizza.api</c>). Registered with <c>AddHttpClient&lt;ApiClient&gt;</c>, so its
/// <see cref="HttpClient.BaseAddress"/> is configured once (from <c>PizzaApi:BaseUrl</c>) and every
/// feature talks to the API through this single seam rather than new-ing up clients.
/// </summary>
public sealed class ApiClient(HttpClient httpClient)
{
    /// <summary>
    /// Issues a GET to <paramref name="requestUri"/> (relative to the configured base address) and
    /// deserializes the JSON body to <typeparamref name="T"/> using the API's default camelCase
    /// settings. Throws on a non-success status or transport failure; callers that want to surface a
    /// friendly error state (e.g. the catalog page) should catch.
    /// </summary>
    public Task<T?> GetAsync<T>(string requestUri, CancellationToken cancellationToken = default) =>
        httpClient.GetFromJsonAsync<T>(requestUri, cancellationToken);
}
