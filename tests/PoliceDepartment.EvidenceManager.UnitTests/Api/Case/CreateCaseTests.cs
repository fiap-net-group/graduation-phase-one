using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using PoliceDepartment.EvidenceManager.Application.Case.UseCases;
using PoliceDepartment.EvidenceManager.Domain.Case;
using PoliceDepartment.EvidenceManager.Domain.Database;
using PoliceDepartment.EvidenceManager.Domain.Exceptions;
using PoliceDepartment.EvidenceManager.Domain.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;
using PoliceDepartment.EvidenceManager.UnitTests.Fixtures.Api;

namespace PoliceDepartment.EvidenceManager.UnitTests.Api.Case
{
    [Collection(nameof(ApiFixtureCollection))]
    public class CreateCaseTests
    {
        private readonly ApiFixture _fixture;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateCaseViewModel> _validator;
        private readonly ILoggerManager _logger;

        public CreateCaseTests(ApiFixture fixture)
        {
            _fixture = fixture;
            _uow = Substitute.For<IUnitOfWork>();
            _mapper = Substitute.For<IMapper>();
            _validator = Substitute.For<IValidator<CreateCaseViewModel>>();
            _logger = Substitute.For<ILoggerManager>();
        }



        [Fact]
        public async Task RunAsync_ValidRequest_ShoudlReturnSuccess()
        {
            // Arrange
            CreateCaseViewModel caseViewModel = _fixture.Case.GenerateViewModel();
            var validationResult = new ValidationResult();
            _validator.ValidateAsync(caseViewModel, CancellationToken.None).Returns(validationResult);

            var caseEntity = _fixture.Case.GenerateSingleEntity();
            _mapper.Map<CaseEntity>(caseViewModel).Returns(caseEntity);

            var _uow = Substitute.For<IUnitOfWork>();
            _uow.Case.AddAsync(caseEntity, CancellationToken.None).Returns(Task.CompletedTask);
            _uow.SaveChangesAsync().Returns(true);

            var sut = new CreateCase(_logger, _uow, _mapper, _validator);

            // Act
            var response = await sut.RunAsync(caseViewModel, CancellationToken.None);

            // Assert
            response.Success.Should().BeTrue();
            _mapper.Received(1).Map<CaseEntity>(caseViewModel);
            await _uow.Case.Received(1).AddAsync(caseEntity, CancellationToken.None);
            await _uow.Received(1).SaveChangesAsync();
        }


        [Theory]
        [InlineData(null, null, ResponseMessage.InvalidCase)]
        [InlineData("", null, ResponseMessage.InvalidCase)]
        [InlineData(null, "", ResponseMessage.InvalidCase)]
        [InlineData("", "", ResponseMessage.InvalidCase)]
        public async Task RunAsync_InvalidRequest_ShouldReturnError(string name, string description, ResponseMessage expectedResponse)
        {
            // Arrange
            var viewModel = new CreateCaseViewModel { Name = name, Description = description };

            var validationResult = new ValidationResult();
            validationResult.Errors.Add(new ValidationFailure("Name", "Name is required"));
            validationResult.Errors.Add(new ValidationFailure("Description", "Description is required"));
            _validator.ValidateAsync(viewModel, CancellationToken.None).Returns(validationResult);

            var sut = new CreateCase(_logger, _uow, _mapper, _validator);

            // Act
            var response = await sut.RunAsync(viewModel, CancellationToken.None);

            // Assert
            response.Success.Should().BeFalse();
            response.ResponseMessageEqual(expectedResponse);

            await _validator.Received(1).ValidateAsync(viewModel, CancellationToken.None);
        }


        [Fact]
        public async Task RunAsync_DatabaseError_ShouldThrow()
        {
            // Arrange
            CreateCaseViewModel caseViewModel = _fixture.Case.GenerateViewModel();
            var validationResult = new ValidationResult();
            _validator.ValidateAsync(caseViewModel, CancellationToken.None).Returns(validationResult);

            var caseEntity = _fixture.Case.GenerateSingleEntity();
            _mapper.Map<CaseEntity>(caseViewModel).Returns(caseEntity);

            var _uow = Substitute.For<IUnitOfWork>();
            _uow.Case.AddAsync(caseEntity, CancellationToken.None).Returns(Task.CompletedTask);
            _uow.SaveChangesAsync().Returns(false);

            var sut = new CreateCase(_logger, _uow, _mapper, _validator);

            //Act
            var act = async () => await sut.RunAsync(caseViewModel, CancellationToken.None);

            //Assert
            await act.Should()
                .ThrowExactlyAsync<InfrastructureException>()
                .WithMessage("An unexpected error ocurred");
        }
    }
}
