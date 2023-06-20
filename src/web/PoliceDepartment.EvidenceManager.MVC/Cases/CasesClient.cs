using PoliceDepartment.EvidenceManager.MVC.Cases.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Client;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;
using Polly.Retry;
using System.Text;
using System.Text.Json;

namespace PoliceDepartment.EvidenceManager.MVC.Cases
{
    public class CasesClient : BaseClient, ICasesClient
    {
        private readonly JsonSerializerOptions _serializeOptions;

        private readonly string _getCasesByOfficerIdUrl;
        private readonly string _createCaseUrl;
        private readonly string _getDetailsUrl;
        private readonly string _editUrl;
        private readonly string _deleteUrl;

        public CasesClient(AsyncRetryPolicy<HttpResponseMessage> retryPolicy,
                           JsonSerializerOptions serializeOptions,
                           IHttpClientFactory clientFactory,
                           IConfiguration configuration,
                           ILoggerManager logger) : base(retryPolicy,
                                                         serializeOptions,
                                                         clientFactory.CreateClient(ClientExtensions.CasesClientName),
                                                         configuration,
                                                         logger)
        {
            _serializeOptions = serializeOptions;

            _getCasesByOfficerIdUrl = configuration["Api:Cases:Endpoints:GetCasesByOfficerId"];
            _createCaseUrl = configuration["Api:Cases:Endpoints:CreateCase"];
            _getDetailsUrl = configuration["Api:Cases:Endpoints:GetDetails"];
            _editUrl = configuration["Api:Cases:Endpoints:Edit"];
            _deleteUrl = configuration["Api:Cases:Endpoints:Delete"];

            ArgumentException.ThrowIfNullOrEmpty(_getCasesByOfficerIdUrl);
            ArgumentException.ThrowIfNullOrEmpty(_createCaseUrl);
            ArgumentException.ThrowIfNullOrEmpty(_getDetailsUrl);
            ArgumentException.ThrowIfNullOrEmpty(_editUrl);
            ArgumentException.ThrowIfNullOrEmpty(_deleteUrl);
        }

        public async Task<BaseResponseWithValue<IEnumerable<CaseViewModel>>> GetByOfficerIdAsync(Guid officerId, string accessToken, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _getCasesByOfficerIdUrl + $"/{officerId}");

            try
            {
                return await SendAuthenticatedAsync<BaseResponseWithValue<IEnumerable<CaseViewModel>>>(request, accessToken, cancellationToken);
            }
            catch
            {
                return new BaseResponseWithValue<IEnumerable<CaseViewModel>>().AsError(ResponseMessage.GenericError);
            }
        }

        public async Task<BaseResponse> CreateCaseAsync(CreateCaseViewModel caseViewModel, string accessToken, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _createCaseUrl)
            {
                Content = new StringContent(JsonSerializer.Serialize(caseViewModel, _serializeOptions),
                                            Encoding.UTF8,
                                            "application/json")
            };

            try
            {
                return await SendAuthenticatedAsync<BaseResponse>(request, accessToken, cancellationToken);
            }
            catch
            {
                return new BaseResponse().AsError(ResponseMessage.GenericError);
            }
        }

        public async Task<BaseResponseWithValue<CaseViewModel>> GetDetailsAsync(Guid id, string accessToken, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _getDetailsUrl + $"/{id}");

            try
            {
                return await SendAuthenticatedAsync<BaseResponseWithValue<CaseViewModel>>(request, accessToken, cancellationToken);
            }
            catch
            {
                return new BaseResponseWithValue<CaseViewModel>().AsError(ResponseMessage.GenericError);
            }
        }

        public async Task<BaseResponse> EditAsync(Guid id, CaseViewModel caseViewModel, string accessToken, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Patch, _editUrl + $"/{id}")
            {
                Content = new StringContent(JsonSerializer.Serialize(caseViewModel, _serializeOptions),
                                            Encoding.UTF8,
                                            "application/json")
            };

            try
            {
                return await SendAuthenticatedAsync<BaseResponse>(request, accessToken, cancellationToken);
            }
            catch
            {
                return new BaseResponse().AsError(ResponseMessage.GenericError);
            }
        }

        public async Task<BaseResponse> DeleteAsync(Guid id, string accessToken, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, _deleteUrl + $"/{id}");

            try
            {
                await SendAuthenticatedAsync(request, accessToken, cancellationToken);

                return new BaseResponse().AsSuccess();
            }
            catch
            {
                return new BaseResponse().AsError(ResponseMessage.GenericError);
            }
        }
    }
}
