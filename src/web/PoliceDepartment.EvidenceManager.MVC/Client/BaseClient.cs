using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using Polly.Retry;
using System.Text.Json;

namespace PoliceDepartment.EvidenceManager.MVC.Client
{
    public class BaseClient
    {
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
        private readonly JsonSerializerOptions _serializeOptions;
        private readonly HttpClient _client;
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
            _client = client;
            _apiKey = configuration["Api:ApiKey"];
            _logger = logger;
        }

        public async Task<TResponse> SendAsync<TResponse>(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                var apiResponse = await _retryPolicy.ExecuteAsync(async action =>
                {
                    request.Headers.Add(ClientExtensions.ApiKeyHeader, _apiKey);

                    return await _client.SendAsync(request, cancellationToken);
                }, cancellationToken);

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
