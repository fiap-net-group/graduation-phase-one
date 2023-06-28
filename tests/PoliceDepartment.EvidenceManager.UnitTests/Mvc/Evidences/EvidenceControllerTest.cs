using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using NSubstitute;
using PoliceDepartment.EvidenceManager.MVC.Authorization.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Controllers;
using PoliceDepartment.EvidenceManager.MVC.Evidences.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Models;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.UnitTests.Fixtures.Mvc;
using System.Collections.Specialized;
using System.Text;

namespace PoliceDepartment.EvidenceManager.UnitTests.Mvc.Evidences
{
    [Collection(nameof(MvcFixtureCollection))]
    public class EvidenceControllerTest
    {
        private readonly MvcFixture _fixture;

        private readonly ILoggerManager _logger;
        private readonly IOfficerUser _officerUser;
        private readonly ICreateEvidence _createEvidence;
        private readonly IDeleteEvidence _deleteEvidence;
        public EvidenceControllerTest(MvcFixture fixture)
        {
            _fixture = fixture;
            _officerUser = Substitute.For<IOfficerUser>();
            _logger = Substitute.For<ILoggerManager>();
            _createEvidence = Substitute.For<ICreateEvidence>();
            _deleteEvidence = Substitute.For<IDeleteEvidence>();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CreateModelStatus_ShouldReturnSuccess(bool success)
        {
            //Arrange
            var viewModel = new CreateEvidencePageViewModel
            {
                Name = "Fake name",
                Description = "Fake description",
                CaseId = Guid.NewGuid(),
                OfficerId = Guid.NewGuid(),
                Image = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, "Data", "dummy.txt"),
                ImageId = Guid.NewGuid()
            };
            _officerUser.Id.Returns(Guid.NewGuid());
            _officerUser.AccessToken.Returns(_fixture.Authorization.GenerateFakeJwtToken());
            _createEvidence.RunAsync(viewModel, Arg.Any<CancellationToken>())
                    .Returns(Task.FromResult(success ? new BaseResponse().AsSuccess() : new BaseResponse().AsError()));

            var sut = new EvidencesController(_logger, _officerUser, _createEvidence, _fixture.Evidences.webHosting, _deleteEvidence);

            sut.ControllerContext.HttpContext = new DefaultHttpContext();
            sut.ControllerContext.HttpContext.Request.Query = new QueryCollection(new Dictionary<string, StringValues>
            {
                { "returnUrl", "cases/Details/1" }
            });

            //Act & Assert
            if (success)
            {
                var response = sut.PostCreate(viewModel, CancellationToken.None).Result as RedirectResult;
                response?.Url.Should().Be("cases/Details/1");
            }
            else
            {
                var response = sut.PostCreate(viewModel, CancellationToken.None).Result as ViewResult;
                response?.ViewData.ModelState.Should().NotBeEmpty();
            }
        }

        [Theory]
        [InlineData("24553cbd-21fa-48e1-9531-7aea07a5788d", false)]
        [InlineData("24553cbd-21fa-48e1-9531-7aea07a5788d", true)]
        public void Delete_AllEvidences_ShouldReturnExpectedResponse(string id, bool success)
        {
            //Arrange
            var expectedResponse = success ? new BaseResponse().AsSuccess() : new BaseResponse().AsError();
            _deleteEvidence.RunAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(expectedResponse));

            var sut = new EvidencesController(_logger, _officerUser, _createEvidence, _fixture.Evidences.webHosting, _deleteEvidence);
            sut.ControllerContext.HttpContext = new DefaultHttpContext();
            sut.ControllerContext.HttpContext.Request.Query = new QueryCollection(new Dictionary<string, StringValues>
            {
                { "returnUrl", "cases/Details" }
            });

            //Act
            var response = sut.Delete(Guid.Parse(id), CancellationToken.None).Result as RedirectResult;

            if (success)
            {
                response?.Url.Should().Be("cases/Details");
                _logger.Received(1).LogDebug("MVC - Delete evidence - Success", ("officerId", _officerUser.Id), ("evidenceId", Guid.Parse(id)));
            }
            else
            {
                response?.Url.Should().Be("cases/Details");
                _logger.Received(1).LogWarning("MVC - Delete evidence - Error", ("officerId", _officerUser.Id), ("evidenceId", Guid.Parse(id)));
            }
        }
    }
}
