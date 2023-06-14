using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using PoliceDepartment.EvidenceManager.MVC.Authorization;
using PoliceDepartment.EvidenceManager.MVC.Client;
using PoliceDepartment.EvidenceManager.SharedKernel.Extensions;
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

namespace PoliceDepartment.EvidenceManager.UnitTests.Mvc.Authorization
{
    [Collection(nameof(MvcFixtureCollection))]
    public class AuthorizationClientTests
    {
        private readonly MvcFixture _fixture;

        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
        private readonly JsonSerializerOptions _serializeOptions;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILoggerManager _logger;

        public AuthorizationClientTests(MvcFixture fixture)
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
            _configuration["Api:Authorization:Endpoints:Login"].Returns(_fixture.Authorization.LoginUrl);
            _configuration["Api:Authorization:Endpoints:Logout"].Returns(_fixture.Authorization.LogoutUrl);
            _logger = Substitute.For<ILoggerManager>();
        }

        [Fact]
        public void SignInAsync_ServerIsOff_SholdReturnGenericError()
        {
            //Arrange
            var mockHttpRequest = new MockHttpMessageHandler();
            mockHttpRequest.When(_fixture.Authorization.LoginUrl)
                           .Respond(HttpStatusCode.InternalServerError, "application/json", "{'error' : 'An error ocurred'}");
            var mockClient = mockHttpRequest.ToHttpClient();
            _clientFactory.CreateClient(ClientExtensions.AuthorizationClientName).Returns(mockClient);

            var sut = new AuthorizationClient(_retryPolicy, _serializeOptions, _clientFactory, _configuration, _logger);

            //Act
            var response = sut.SignInAsync("username","password123",CancellationToken.None).Result;

            //Assert
            response.Should().NotBeNull();
            response.Success.Should().BeFalse();
            response.ResponseDetails.Message.Should().Be("An error ocurred, try again later");
        }


        [Fact]
        public void SignInAsync_InvalidRequest_SholdReturnBadRequest()
        {
            //Arrange
            var expectedResponseBody = JsonSerializer.Serialize(new BaseResponseWithValue<AccessTokenViewModel>().AsError(ResponseMessage.InvalidCredentials), _serializeOptions);
            var mockHttpRequest = new MockHttpMessageHandler();
            mockHttpRequest.When(_fixture.Authorization.LoginUrl)
                           .Respond(HttpStatusCode.BadRequest, "application/json", expectedResponseBody);
            var mockClient = mockHttpRequest.ToHttpClient();
            _clientFactory.CreateClient(ClientExtensions.AuthorizationClientName).Returns(mockClient);

            var sut = new AuthorizationClient(_retryPolicy, _serializeOptions, _clientFactory, _configuration, _logger);

            //Act
            var response = sut.SignInAsync("username", "password123", CancellationToken.None).Result;

            //Assert
            response.Should().NotBeNull();
            response.Success.Should().BeFalse();
            response.ResponseDetails.Message.Should().Be(ResponseMessage.InvalidCredentials.GetDescription());
        }

        [Fact]
        public void SignInAsync_ValidRequest_SholdReturnSuccessAndValidToken()
        {
            //Arrange
            var expectedResponseBody = JsonSerializer.Serialize(new BaseResponseWithValue<AccessTokenViewModel>().AsSuccess(_fixture.Authorization.GenerateViewModel(valid: true)), _serializeOptions);
            var mockHttpRequest = new MockHttpMessageHandler();
            mockHttpRequest.When(_fixture.Authorization.LoginUrl)
                           .Respond(HttpStatusCode.OK, "application/json", expectedResponseBody);
            var mockClient = mockHttpRequest.ToHttpClient();
            _clientFactory.CreateClient(ClientExtensions.AuthorizationClientName).Returns(mockClient);

            var sut = new AuthorizationClient(_retryPolicy, _serializeOptions, _clientFactory, _configuration, _logger);

            //Act
            var response = sut.SignInAsync("username", "password123", CancellationToken.None).Result;

            //Assert
            response.Should().NotBeNull();
            response.Success.Should().BeTrue();
        }

        [Fact]
        public void SignOutAsync_ServerIsOff_SholdReturnGenericError()
        {
            //Arrange
            var mockHttpRequest = new MockHttpMessageHandler();
            mockHttpRequest.When(_fixture.Authorization.LogoutUrl)
                           .Respond(HttpStatusCode.InternalServerError, "application/json", "{'error' : 'An error ocurred'}");
            var mockClient = mockHttpRequest.ToHttpClient();
            _clientFactory.CreateClient(ClientExtensions.AuthorizationClientName).Returns(mockClient);

            var sut = new AuthorizationClient(_retryPolicy, _serializeOptions, _clientFactory, _configuration, _logger);

            //Act
            var response = sut.SignOutAsync(_fixture.Authorization.GenerateFakeJwtToken(), CancellationToken.None).Result;

            //Assert
            response.Should().NotBeNull();
            response.Success.Should().BeFalse();
            response.ResponseDetails.Message.Should().Be("An error ocurred, try again later");
        }


        [Fact]
        public void SignOutAsync_Unauthorized_SholdReturnUnauthorized()
        {
            //Arrange
            var expectedResponseBody = JsonSerializer.Serialize(new BaseResponseWithValue<AccessTokenViewModel>().AsError(ResponseMessage.UserIsNotAuthenticated), _serializeOptions);
            var mockHttpRequest = new MockHttpMessageHandler();
            mockHttpRequest.When(_fixture.Authorization.LogoutUrl)
                           .Respond(HttpStatusCode.Unauthorized, "application/json", expectedResponseBody);
            var mockClient = mockHttpRequest.ToHttpClient();
            _clientFactory.CreateClient(ClientExtensions.AuthorizationClientName).Returns(mockClient);

            var sut = new AuthorizationClient(_retryPolicy, _serializeOptions, _clientFactory, _configuration, _logger);

            //Act
            var response = sut.SignOutAsync(_fixture.Authorization.GenerateFakeJwtToken(), CancellationToken.None).Result;

            //Assert
            response.Should().NotBeNull();
            response.Success.Should().BeFalse();
            response.ResponseDetails.Message.Should().Be(ResponseMessage.UserIsNotAuthenticated.GetDescription());
        }

        [Fact]
        public void SignOutAsync_ValidRequest_SholdReturnSuccessAndValidToken()
        {
            //Arrange
            var expectedResponseBody = JsonSerializer.Serialize(new BaseResponse().AsSuccess(), _serializeOptions);
            var mockHttpRequest = new MockHttpMessageHandler();
            mockHttpRequest.When(_fixture.Authorization.LogoutUrl)
                           .Respond(HttpStatusCode.OK, "application/json", expectedResponseBody);
            var mockClient = mockHttpRequest.ToHttpClient();
            _clientFactory.CreateClient(ClientExtensions.AuthorizationClientName).Returns(mockClient);

            var sut = new AuthorizationClient(_retryPolicy, _serializeOptions, _clientFactory, _configuration, _logger);

            //Act
            var response = sut.SignOutAsync(_fixture.Authorization.GenerateFakeJwtToken(), CancellationToken.None).Result;

            //Assert
            response.Should().NotBeNull();
            response.Success.Should().BeTrue();
        }
    }
}
