using FluentAssertions;
using NSubstitute;
using PoliceDepartment.EvidenceManager.MVC.Authorization.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Cases.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Cases.UseCases;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;
using PoliceDepartment.EvidenceManager.UnitTests.Fixtures.Mvc;

namespace PoliceDepartment.EvidenceManager.UnitTests.Mvc.Cases.UseCases
{
    [Collection(nameof(MvcFixtureCollection))]
    public class GetCasesByOfficerIdTests
    {
        private readonly MvcFixture _fixture;

        private readonly ICasesClient _client;
        private readonly ILoggerManager _logger;
        private readonly IOfficerUser _officerUser;

        public GetCasesByOfficerIdTests(MvcFixture fixture)
        {
            _fixture = fixture;

            _client = Substitute.For<ICasesClient>();
            _logger = Substitute.For<ILoggerManager>();
            _officerUser = Substitute.For<IOfficerUser>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(10)]
        public void RunAsync_ValidRequest_ShouldReturnCasesCollection(int quantity)
        {
            //Arrange
            var expectedResponse = new BaseResponseWithValue<IEnumerable<CaseViewModel>>().AsSuccess(_fixture.Cases.GenerateViewModelCollection(quantity));
            _client.GetByOfficerIdAsync(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(expectedResponse);
            _officerUser.AccessToken.Returns(_fixture.Authorization.GenerateFakeJwtToken());

            var sut = new GetCasesByOfficerId(_client, _logger, _officerUser);

            //Act
            var response = sut.RunAsync(Guid.NewGuid(), CancellationToken.None).Result;

            //Assert
            response.Success.Should().BeTrue();
            response.Value.Count().Should().Be(quantity);
        }

        [Fact]
        public void RunAsync_InvalidRequest_ShouldErrorResponse()
        {
            //Arrange
            var expectedResponse = new BaseResponseWithValue<IEnumerable<CaseViewModel>>().AsError(ResponseMessage.UserIsNotAuthenticated);
            _client.GetByOfficerIdAsync(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(expectedResponse);
            _officerUser.AccessToken.Returns(_fixture.Authorization.GenerateFakeJwtToken());

            var sut = new GetCasesByOfficerId(_client, _logger, _officerUser);

            //Act
            var response = sut.RunAsync(Guid.NewGuid(), CancellationToken.None).Result;

            //Assert
            response.Success.Should().BeFalse();
        }
    }
}
