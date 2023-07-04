using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using Polly.Retry;
using System.Net.Http.Headers;
using System.Text.Json;

namespace PoliceDepartment.EvidenceManager.MVC.Client
{
    public class BaseClient
    {
        protected HttpClient Client { get; }

        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
        private readonly JsonSerializerOptions _serializeOptions;
        private readonly string _apiKey;
        private readonly ILoggerManager _logger;

        public BaseClient(AsyncRetryPolicy<HttpResponseMessage> retryPolicy,
                          JsonSerializerOptions serializeOptions,
                          HttpClient client,
                          IConfiguration configuration,
                          ILoggerManager logger)
        {
            _retryPolicy = retryPolicy;
            _serializeOptions = serializeOptions;
            Client = client;
            _apiKey = configuration["Api:ApiKey"];
            _logger = logger;
        }

        protected async Task<bool> SendAuthenticatedAsync(HttpRequestMessage request, string accessToken, CancellationToken cancellationToken)
        {
            try
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var apiResponse = await _retryPolicy.ExecuteAsync(async action =>
                {
                    request.Headers.Add(ClientExtensions.ApiKeyHeader, _apiKey);

                    return await Client.SendAsync(request, cancellationToken);
                },
                cancellationToken);

                _logger.LogDebug("API call", ("response", apiResponse));

                return apiResponse.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Error on API Access", ex);

                throw;
            }
        }

        protected async Task<TResponse> SendAuthenticatedAsync<TResponse>(HttpRequestMessage request, string accessToken, CancellationToken cancellationToken)
        {
            try
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var apiResponse = await _retryPolicy.ExecuteAsync(async action =>
                {
                    request.Headers.Add(ClientExtensions.ApiKeyHeader, _apiKey);

                    return await Client.SendAsync(request, cancellationToken);
                },
                cancellationToken);

                _logger.LogDebug("API call with response", ("response", apiResponse));

                return await apiResponse.Content.ReadFromJsonAsync<TResponse>(_serializeOptions, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Error on API Access", ex);

                throw;
            }
        }

        protected async Task<TResponse> SendAsync<TResponse>(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                var apiResponse = await _retryPolicy.ExecuteAsync(async action =>
                {
                    request.Headers.Add(ClientExtensions.ApiKeyHeader, _apiKey);

                    return await Client.SendAsync(request, cancellationToken);
                },
                cancellationToken);

                _logger.LogDebug("API call with response", ("response", apiResponse));

                return await apiResponse.Content.ReadFromJsonAsync<TResponse>(_serializeOptions, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Error on API Access", ex);

                throw;
            }
        }
    }
}
