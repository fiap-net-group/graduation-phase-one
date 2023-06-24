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

        private readonly string _createEvidenceImageUrl;
        private readonly string _createEvidenceUrl;
        private readonly string _deleteEvidenceImageUrl;
        public EvidencesClient(AsyncRetryPolicy<HttpResponseMessage> retryPolicy,
                           JsonSerializerOptions serializeOptions,
                           IHttpClientFactory clientFactory,
                           IConfiguration configuration,
                           ILoggerManager logger) : base(retryPolicy, serializeOptions, clientFactory.CreateClient(ClientExtensions.EvidencesClientName), configuration, logger)
        {
            _serializeOptions = serializeOptions;
            _createEvidenceImageUrl = configuration["Api:Evidences:Endpoints:CreateEvidenceImage"];
            _createEvidenceUrl = configuration["Api:Evidences:Endpoints:CreateEvidence"];
            _deleteEvidenceImageUrl = configuration["Api:Evidences:Endpoints:DeleteEvidenceImage"];

            ArgumentException.ThrowIfNullOrEmpty(_createEvidenceUrl);
        }

        public async Task<BaseResponseWithValue<string>> CreateEvidenceImage(IFormFile image, string accessToken, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _createEvidenceImageUrl)
            {
                Content = new MultipartFormDataContent
                {
                    { new StreamContent(image.OpenReadStream()), "csvFile", "filename" }
                }
            };

            try
            {
                return await SendAuthenticatedAsync<BaseResponseWithValue<string>>(request, accessToken, cancellationToken);
            }
            catch (Exception)
            {
                return new BaseResponseWithValue<string>().AsError(ResponseMessage.GenericError);
            }
        }

        public async Task<BaseResponseWithValue<string>> DeleteEvidenceImage(string imageId, string accessToken, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, _deleteEvidenceImageUrl + $"/{imageId}");            

            try
            {
                return await SendAuthenticatedAsync<BaseResponseWithValue<string>>(request, accessToken, cancellationToken);
            }
            catch (Exception)
            {
                return new BaseResponseWithValue<string>().AsError(ResponseMessage.GenericError);
            }
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
    }
}
