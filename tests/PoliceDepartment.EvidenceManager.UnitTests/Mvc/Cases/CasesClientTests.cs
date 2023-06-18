using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using PoliceDepartment.EvidenceManager.MVC.Authorization;
using PoliceDepartment.EvidenceManager.MVC.Cases;
using PoliceDepartment.EvidenceManager.MVC.Client;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;
using PoliceDepartment.EvidenceManager.SharedKernel.Extensions;
using PoliceDepartment.EvidenceManager.UnitTests.Fixtures.Mvc;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using RichardSzalay.MockHttp;
using System.Net;
using System.Text.Json;

namespace PoliceDepartment.EvidenceManager.UnitTests.Mvc.Cases
{
    [Collection(nameof(MvcFixtureCollection))]
    public class CasesClientTests
    {
        private readonly MvcFixture _fixture;

        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
        private readonly JsonSerializerOptions _serializeOptions;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILoggerManager _logger;

        public CasesClientTests(MvcFixture fixture)
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
            _configuration["Api:Cases:Endpoints:GetCasesByOfficerId"].Returns(_fixture.Cases.GetByOfficerIdUrl);
            _configuration["Api:Cases:Endpoints:CreateCase"].Returns(_fixture.Cases.CreateCaseUrl);
            _logger = Substitute.For<ILoggerManager>();
        }

        [Fact]
        public void GetByOfficerIdAsync_ServerIsOff_ShouldReturnGenericError()
        {
            //Arrange
            var mockHttpRequest = new MockHttpMessageHandler();
            mockHttpRequest.When(_fixture.Cases.GetByOfficerIdUrl)
                           .Respond(HttpStatusCode.InternalServerError, "application/json", "{'error' : 'An error ocurred'}");
            var mockClient = mockHttpRequest.ToHttpClient();
            _clientFactory.CreateClient(ClientExtensions.CasesClientName).Returns(mockClient);

            var sut = new CasesClient(_retryPolicy, _serializeOptions, _clientFactory, _configuration, _logger);

            //Act
            var response = sut.GetByOfficerIdAsync(Guid.NewGuid(), _fixture.Authorization.GenerateFakeJwtToken(), CancellationToken.None).Result;

            //Assert
            response.Should().NotBeNull();
            response.Success.Should().BeFalse();
            response.ResponseDetails.Message.Should().Be("An error ocurred, try again later");
        }

        [Fact]
        public void GetByOfficerIdAsync_Unauthorized_ShouldReturnUnauthorized()
        {
            //Arrange
            var expectedResponseBody = JsonSerializer.Serialize(new BaseResponseWithValue<IEnumerable<CaseViewModel>>().AsError(ResponseMessage.UserIsNotAuthenticated), _serializeOptions);
            var mockHttpRequest = new MockHttpMessageHandler();
            mockHttpRequest.When(_fixture.Cases.GetByOfficerIdUrl)
                           .Respond(HttpStatusCode.Unauthorized, "application/json", expectedResponseBody);
            var mockClient = mockHttpRequest.ToHttpClient();
            _clientFactory.CreateClient(ClientExtensions.CasesClientName).Returns(mockClient);

            var sut = new CasesClient(_retryPolicy, _serializeOptions, _clientFactory, _configuration, _logger);

            //Act
            var response = sut.GetByOfficerIdAsync(Guid.NewGuid(), _fixture.Authorization.GenerateFakeJwtToken(), CancellationToken.None).Result;

            //Assert
            response.Should().NotBeNull();
            response.Success.Should().BeFalse();
            response.ResponseDetails.Message.Should().Be(ResponseMessage.UserIsNotAuthenticated.GetDescription());
        }

        [Fact]
        public void GetByOfficerIdAsync_ValidRequest_ShouldReturnSuccessAndValidToken()
        {
            //Arrange
            var officerId = Guid.NewGuid();
            var expectedResponseBody = JsonSerializer.Serialize(new BaseResponseWithValue<IEnumerable<CaseViewModel>>().AsSuccess(_fixture.Cases.GenerateViewModelCollection(3, officerId)), _serializeOptions);
            var mockHttpRequest = new MockHttpMessageHandler();
            mockHttpRequest.When(_fixture.Cases.GetByOfficerIdUrl)
                           .Respond(HttpStatusCode.OK, "application/json", expectedResponseBody);
            var mockClient = mockHttpRequest.ToHttpClient();
            _clientFactory.CreateClient(ClientExtensions.CasesClientName).Returns(mockClient);

            var sut = new CasesClient(_retryPolicy, _serializeOptions, _clientFactory, _configuration, _logger);

            //Act
            var response = sut.GetByOfficerIdAsync(officerId, _fixture.Authorization.GenerateFakeJwtToken(), CancellationToken.None).Result;

            //Assert
            response.Should().NotBeNull();
            response.Success.Should().BeTrue();
            response.Value.Should().NotBeEmpty();
            response.Value.Any(c => c.OfficerId == officerId).Should().BeTrue();
        }

        [Fact]
        public void CreateCaseAsync_ServerIsOff_ShouldReturnGenericError()
        {
            //Arrange
            var mockHttpRequest = new MockHttpMessageHandler();
            mockHttpRequest.When(_fixture.Cases.CreateCaseUrl)
                           .Respond(HttpStatusCode.InternalServerError, "application/json", "{'error' : 'An error ocurred'}");
            var mockClient = mockHttpRequest.ToHttpClient();
            _clientFactory.CreateClient(ClientExtensions.CasesClientName).Returns(mockClient);

            var sut = new CasesClient(_retryPolicy, _serializeOptions, _clientFactory, _configuration, _logger);

            //Act
            var response = sut.CreateCaseAsync(_fixture.Cases.GenerateSingleCreateCaseViewModel(), _fixture.Authorization.GenerateFakeJwtToken(), CancellationToken.None).Result;

            //Assert
            response.Should().NotBeNull();
            response.Success.Should().BeFalse();
            response.ResponseDetails.Message.Should().Be("An error ocurred, try again later");
        }

        [Fact]
        public void CreateCaseAsync_InvalidCase_ShouldReturnBadRequest()
        {
            //Arrange
            var expectedResponseBody = JsonSerializer.Serialize(new BaseResponse().AsError(ResponseMessage.InvalidCase), _serializeOptions);
            var mockHttpRequest = new MockHttpMessageHandler();
            mockHttpRequest.When(_fixture.Cases.CreateCaseUrl)
                           .Respond(HttpStatusCode.Unauthorized, "application/json", expectedResponseBody);
            var mockClient = mockHttpRequest.ToHttpClient();
            _clientFactory.CreateClient(ClientExtensions.CasesClientName).Returns(mockClient);

            var sut = new CasesClient(_retryPolicy, _serializeOptions, _clientFactory, _configuration, _logger);

            //Act
            var response = sut.CreateCaseAsync(new CreateCaseViewModel(), _fixture.Authorization.GenerateFakeJwtToken(), CancellationToken.None).Result;

            //Assert
            response.Should().NotBeNull();
            response.Success.Should().BeFalse();
            response.ResponseDetails.Message.Should().Be(ResponseMessage.InvalidCase.GetDescription());
        }

        [Fact]
        public void CreateCaseAsync_Unauthorized_ShouldReturnUnauthorized()
        {
            //Arrange
            var expectedResponseBody = JsonSerializer.Serialize(new BaseResponse().AsError(ResponseMessage.UserIsNotAuthenticated), _serializeOptions);
            var mockHttpRequest = new MockHttpMessageHandler();
            mockHttpRequest.When(_fixture.Cases.CreateCaseUrl)
                           .Respond(HttpStatusCode.Unauthorized, "application/json", expectedResponseBody);
            var mockClient = mockHttpRequest.ToHttpClient();
            _clientFactory.CreateClient(ClientExtensions.CasesClientName).Returns(mockClient);

            var sut = new CasesClient(_retryPolicy, _serializeOptions, _clientFactory, _configuration, _logger);

            //Act
            var response = sut.CreateCaseAsync(_fixture.Cases.GenerateSingleCreateCaseViewModel(), _fixture.Authorization.GenerateFakeJwtToken(), CancellationToken.None).Result;

            //Assert
            response.Should().NotBeNull();
            response.Success.Should().BeFalse();
            response.ResponseDetails.Message.Should().Be(ResponseMessage.UserIsNotAuthenticated.GetDescription());
        }

        [Fact]
        public void CreateCaseAsync_ValidRequest_ShouldReturnSuccessAndValidToken()
        {
            //Arrange
            var officerId = Guid.NewGuid();
            var expectedResponseBody = JsonSerializer.Serialize(new BaseResponse().AsSuccess(), _serializeOptions);
            var mockHttpRequest = new MockHttpMessageHandler();
            mockHttpRequest.When(_fixture.Cases.CreateCaseUrl)
                           .Respond(HttpStatusCode.OK, "application/json", expectedResponseBody);
            var mockClient = mockHttpRequest.ToHttpClient();
            _clientFactory.CreateClient(ClientExtensions.CasesClientName).Returns(mockClient);

            var sut = new CasesClient(_retryPolicy, _serializeOptions, _clientFactory, _configuration, _logger);

            //Act
            var response = sut.CreateCaseAsync(_fixture.Cases.GenerateSingleCreateCaseViewModel(), _fixture.Authorization.GenerateFakeJwtToken(), CancellationToken.None).Result;

            //Assert
            response.Should().NotBeNull();
            response.Success.Should().BeTrue();
        }
    }
}
