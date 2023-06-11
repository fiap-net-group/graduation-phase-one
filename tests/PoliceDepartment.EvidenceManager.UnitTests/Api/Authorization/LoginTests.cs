using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using PoliceDepartment.EvidenceManager.Application.Authorization.UseCases;
using PoliceDepartment.EvidenceManager.Domain.Authorization;
using PoliceDepartment.EvidenceManager.Domain.Database;
using PoliceDepartment.EvidenceManager.Domain.Exceptions;
using PoliceDepartment.EvidenceManager.Domain.Officer;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
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
        [InlineData("", "", true)]
        [InlineData(null, null, true)]
        [InlineData("", null, true)]
        [InlineData(null, "", true)]
        [InlineData("unexistentUsername@email.com", "", true)]
        [InlineData("", "unexistent123", true)]
        [InlineData("unexistentUsername@email.com", "unexistent123", true)]
        [InlineData("unexistentUsername@email.com", "unexistent123", false)]
        public async Task RunAsync_InvalidCredentials_ShouldReturnError(string username, string password, bool officerExists)
        {
            //Arrange
            var logger = Substitute.For<ILoggerManager>();
            var identityManager = Substitute.For<IIdentityManager>();
            identityManager.AuthenticateAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(new AccessTokenModel());
            var mapper = Substitute.For<IMapper>();
            var uow = Substitute.For<IUnitOfWork>();
            uow.Officer.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                              .Returns(Task.FromResult(officerExists ? _fixture.Officer.GenerateSingleEntity() : new OfficerEntity()));
            var configuration = Substitute.For<IConfiguration>();

            var sut = new Login(logger, identityManager, mapper, uow, configuration);

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
            var uow = Substitute.For<IUnitOfWork>();
            var mapper = Substitute.For<IMapper>();
            var identityManager = Substitute.For<IIdentityManager>();
            var configuration = Substitute.For<IConfiguration>();

            var sut = new Login(logger, identityManager, mapper, uow, configuration);

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
            identityManager.AuthenticateAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new AccessTokenModel("Bearer", _fixture.Authorization.GenerateFakeJwtToken(), DateTime.Now.AddDays(10), Guid.NewGuid().ToString()));
            var mapper = Substitute.For<IMapper>();
            var uow = Substitute.For<IUnitOfWork>();
            uow.Officer.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                              .Returns(Task.FromResult(_fixture.Officer.GenerateSingleEntity()));
            var configuration = Substitute.For<IConfiguration>();

            var sut = new Login(logger, identityManager, mapper, uow, configuration);

            //Act
            var response = await sut.RunAsync(new LoginViewModel("username@email.com", "password123"), CancellationToken.None);

            //Assert
            response.Success.Should().BeTrue();
        }
    }
}
