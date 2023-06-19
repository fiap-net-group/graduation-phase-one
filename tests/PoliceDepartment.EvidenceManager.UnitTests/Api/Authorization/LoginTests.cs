using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using PoliceDepartment.EvidenceManager.Application.Authorization.UseCases;
using PoliceDepartment.EvidenceManager.SharedKernel.Authorization;
using PoliceDepartment.EvidenceManager.SharedKernel.Database;
using PoliceDepartment.EvidenceManager.SharedKernel.Exceptions;
using PoliceDepartment.EvidenceManager.SharedKernel.Officer;
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

        private readonly ILoggerManager _logger;
        private readonly IIdentityManager _identityManager;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;
        private readonly IConfiguration _configuration;

        public LoginTests(ApiFixture fixture)
        {
            _fixture = fixture;
            _logger = Substitute.For<ILoggerManager>();
            _identityManager = Substitute.For<IIdentityManager>();
            _mapper = Substitute.For<IMapper>();
            _uow = Substitute.For<IUnitOfWork>();
            _configuration = Substitute.For<IConfiguration>();
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
            _identityManager.AuthenticateAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(new AccessTokenModel());
            _uow.Officer.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                              .Returns(Task.FromResult(officerExists ? _fixture.Officer.GenerateSingleEntity() : new OfficerEntity()));

            var sut = new Login(_logger, _identityManager, _mapper, _uow, _configuration);

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
            var sut = new Login(_logger, _identityManager, _mapper, _uow, _configuration);

            //Act
            var act = () => sut.RunAsync(default, CancellationToken.None);

            //Assert
            act.Should().ThrowExactlyAsync<BusinessException>().WithMessage("Invalid parameter");
        }

        [Fact]
        public async Task RunAsync_ExistingCredentials_ShouldReturnSuccess()
        {
            //Arrange
            _identityManager.AuthenticateAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new AccessTokenModel("Bearer", _fixture.Authorization.GenerateFakeJwtToken(), DateTime.Now.AddDays(10), Guid.NewGuid().ToString()));
            _uow.Officer.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                              .Returns(Task.FromResult(_fixture.Officer.GenerateSingleEntity()));

            var sut = new Login(_logger, _identityManager, _mapper, _uow, _configuration);

            //Act
            var response = await sut.RunAsync(new LoginViewModel("username@email.com", "password123"), CancellationToken.None);

            //Assert
            response.Success.Should().BeTrue();
        }
    }
}
