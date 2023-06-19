using FluentAssertions;
using NSubstitute;
using PoliceDepartment.EvidenceManager.MVC.Authorization.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Cases.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Cases.UseCases;
using PoliceDepartment.EvidenceManager.MVC.Models;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;
using PoliceDepartment.EvidenceManager.UnitTests.Fixtures.Mvc;

namespace PoliceDepartment.EvidenceManager.UnitTests.Mvc.Cases.UseCases
{
    [Collection(nameof(MvcFixtureCollection))]
    public class CreateCaseTests
    {
        private readonly MvcFixture _fixture;

        private readonly ILoggerManager _logger;
        private readonly ICasesClient _client;
        private readonly IOfficerUser _officerUser;

        public CreateCaseTests(MvcFixture fixture)
        {
            _fixture = fixture;

            _logger = Substitute.For<ILoggerManager>();
            _client = Substitute.For<ICasesClient>();
            _officerUser = Substitute.For<IOfficerUser>();
        }

        [Fact]
        public void RunAsync_InvalidRequest_ShouldReturnError()
        {
            //Arrange
            var viewModel = new CreateCasePageViewModel
            {
                Name = "Fake name",
                Description = "Fake description"
            };
            _officerUser.Id.Returns(Guid.NewGuid());
            _officerUser.AccessToken.Returns(_fixture.Authorization.GenerateFakeJwtToken());
            _client.CreateCaseAsync(Arg.Any<CreateCaseViewModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                    .Returns(Task.FromResult(new BaseResponse().AsError()));

            var sut = new CreateCase(_logger, _client, _officerUser);

            //Act
            var response = sut.RunAsync(viewModel, CancellationToken.None).Result;

            //Assert
            response.Success.Should().BeFalse();
        }

        [Fact]
        public void RunAsync_ValidRequest_ShouldReturnSuccess()
        {
            //Arrange
            var viewModel = new CreateCasePageViewModel
            {
                Name = "Fake name",
                Description = "Fake description"
            };
            _officerUser.Id.Returns(Guid.NewGuid());
            _officerUser.AccessToken.Returns(_fixture.Authorization.GenerateFakeJwtToken());
            _client.CreateCaseAsync(Arg.Any<CreateCaseViewModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                    .Returns(Task.FromResult(new BaseResponse().AsSuccess()));

            var sut = new CreateCase(_logger, _client, _officerUser);

            //Act
            var response = sut.RunAsync(viewModel, CancellationToken.None).Result;

            //Assert
            response.Success.Should().BeTrue();
        }
    }
}
