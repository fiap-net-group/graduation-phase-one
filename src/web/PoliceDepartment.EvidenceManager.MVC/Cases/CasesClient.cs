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

            ArgumentException.ThrowIfNullOrEmpty(_getCasesByOfficerIdUrl);
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

        public async Task<BaseResponse> CreateCaseAsync(CaseViewModel caseViewModel, string accessToken, CancellationToken cancellationToken)
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
                return new BaseResponseWithValue<BaseResponse>().AsError(ResponseMessage.GenericError);
            }
        }
    }
}
