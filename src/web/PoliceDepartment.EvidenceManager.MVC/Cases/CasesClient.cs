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

            ArgumentException.ThrowIfNullOrEmpty(_getCasesByOfficerIdUrl);
        }

        public async Task<BaseResponseWithValue<IEnumerable<CaseViewModel>>> GetByOfficerIdAsync(Guid officerId, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _getCasesByOfficerIdUrl + $"/{officerId}");

            try
            {
                return await SendAsync<BaseResponseWithValue<IEnumerable<CaseViewModel>>>(request, cancellationToken);
            }
            catch
            {
                return new BaseResponseWithValue<IEnumerable<CaseViewModel>>().AsError(ResponseMessage.GenericError);
            }
        }
    }
}
