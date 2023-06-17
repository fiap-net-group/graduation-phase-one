using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using NSubstitute;
using PoliceDepartment.EvidenceManager.MVC.Authorization;
using PoliceDepartment.EvidenceManager.MVC.Authorization.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Authorization.UseCases;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.UnitTests.Fixtures.Mvc;
using System.Threading;

namespace PoliceDepartment.EvidenceManager.UnitTests.Mvc.Authorization.UseCases
{
    [Collection(nameof(MvcFixtureCollection))]
    public class LogoutTests
    {
        private readonly MvcFixture _fixture;

        private readonly ILoggerManager _logger;
        private readonly IAuthorizationClient _client;
        private readonly IAuthenticationService _authService;
        private readonly IOfficerUser _officerUser;

        public LogoutTests(MvcFixture fixture)
        {
            _fixture = fixture;

            _logger = Substitute.For<ILoggerManager>();
            _client = Substitute.For<IAuthorizationClient>();
            _authService = Substitute.For<IAuthenticationService>();
            _officerUser = Substitute.For<IOfficerUser>();
        }

        [Fact]
        public void RunAsync_UserUnauthorized_ShouldReturnError()
        {
            //Arrange
            _officerUser.IsAuthenticated.Returns(false);

            var sut = new Logout(_logger,_client, _authService,_officerUser);

            //Act
            var response = sut.RunAsync(CancellationToken.None).Result;

            //Assert
            response.Success.Should().BeFalse();
            response.ResponseMessageEqual(ResponseMessage.UserIsNotAuthenticated);
        }

        [Fact]
        public void RunAsync_ValidUser_ShouldReturnSuccess()
        {
            //Arrange
            _officerUser.IsAuthenticated.Returns(true);
            _officerUser.AccessToken.Returns(_fixture.Authorization.GenerateFakeJwtToken());
            _client.SignOutAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(new BaseResponse().AsSuccess());

            var sut = new Logout(_logger, _client, _authService, _officerUser);

            //Act
            var response = sut.RunAsync(CancellationToken.None).Result;

            //Assert
            response.Success.Should().BeTrue();
        }
    }
}
