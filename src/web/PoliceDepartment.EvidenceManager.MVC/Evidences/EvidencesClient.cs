using PoliceDepartment.EvidenceManager.MVC.Client;
using PoliceDepartment.EvidenceManager.MVC.Evidences.Interfaces;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;
using Polly.Retry;
using System.Text;
using System.Text.Json;

namespace PoliceDepartment.EvidenceManager.MVC.Evidences
{
    public class EvidencesClient : BaseClient, IEvidencesClient
    {
        private readonly JsonSerializerOptions _serializeOptions;

        private readonly string _createEvidenceUrl;
        private readonly string _deleteEvidenceUrl;
        public EvidencesClient(AsyncRetryPolicy<HttpResponseMessage> retryPolicy,
                           JsonSerializerOptions serializeOptions,
                           IHttpClientFactory clientFactory,
                           IConfiguration configuration,
                           ILoggerManager logger) : base(retryPolicy, serializeOptions, clientFactory.CreateClient(ClientExtensions.EvidencesClientName), configuration, logger)
        {
            _serializeOptions = serializeOptions;
            _createEvidenceUrl = configuration["Api:Evidences:Endpoints:CreateEvidence"];
            _deleteEvidenceUrl = configuration["Api:Evidences:Endpoints:Delete"];

            ArgumentException.ThrowIfNullOrEmpty(_createEvidenceUrl);
        }

        public async Task<BaseResponse> CreateEvidenceAsync(CreateEvidenceViewModel evidenceViewModel, string accessToken, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _createEvidenceUrl)
            {
                Content = new StringContent(JsonSerializer.Serialize(evidenceViewModel, _serializeOptions), Encoding.UTF8, "application/json")
            };

            try
            {
                return await SendAuthenticatedAsync<BaseResponse>(request, accessToken, cancellationToken);
            }
            catch (Exception)
            {
                return new BaseResponse().AsError(ResponseMessage.GenericError);
            }
        }

        public async Task<BaseResponse> DeleteAsync(Guid id, string accessToken, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, _deleteEvidenceUrl + $"/{id}");

            try
            {
                var response = await SendAuthenticatedAsync(request, accessToken, cancellationToken);

                return response ? new BaseResponse().AsSuccess() : new BaseResponse().AsError();
            }
            catch
            {
                return new BaseResponse().AsError(ResponseMessage.GenericError);
            }
        }
    }
}
