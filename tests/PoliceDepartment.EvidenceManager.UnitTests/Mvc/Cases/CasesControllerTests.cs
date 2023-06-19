using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using PoliceDepartment.EvidenceManager.MVC.Authorization.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Cases.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Controllers;
using PoliceDepartment.EvidenceManager.MVC.Models;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;
using PoliceDepartment.EvidenceManager.UnitTests.Fixtures.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace PoliceDepartment.EvidenceManager.UnitTests.Mvc.Cases
{
    [Collection(nameof(MvcFixtureCollection))]
    public class CasesControllerTests
    {
        private readonly MvcFixture _fixture;

        private readonly ILoggerManager _logger;
        private readonly IOfficerUser _officerUser;
        private readonly IGetCasesByOfficerId _getCasesByOfficerId;
        private readonly ICreateCase _createCase;
        private readonly IGetCaseDetails _getCaseDetails;

        public CasesControllerTests(MvcFixture fixture)
        {
            _fixture = fixture;

            _logger = Substitute.For<ILoggerManager>();
            _officerUser = Substitute.For<IOfficerUser>();
            _getCasesByOfficerId = Substitute.For<IGetCasesByOfficerId>();
            _createCase = Substitute.For<ICreateCase>();
            _getCaseDetails = Substitute.For<IGetCaseDetails>();
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

            var sut = new CasesController(_logger, _officerUser, _getCasesByOfficerId, _createCase, _getCaseDetails);

            //Act
            var response = sut.Index(CancellationToken.None).Result as ViewResult;
            var responseModel = response?.Model as CasesPageModel;

            //Assert
            response?.Model.Should().NotBeNull();
            responseModel?.Cases.Should().NotBeNull();
            responseModel?.Cases.Count().Should().Be(caseQuantity);
        }

        [Theory]
        [InlineData("","description fake", "Name is required")]
        [InlineData(null,"description fake", "Name is required")]
        [InlineData(" ","description fake", "Name is required")]
        [InlineData("fake name","", "Description is required")]
        [InlineData("fake name",null, "Description is required")]
        [InlineData("fake name"," ", "Description is required")]
        [InlineData("", "", "Name is required", "Description is required")]
        [InlineData(null, "", "Name is required", "Description is required")]
        [InlineData(" ", "", "Name is required", "Description is required")]
        [InlineData("", null, "Name is required", "Description is required")]
        [InlineData(null, null, "Name is required", "Description is required")]
        [InlineData(" ", null, "Name is required", "Description is required")]
        [InlineData("", " ", "Name is required", "Description is required")]
        [InlineData(null, " ", "Name is required", "Description is required")]
        [InlineData(" ", " ", "Name is required", "Description is required")]
        public void CreateModelState_InvalidModel_ShouldReturnInvalid(string name, string description, params string[] errorMessages)
        {
            //Arrange
            var sut = new CreateCasePageViewModel { Name = name, Description = description };
            var context = new ValidationContext(sut, null, null);
            var results = new List<ValidationResult>();
            TypeDescriptor.AddProviderTransparent(new AssociatedMetadataTypeTypeDescriptionProvider(typeof(CreateCasePageViewModel), typeof(CreateCasePageViewModel)), typeof(CreateCasePageViewModel));

            //Act
            var response = Validator.TryValidateObject(sut, context, results, true);

            //Assert
            response.Should().BeFalse();
            if (errorMessages is not null && errorMessages.Any())
            {
                results.Select(r => r.ErrorMessage).Should().Contain(errorMessages);
                results.Count.Should().Be(errorMessages.Length);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void PostCreate_AllResponses_ShouldReturnAllResponses(bool success)
        {
            //Arrange
            var expectedResponse = new BaseResponse();
            _createCase.RunAsync(Arg.Any<CreateCasePageViewModel>(), Arg.Any<CancellationToken>())
                       .Returns(Task.FromResult(success ? expectedResponse.AsSuccess() : expectedResponse.AsError()));

            var viewModel = new CreateCasePageViewModel
            {
                Name = "Fake name",
                Description = "Fake description"
            };
            var sut = new CasesController(_logger, _officerUser, _getCasesByOfficerId, _createCase, _getCaseDetails);

            //Act & Assert
            if (success)
            {
                var response = sut.PostCreate(viewModel, CancellationToken.None).Result as RedirectToActionResult;
                response?.ActionName.Should().Be("Index");
                response?.ControllerName.Should().Be("Home");
            }
            else
            {
                var response = sut.PostCreate(viewModel, CancellationToken.None).Result as ViewResult;
                response?.ViewData.ModelState.Should().NotBeEmpty();
            }
        }

        [Theory]
        [InlineData("00000000-0000-0000-0000-000000000000", true, false, false)]
        [InlineData("24553cbd-21fa-48e1-9531-7aea07a5788d", false, false, false)]
        [InlineData("24553cbd-21fa-48e1-9531-7aea07a5788d", true, false, false)]
        [InlineData("24553cbd-21fa-48e1-9531-7aea07a5788d", true, true, false)]
        [InlineData("24553cbd-21fa-48e1-9531-7aea07a5788d", true, true, true)]
        public void Details_AllCases_ShouldReturnExpectedResponse(string id, bool success, bool valueIsNotNull, bool valueIsValid)
        {
            //Arrang
            var expectedResponse = new BaseResponseWithValue<CaseViewModel>
            {
                Success = success,
            };
            var value = _fixture.Cases.GenerateSingleViewModel();
            if (!valueIsValid) value.Id = Guid.Empty;
            if (valueIsNotNull) expectedResponse.Value = _fixture.Cases.GenerateSingleViewModel();

            _getCaseDetails.RunAsync(Arg.Any<Guid>(),Arg.Any<CancellationToken>())
                            .Returns(expectedResponse);

            var sut = new CasesController(_logger, _officerUser, _getCasesByOfficerId, _createCase, _getCaseDetails);

            //Act & Assert
            if(success && valueIsNotNull && valueIsValid)
            {
                var response = sut.Details(Guid.Parse(id), CancellationToken.None).Result as ViewResult;
                response?.Model.Should().Be(expectedResponse.Value);
            }
            else
            {
                var response = sut.Details(Guid.Parse(id), CancellationToken.None).Result as RedirectToActionResult;
                response?.ActionName.Should().Be("Error");
                response?.ControllerName.Should().Be("Home");
            }
        }
    }
}
