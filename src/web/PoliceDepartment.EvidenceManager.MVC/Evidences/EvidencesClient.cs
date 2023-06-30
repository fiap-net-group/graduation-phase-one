using Microsoft.AspNetCore.Http;
using PoliceDepartment.EvidenceManager.MVC.Client;
using PoliceDepartment.EvidenceManager.MVC.Evidences.Interfaces;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;
using Polly.Retry;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace PoliceDepartment.EvidenceManager.MVC.Evidences
{
    public class EvidencesClient : BaseClient, IEvidencesClient
    {
        private readonly JsonSerializerOptions _serializeOptions;

        private readonly string _createEvidenceImageUrl;
        private readonly string _createEvidenceUrl;
        private readonly string _editUrl;
        private readonly string _deleteEvidenceImageUrl;
        private readonly string _deleteEvidenceUrl;
        private readonly string _getEvidenceByIdUrl;
        private readonly string _getEvidenceImageUrl;
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
            _deleteEvidenceUrl = configuration["Api:Evidences:Endpoints:DeleteEvidence"];
            _editUrl = configuration["Api:Evidences:Endpoints:EditEvidence"];
            _getEvidenceByIdUrl = configuration["Api:Evidences:Endpoints:GetDetails"];
            _getEvidenceImageUrl = configuration["Api:Evidences:Endpoints:GetEvidenceImage"];

            ArgumentException.ThrowIfNullOrEmpty(_createEvidenceUrl);
        }

        public async Task<BaseResponseWithValue<string>> CreateEvidenceImageAsync(IFormFile image, string accessToken, CancellationToken cancellationToken)
        {
            await using var imageStream = image.OpenReadStream();
            using var bynaryReader = new BinaryReader(imageStream);
            var data = bynaryReader.ReadBytes((int)imageStream.Length);

            var evidenceImage = JsonSerializer.Serialize(new EvidenceFileViewModel
            {
                FileExtension = Path.GetExtension(image.FileName),
                ImageByte = data
            }, 
            _serializeOptions);

            using var request = new HttpRequestMessage(HttpMethod.Post, _createEvidenceImageUrl)
            {
                Content = new StringContent(evidenceImage, Encoding.UTF8, "application/json")
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

        public async Task<BaseResponse> EditAsync(Guid id, EvidenceViewModel evidenceViewModel, string accessToken, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Patch, _editUrl + $"/{id}")
            {
                Content = new StringContent(JsonSerializer.Serialize(evidenceViewModel, _serializeOptions),
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

        public async Task<BaseResponse> DeleteEvidenceAsync(Guid id, string accessToken, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, _deleteEvidenceUrl + $"/{id}");

            try
            {
                return await SendAuthenticatedAsync<BaseResponseWithValue<string>>(request, accessToken, cancellationToken);
            }
            catch (Exception)
            {
                return new BaseResponse().AsError(ResponseMessage.GenericError);
            }
        }

        public async Task<BaseResponseWithValue<string>> DeleteEvidenceImageAsync(string imageId, string accessToken, CancellationToken cancellationToken)
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

        public async Task<BaseResponseWithValue<EvidenceViewModel>> GetEvidenceByIdAsync(Guid id, string accessToken, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _getEvidenceByIdUrl + $"/{id}");

            try
            {
                return await SendAuthenticatedAsync<BaseResponseWithValue<EvidenceViewModel>>(request, accessToken, cancellationToken);
            }
            catch (Exception)
            {
                return new BaseResponseWithValue<EvidenceViewModel>().AsError(ResponseMessage.GenericError);
            }
        }

        public async Task<BaseResponseWithValue<string>> GetEvidenceImageAsync(string imageId, string accessToken, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _getEvidenceImageUrl + $"/{imageId}");

            try
            {
                return await SendAuthenticatedAsync<BaseResponseWithValue<string>>(request, accessToken, cancellationToken);
            }
            catch (Exception)
            {
                return new BaseResponseWithValue<string>().AsError(ResponseMessage.GenericError);
            }
        }
    }
}
