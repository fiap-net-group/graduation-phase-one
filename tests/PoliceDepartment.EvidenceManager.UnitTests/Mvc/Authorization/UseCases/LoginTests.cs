using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using NSubstitute;
using PoliceDepartment.EvidenceManager.MVC.Authorization.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Authorization.UseCases;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;
using PoliceDepartment.EvidenceManager.UnitTests.Fixtures.Mvc;

namespace PoliceDepartment.EvidenceManager.UnitTests.Mvc.Authorization.UseCases
{
    [Collection(nameof(MvcFixtureCollection))]
    public class LoginTests
    {
        private readonly MvcFixture _fixture;

        private readonly IAuthenticationService _authService;
        private readonly IOfficerUser _officerUser;
        private readonly IAuthorizationClient _client;
        private readonly ILoggerManager _logger;

        public LoginTests(MvcFixture fixture)
        {
            _fixture = fixture;
            
            _authService = Substitute.For<IAuthenticationService>();
            _officerUser = Substitute.For<IOfficerUser>();
            _client = Substitute.For<IAuthorizationClient>();
            _logger = Substitute.For<ILoggerManager>();
        }

        [Theory]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(false, false)]
        public async Task RunAsync_InvalidAuthResponse_ShouldReturnErrorsAsync(bool responseSuccess, bool validAuthToken)
        {
            //Arrange
            var fakeToken = validAuthToken 
                ? new AccessTokenViewModel("Bearer",_fixture.Authorization.GenerateFakeJwtToken(),DateTime.Now.AddDays(1), Guid.NewGuid().ToString())
                : new AccessTokenViewModel();

            var authResponse = new BaseResponseWithValue<AccessTokenViewModel>();

            _client.SignInAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                   .Returns(Task.FromResult(responseSuccess ? authResponse.AsSuccess(fakeToken) : authResponse.AsError(ResponseMessage.InvalidCredentials)));

            var sut = new Login(_authService,_officerUser, _client, _logger);

            //Act
            var response = await sut.RunAsync("username", "password", CancellationToken.None);

            //Assert
            response.Should().NotBeNull();
            response.Success.Should().BeFalse();
        }

        [Fact]
        public async Task RunAsync_ValidRequest_ShouldReturnSuccess()
        {
            //Arrange
            var authResponse = new BaseResponseWithValue<AccessTokenViewModel>()
                .AsSuccess(new AccessTokenViewModel("Bearer", _fixture.Authorization.GenerateFakeJwtToken(), DateTime.Now.AddDays(1), Guid.NewGuid().ToString()));

            _client.SignInAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(authResponse));

            var sut = new Login(_authService, _officerUser, _client, _logger);

            //Act
            var response = await sut.RunAsync("username", "password", CancellationToken.None);

            //Assert
            response.Should().NotBeNull();
            response.Success.Should().BeTrue();
        }
    }
}
