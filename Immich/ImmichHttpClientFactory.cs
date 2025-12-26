// Purpose: Create and configure the HttpClient used to communicate with the Immich API.

namespace ImmichNsfwLocal.Immich;

public static class ImmichHttpClientFactory
{
    public static HttpClient Create(string baseUrl, string apiKey)
    {
        var http = new HttpClient
        {
            BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/"),
            Timeout = TimeSpan.FromSeconds(60)
        };

        http.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        http.DefaultRequestHeaders.Add("x-api-key", apiKey);

        return http;
    }
}
