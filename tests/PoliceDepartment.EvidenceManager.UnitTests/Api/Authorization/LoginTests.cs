using FluentAssertions;
using NSubstitute;
using PoliceDepartment.EvidenceManager.Application.Authorization.UseCases;
using PoliceDepartment.EvidenceManager.Domain.Authorization;
using PoliceDepartment.EvidenceManager.Domain.Exceptions;
using PoliceDepartment.EvidenceManager.Domain.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;
using PoliceDepartment.EvidenceManager.UnitTests.Fixtures.Api;

namespace PoliceDepartment.EvidenceManager.UnitTests.Api.Authorization
{
    [Collection(nameof(ApiFixtureCollection))]
    public class LoginTests
    {
        private readonly ApiFixture _fixture;

        public LoginTests(ApiFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [InlineData("", "")]
        [InlineData(null, null)]
        [InlineData("", null)]
        [InlineData(null, "")]
        [InlineData("unexistentUsername@email.com", "")]
        [InlineData("", "unexistent123")]
        [InlineData("unexistentUsername@email.com", "unexistent123")]
        public async Task RunAsync_InvalidCredentials_ShouldReturnError(string username, string password)
        {
            //Arrange
            var logger = Substitute.For<ILoggerManager>();
            var identityManager = Substitute.For<IIdentityManager>();
            identityManager.AuthenticateAsync(username, password).Returns(new AccessTokenModel());

            var sut = new Login(logger, identityManager);

            //Act
            var response = await sut.RunAsync(new LoginViewModel(username, password), CancellationToken.None);

            //Assert
            response.Success.Should().BeFalse();
            response.ResponseMessageEqual(ResponseMessage.InvalidCredentials);
        }

        [Fact]
        public void RunAsync_NullViewModel_ShouldThrow()
        {
            //Arrange
            var logger = Substitute.For<ILoggerManager>();
            var identityManager = Substitute.For<IIdentityManager>();
            var sut = new Login(logger, identityManager);

            //Act
            var act = () => sut.RunAsync(default, CancellationToken.None);

            //Assert
            act.Should().ThrowExactlyAsync<BusinessException>().WithMessage("Invalid parameter");
        }

        [Fact]
        public async Task RunAsync_ExistingCredentials_ShouldReturnSuccess()
        {
            //Arrange
            var logger = Substitute.For<ILoggerManager>();
            var identityManager = Substitute.For<IIdentityManager>();
            identityManager.AuthenticateAsync(Arg.Any<string>(), Arg.Any<string>())
            .Returns(new AccessTokenModel("Bearer", _fixture.Authorization.GenerateFakeJwtToken(), DateTime.Now.AddDays(10), Guid.NewGuid().ToString()));

            var sut = new Login(logger, identityManager);

            //Act
            var response = await sut.RunAsync(new LoginViewModel("username@email.com", "password123"), CancellationToken.None);

            //Assert
            response.Success.Should().BeTrue();
        }
    }
}
