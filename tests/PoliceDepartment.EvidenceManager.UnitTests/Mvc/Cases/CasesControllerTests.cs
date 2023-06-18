using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using PoliceDepartment.EvidenceManager.MVC.Authorization;
using PoliceDepartment.EvidenceManager.MVC.Authorization.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Cases.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Controllers;
using PoliceDepartment.EvidenceManager.MVC.Models;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;
using PoliceDepartment.EvidenceManager.UnitTests.Fixtures.Mvc;

namespace PoliceDepartment.EvidenceManager.UnitTests.Mvc.Cases
{
    [Collection(nameof(MvcFixtureCollection))]
    public class CasesControllerTests
    {
        private readonly MvcFixture _fixture;

        private readonly ILoggerManager _logger;
        private readonly IGetCasesByOfficerId _getCasesByOfficerId;
        private readonly IOfficerUser _officerUser;

        public CasesControllerTests(MvcFixture fixture)
        {
            _fixture = fixture;

            _logger = Substitute.For<ILoggerManager>();
            _getCasesByOfficerId = Substitute.For<IGetCasesByOfficerId>();
            _officerUser = Substitute.For<IOfficerUser>();
        }

        [Theory]
        [InlineData(true, 0)]
        [InlineData(true, 3)]
        [InlineData(false, 0)]
        public void Index_AllUseCases_ShouldReturnExpected(bool success, int caseQuantity)
        {
            //Arrange
            var expectedResponse = new BaseResponseWithValue<IEnumerable<CaseViewModel>>();
            _getCasesByOfficerId.RunAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                                .Returns(success ? expectedResponse.AsSuccess(_fixture.Cases.GenerateViewModelCollection(caseQuantity)) : expectedResponse.AsError());
            _officerUser.Id.Returns(Guid.Empty);

            var sut = new CasesController(_logger, _officerUser, _getCasesByOfficerId);

            //Act
            var response = sut.Index(CancellationToken.None).Result as ViewResult;
            var responseModel = response?.Model as CasesPageModel;

            //Assert
            response?.Model.Should().NotBeNull();
            responseModel?.Cases.Should().NotBeNull();
            responseModel?.Cases.Count().Should().Be(caseQuantity);
        }
    }
}
