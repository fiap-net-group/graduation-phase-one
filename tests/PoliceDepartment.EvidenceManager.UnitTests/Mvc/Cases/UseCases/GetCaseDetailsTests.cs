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
    public class GetCaseDetailsTests
    {
        private readonly MvcFixture _fixture;

        private readonly ICasesClient _client;
        private readonly ILoggerManager _logger;
        private readonly IOfficerUser _officerUser;


        public GetCaseDetailsTests(MvcFixture fixture)
        {
            _fixture = fixture;

            _client = Substitute.For<ICasesClient>();
            _logger = Substitute.For<ILoggerManager>();
            _officerUser = Substitute.For<IOfficerUser>();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void RunAsync_AllCases_ShouldReturnExpectedResponse(bool successApiResponse)
        {
            //Arrange
            var expectedResponse = successApiResponse ?
                                   new BaseResponseWithValue<CaseViewModel>().AsSuccess(_fixture.Cases.GenerateSingleViewModel()) :
                                   new BaseResponseWithValue<CaseViewModel>().AsError();
            _client.GetDetailsAsync(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                    .Returns(Task.FromResult(expectedResponse));
            _officerUser.Id.Returns(Guid.NewGuid());
            _officerUser.AccessToken.Returns(_fixture.Authorization.GenerateFakeJwtToken());

            var sut = new GetCaseDetails(_client, _logger, _officerUser);

            //Act
            var response = sut.RunAsync(Guid.NewGuid(), CancellationToken.None).Result;

            //Assert
            response.Success.Should().Be(successApiResponse);
        }
    }
}
