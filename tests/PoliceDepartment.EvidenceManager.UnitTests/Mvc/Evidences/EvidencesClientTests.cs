using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using PoliceDepartment.EvidenceManager.MVC.Client;
using PoliceDepartment.EvidenceManager.MVC.Evidences;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;
using PoliceDepartment.EvidenceManager.UnitTests.Fixtures.Mvc;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using RichardSzalay.MockHttp;
using System.Net;
using System.Text.Json;

namespace PoliceDepartment.EvidenceManager.UnitTests.Mvc.Evidences
{
    [Collection(nameof(MvcFixtureCollection))]
    public class EvidencesClientTests
    {
        private readonly MvcFixture _fixture;

        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
        private readonly JsonSerializerOptions _serializeOptions;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILoggerManager _logger;

        public EvidencesClientTests(MvcFixture fixture)
        {
            _fixture = fixture;

            _retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10),
                }, (outcome, timespan, retryCount, context) =>
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"Attempt: {retryCount}!");
                    Console.ForegroundColor = ConsoleColor.White;
                });
            _serializeOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            _clientFactory = Substitute.For<IHttpClientFactory>();
            _configuration = Substitute.For<IConfiguration>();
            _configuration["Api:Evidences:Endpoints:CreateEvidenceImage"].Returns(_fixture.Evidences.CreateEvidenceImageUrl);
            _configuration["Api:Evidences:Endpoints:CreateEvidence"].Returns(_fixture.Evidences.CreateEvidenceUrl);
            _configuration["Api:Evidences:Endpoints:DeleteEvidenceImage"].Returns(_fixture.Evidences.DeleteEvidenceImageUrl);
            _configuration["Api:Evidences:Endpoints:GetEvidenceById"].Returns(_fixture.Evidences.GetEvidenceByIdUrl);
            _configuration["Api:Evidences:Endpoints:GetEvidenceImage"].Returns(_fixture.Evidences.GetEvidenceImageUrl);
            _logger = Substitute.For<ILoggerManager>();
        }

        [Theory]
        [InlineData(HttpStatusCode.InternalServerError, ResponseMessage.GenericError)]
        [InlineData(HttpStatusCode.Unauthorized, ResponseMessage.UserIsNotAuthenticated)]
        [InlineData(HttpStatusCode.BadRequest, ResponseMessage.InvalidEvidence)]
        [InlineData(HttpStatusCode.OK, ResponseMessage.Success)]
        public void CreateEvidenceImageAsync_AllCases_ShouldReturnExpectedResponse(HttpStatusCode code, ResponseMessage responseMessage)
        {
            //Arrange
            var responseModel = responseMessage == ResponseMessage.Success ?
                new BaseResponseWithValue<string>().AsSuccess(Guid.NewGuid().ToString()) :
                new BaseResponseWithValue<string>().AsError(responseMessage);
            var expectedResponseBody = JsonSerializer.Serialize(responseModel, _serializeOptions);
            var mockHttpRequest = new MockHttpMessageHandler();
            mockHttpRequest.When(_fixture.Evidences.CreateEvidenceImageUrl)
                           .Respond(code, "application/json", expectedResponseBody);
            var mockClient = mockHttpRequest.ToHttpClient();
            _clientFactory.CreateClient(ClientExtensions.EvidencesClientName).Returns(mockClient);

            var image = Substitute.For<IFormFile>();
            image.OpenReadStream().Returns(new MemoryStream());

            var sut = new EvidencesClient(_retryPolicy, _serializeOptions, _clientFactory, _configuration, _logger);

            //Act
            var response = sut.CreateEvidenceImageAsync(image, _fixture.Authorization.GenerateFakeJwtToken(), CancellationToken.None).Result;

            //Assert
            response.Should().NotBeNull();
            response.Success.Should().Be(responseModel.Success);
            response.ResponseDetails.Message.Should().Be(responseModel.ResponseDetails.Message);
        }

        [Theory]
        [InlineData(HttpStatusCode.InternalServerError)]
        [InlineData(HttpStatusCode.Forbidden)]
        [InlineData(HttpStatusCode.Unauthorized)]
        [InlineData(HttpStatusCode.NotFound)]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.OK)]
        public void DeleteAsync_AllCases_ShouldReturnExpectedResponse(HttpStatusCode code)
        {
            //Arrange
            var responseModel = code == HttpStatusCode.OK ?
                new BaseResponseWithValue<string>().AsSuccess(Guid.NewGuid().ToString()) :
                new BaseResponseWithValue<string>().AsError();
            var expectedResponseBody = JsonSerializer.Serialize(responseModel, _serializeOptions);
            var mockHttpRequest = new MockHttpMessageHandler();
            mockHttpRequest.When(_fixture.Evidences.DeleteEvidenceImageUrl)
                           .Respond(code, "application/json", expectedResponseBody);
            var mockClient = mockHttpRequest.ToHttpClient();
            _clientFactory.CreateClient(ClientExtensions.EvidencesClientName).Returns(mockClient);

            var sut = new EvidencesClient(_retryPolicy, _serializeOptions, _clientFactory, _configuration, _logger);

            //Act
            var response = sut.DeleteEvidenceImageAsync(Guid.NewGuid().ToString(), _fixture.Authorization.GenerateFakeJwtToken(), CancellationToken.None).Result;

            //Assert
            response.Should().NotBeNull();
            if (code == HttpStatusCode.OK)
                response.Success.Should().BeTrue();
            else
                response.Success.Should().BeFalse();
        }

        [Theory]
        [InlineData(HttpStatusCode.InternalServerError, ResponseMessage.GenericError)]
        [InlineData(HttpStatusCode.Unauthorized, ResponseMessage.UserIsNotAuthenticated)]
        [InlineData(HttpStatusCode.BadRequest, ResponseMessage.InvalidEvidence)]
        [InlineData(HttpStatusCode.OK, ResponseMessage.Success)]
        public void CreateEvidenceAsync_AllCases_ShouldReturnExpectedResponse(HttpStatusCode code, ResponseMessage responseMessage)
        {
            //Arrange
            var responseModel = responseMessage == ResponseMessage.Success ?
                new BaseResponse().AsSuccess() :
                new BaseResponse().AsError(responseMessage);
            var expectedResponseBody = JsonSerializer.Serialize(responseModel, _serializeOptions);
            var mockHttpRequest = new MockHttpMessageHandler();
            mockHttpRequest.When(_fixture.Evidences.CreateEvidenceUrl)
                           .Respond(code, "application/json", expectedResponseBody);
            var mockClient = mockHttpRequest.ToHttpClient();
            _clientFactory.CreateClient(ClientExtensions.EvidencesClientName).Returns(mockClient);

            var sut = new EvidencesClient(_retryPolicy, _serializeOptions, _clientFactory, _configuration, _logger);

            //Act
            var response = sut.CreateEvidenceAsync(_fixture.Evidences.GenerateSingleCreateEvidenceViewModel(), _fixture.Authorization.GenerateFakeJwtToken(), CancellationToken.None).Result;

            //Assert
            response.Should().NotBeNull();
            response.Success.Should().Be(responseModel.Success);
            response.ResponseDetails.Message.Should().Be(responseModel.ResponseDetails.Message);
        }

        [Theory]
        [InlineData(HttpStatusCode.InternalServerError)]
        [InlineData(HttpStatusCode.Forbidden)]
        [InlineData(HttpStatusCode.Unauthorized)]
        [InlineData(HttpStatusCode.NotFound)]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.OK)]
        public void GetEvidenceByIdAsync_AllCases_ShouldReturnExpectedResponse(HttpStatusCode code)
        {
            //Arrange
            var responseModel = code == HttpStatusCode.OK ?
                new BaseResponseWithValue<EvidenceViewModel>().AsSuccess(_fixture.Evidences.GenerateSingleViewModel()) :
                new BaseResponseWithValue<EvidenceViewModel>().AsError();
            var expectedResponseBody = JsonSerializer.Serialize(responseModel, _serializeOptions);
            var mockHttpRequest = new MockHttpMessageHandler();
            mockHttpRequest.When(_fixture.Evidences.GetEvidenceByIdUrl)
                           .Respond(code, "application/json", expectedResponseBody);
            var mockClient = mockHttpRequest.ToHttpClient();
            _clientFactory.CreateClient(ClientExtensions.EvidencesClientName).Returns(mockClient);

            var sut = new EvidencesClient(_retryPolicy, _serializeOptions, _clientFactory, _configuration, _logger);

            //Act
            var response = sut.GetEvidenceByIdAsync(Guid.NewGuid(), _fixture.Authorization.GenerateFakeJwtToken(), CancellationToken.None).Result;

            //Assert
            response.Should().NotBeNull();
            if (code == HttpStatusCode.OK)
                response.Success.Should().BeTrue();
            else
                response.Success.Should().BeFalse();
        }

        [Theory]
        [InlineData(HttpStatusCode.InternalServerError)]
        [InlineData(HttpStatusCode.Forbidden)]
        [InlineData(HttpStatusCode.Unauthorized)]
        [InlineData(HttpStatusCode.NotFound)]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.OK)]
        public void GetEvidenceImageAsync_AllCases_ShouldReturnExpectedResponse(HttpStatusCode code)
        {
            //Arrange
            var responseModel = code == HttpStatusCode.OK ?
                new BaseResponseWithValue<string>().AsSuccess("https://fakeimageprovider.com/12312312312312312") :
                new BaseResponseWithValue<string>().AsError();
            var expectedResponseBody = JsonSerializer.Serialize(responseModel, _serializeOptions);
            var mockHttpRequest = new MockHttpMessageHandler();
            mockHttpRequest.When(_fixture.Evidences.GetEvidenceImageUrl)
                           .Respond(code, "application/json", expectedResponseBody);
            var mockClient = mockHttpRequest.ToHttpClient();
            _clientFactory.CreateClient(ClientExtensions.EvidencesClientName).Returns(mockClient);

            var sut = new EvidencesClient(_retryPolicy, _serializeOptions, _clientFactory, _configuration, _logger);

            //Act
            var response = sut.GetEvidenceImageAsync(Guid.NewGuid().ToString(), _fixture.Authorization.GenerateFakeJwtToken(), CancellationToken.None).Result;

            //Assert
            response.Should().NotBeNull();
            if (code == HttpStatusCode.OK)
                response.Success.Should().BeTrue();
            else
                response.Success.Should().BeFalse();
        }
    }
}
