using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using PoliceDepartment.EvidenceManager.MVC.Client;
using PoliceDepartment.EvidenceManager.MVC.Evidences;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.UnitTests.Fixtures.Mvc;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PoliceDepartment.EvidenceManager.UnitTests.Mvc.Evidences
{
    [Collection(nameof(MvcFixtureCollection))]
    public class EvidenceClientTests
    {
        private readonly MvcFixture _fixture;

        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
        private readonly JsonSerializerOptions _serializeOptions;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILoggerManager _logger;

        public EvidenceClientTests(MvcFixture fixture)
        {
            _fixture= fixture;

            _retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(new[]
                { 
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10)
                }, (outcome, timespan, retryCount, context) =>
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"Attempt: {retryCount}!");
                    Console.ForegroundColor = ConsoleColor.White;
                });

            _serializeOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            _clientFactory = Substitute.For<IHttpClientFactory>();
            _configuration = Substitute.For<IConfiguration>();
            _configuration["Api:Evidences:Endpoints:CreateEvidence"].Returns(_fixture.Evidences.CreateEvidenceUrl);
            _configuration["Api:Evidences:Endpoints:Delete"].Returns(_fixture.Evidences.DeleteUrl);
            _logger = Substitute.For<ILoggerManager>();
        }

        [Theory]
        [InlineData(HttpStatusCode.InternalServerError, ResponseMessage.GenericError)]
        [InlineData(HttpStatusCode.Unauthorized, ResponseMessage.UserIsNotAuthenticated)]
        [InlineData(HttpStatusCode.BadRequest, ResponseMessage.CaseAlreadyExists)]
        [InlineData(HttpStatusCode.BadRequest, ResponseMessage.InvalidCase)]
        [InlineData(HttpStatusCode.OK, ResponseMessage.Success)]
        public void CreateEvidenceAsync_AllEvidences_ShouldReturnExpectedResponse(HttpStatusCode code,ResponseMessage responseMessage)
        {
            var responseModel = responseMessage == ResponseMessage.Success ? new BaseResponse().AsSuccess() : new BaseResponse().AsError();
            
            var expectedResponseBody = JsonSerializer.Serialize(responseModel, _serializeOptions);
            var mockHttpRequest = new MockHttpMessageHandler();

            mockHttpRequest.When(_fixture.Evidences.CreateEvidenceUrl)
                .Respond(code, "application/json", expectedResponseBody);

            var mockClient = mockHttpRequest.ToHttpClient();
            _clientFactory.CreateClient(ClientExtensions.EvidencesClientName).Returns(mockClient);

            var sut = new EvidencesClient(_retryPolicy, _serializeOptions, _clientFactory, _configuration, _logger);

            var response = sut.CreateEvidenceAsync(
                _fixture.Evidences.GenerateSingleCreateViewModel(),
                _fixture.Authorization.GenerateFakeJwtToken(), CancellationToken.None).Result;

            response.Should().NotBeNull();
            response.Success.Should().Be(responseMessage == ResponseMessage.Success);
            response.ResponseDetails.Message.Should().Be(responseModel.ResponseDetails.Message);
        }

        [Theory]
        [InlineData(HttpStatusCode.InternalServerError)]
        [InlineData(HttpStatusCode.Unauthorized)]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.NotFound)]
        [InlineData(HttpStatusCode.NoContent)]
        public void DeleteAsync_AllEvidences_ShouldReturnExpectedResponse(HttpStatusCode code)
        {
            //Arrange
            var expectedResponseBody = JsonSerializer.Serialize(new HttpResponseMessage(code), _serializeOptions);
            var mockHttpRequest = new MockHttpMessageHandler();
            mockHttpRequest.When(_fixture.Evidences.DeleteUrl)
                .Respond(code, "application/json", expectedResponseBody);
            var mockClient = mockHttpRequest.ToHttpClient();
            _clientFactory.CreateClient(ClientExtensions.EvidencesClientName).Returns(mockClient);

            var sut = new EvidencesClient(_retryPolicy, _serializeOptions, _clientFactory, _configuration, _logger);

            //Act
            var response = sut.DeleteAsync(Guid.NewGuid(),_fixture.Authorization.GenerateFakeJwtToken(), CancellationToken.None).Result;

            //Assert
            response.Should().NotBeNull();
            if(code == HttpStatusCode.NoContent)
                response.Success.Should().BeTrue();
            else
                response.Success.Should().BeFalse();
        }

    }
}
