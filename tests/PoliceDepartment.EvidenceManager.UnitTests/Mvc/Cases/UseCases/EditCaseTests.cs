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
    public class EditCaseTests
    {
        private readonly MvcFixture _fixture;

        private readonly ICasesClient _client;
        private readonly ILoggerManager _logger;
        private readonly IOfficerUser _officerUser;

        public EditCaseTests(MvcFixture fixure)
        {
            _fixture = fixure;

            _client = Substitute.For<ICasesClient>();
            _logger = Substitute.For<ILoggerManager>();
            _officerUser = Substitute.For<IOfficerUser>();
        }

        [Fact]
        public void RunAsync_InvalidRequest_ShouldReturnError()
        {
            //Arrange
            var viewModel = _fixture.Cases.GenerateSingleViewModel();
            _officerUser.Id.Returns(Guid.NewGuid());
            _officerUser.AccessToken.Returns(_fixture.Authorization.GenerateFakeJwtToken());
            _client.EditAsync(Arg.Any<Guid>(), Arg.Any<CaseViewModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                    .Returns(Task.FromResult(new BaseResponse().AsError()));

            var sut = new EditCase(_client, _logger, _officerUser);

            //Act
            var response = sut.RunAsync(Guid.NewGuid(), viewModel, CancellationToken.None).Result;

            //Assert
            response.Success.Should().BeFalse();
        }

        [Fact]
        public void RunAsync_ValidRequest_ShouldReturnSuccess()
        {
            //Arrange
            var viewModel = _fixture.Cases.GenerateSingleViewModel();
            _officerUser.Id.Returns(Guid.NewGuid());
            _officerUser.AccessToken.Returns(_fixture.Authorization.GenerateFakeJwtToken());
            _client.EditAsync(Arg.Any<Guid>(), Arg.Any<CaseViewModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                    .Returns(Task.FromResult(new BaseResponse().AsSuccess()));

            var sut = new EditCase(_client, _logger, _officerUser);

            //Act
            var response = sut.RunAsync(Guid.NewGuid(), viewModel, CancellationToken.None).Result;

            //Assert
            response.Success.Should().BeTrue();
        }
    }
}
