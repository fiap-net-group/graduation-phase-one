using PoliceDepartment.EvidenceManager.MVC.Authorization.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Client;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;
using Polly.Retry;
using System.Text;
using System.Text.Json;

namespace PoliceDepartment.EvidenceManager.MVC.Authorization
{
    public sealed class AuthorizationClient : BaseClient, IAuthorizationClient
    {
        private readonly JsonSerializerOptions _serializeOptions;

        private readonly string _loginUrl;

        public AuthorizationClient(AsyncRetryPolicy<HttpResponseMessage> retryPolicy,
                                   JsonSerializerOptions serializeOptions,
                                   IHttpClientFactory _clientFactory,
                                   IConfiguration configuration,
                                   ILoggerManager logger) : base(retryPolicy,
                                                                 serializeOptions,
                                                                 _clientFactory.CreateClient(ClientExtensions.AuthorizationClientName),
                                                                 configuration,
                                                                 logger)
        {
            _serializeOptions = serializeOptions;

            _loginUrl = configuration["Api:Authorization:Endpoints:Login"];

            ArgumentException.ThrowIfNullOrEmpty(_loginUrl);
        }

        public async Task<BaseResponseWithValue<AccessTokenViewModel>> AuthorizeAsync(string username, string password, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _loginUrl)
            {
                Content = new StringContent(JsonSerializer.Serialize(new LoginViewModel(username, password), _serializeOptions),
                                            Encoding.UTF8,
                                            "application/json")
            };

            try
            {
                return await SendAsync<BaseResponseWithValue<AccessTokenViewModel>>(request, cancellationToken);
            }
            catch
            {
                return new BaseResponseWithValue<AccessTokenViewModel>().AsError(ResponseMessage.GenericError);   
            }
        }
    }
}
