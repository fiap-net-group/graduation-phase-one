using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using PoliceDepartment.EvidenceManager.API.Application.Evidence.UseCases;
using PoliceDepartment.EvidenceManager.Application.Case.UseCases;
using PoliceDepartment.EvidenceManager.Domain.Case;
using PoliceDepartment.EvidenceManager.Domain.Database;
using PoliceDepartment.EvidenceManager.Domain.Evidence;
using PoliceDepartment.EvidenceManager.Domain.Exceptions;
using PoliceDepartment.EvidenceManager.Domain.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;
using PoliceDepartment.EvidenceManager.UnitTests.Fixtures.Api;

namespace PoliceDepartment.EvidenceManager.UnitTests.Api.Evidence
{
    [Collection(nameof(ApiFixtureCollection))]
    public class CreateEvidenceTest
    {
        private readonly ApiFixture _fixture;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateEvidenceViewModel> _validator;
        private readonly ILoggerManager _logger;

        public CreateEvidenceTest(ApiFixture fixture)
        {
            _fixture = fixture;
            _uow = Substitute.For<IUnitOfWork>();
            _mapper = Substitute.For<IMapper>();
            _validator = Substitute.For<IValidator<CreateEvidenceViewModel>>();
            _logger = Substitute.For<ILoggerManager>();
        }

        [Fact]
        public async Task RunAsync_ValidRequest_ShoudlReturnSuccess()
        {
            // Arrange
            CreateEvidenceViewModel evidenceViewModel = _fixture.Evidence.GenerateViewModel();
            var validationResult = new ValidationResult();
            _validator.ValidateAsync(evidenceViewModel, CancellationToken.None).Returns(validationResult);

            var evidenceEntity = _fixture.Evidence.GenerateSingleEntity();
            _mapper.Map<EvidenceEntity>(evidenceViewModel).Returns(evidenceEntity);

            var _uow = Substitute.For<IUnitOfWork>();
            _uow.Evidence.CreateAsync(evidenceEntity, CancellationToken.None).Returns(Task.CompletedTask);
            _uow.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(true);

            var sut = new CreateEvidence(_logger, _uow, _mapper, _validator);

            // Act
            var response = await sut.RunAsync(evidenceViewModel, CancellationToken.None);

            // Assert
            response.Success.Should().BeTrue();
            _mapper.Received(1).Map<EvidenceEntity>(evidenceViewModel);
            await _uow.Evidence.Received(1).CreateAsync(evidenceEntity, CancellationToken.None);
            await _uow.Received(1).SaveChangesAsync(CancellationToken.None);
        }

        [Theory]
        [InlineData(null, null, "", "", ResponseMessage.InvalidEvidence)]
        [InlineData("", null, "", "", ResponseMessage.InvalidEvidence)]
        [InlineData(null, "", "", "", ResponseMessage.InvalidEvidence)]
        [InlineData("", "", "", "", ResponseMessage.InvalidEvidence)]
        public async Task RunAsync_InvalidRequest_ShouldReturnError(string name, string description, Guid imageId, Guid caseId, ResponseMessage expectedResponse)
        {
            // Arrange
            CreateEvidenceViewModel viewModel = new CreateEvidenceViewModel { Name = name, Description = description, CaseId = caseId, ImageId = imageId };

            var validationResult = new ValidationResult();
            validationResult.Errors.Add(new ValidationFailure("Name", "Name is required"));
            validationResult.Errors.Add(new ValidationFailure("Description", "Description is required"));
            _validator.ValidateAsync(viewModel, CancellationToken.None).Returns(validationResult);

            var sut = new CreateEvidence(_logger, _uow, _mapper, _validator);

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
            CreateEvidenceViewModel caseViewModel = _fixture.Evidence.GenerateViewModel();
            var validationResult = new ValidationResult();
            _validator.ValidateAsync(caseViewModel, CancellationToken.None).Returns(validationResult);

            var caseEntity = _fixture.Case.GenerateSingleEntity();
            _mapper.Map<CaseEntity>(caseViewModel).Returns(caseEntity);

            var _uow = Substitute.For<IUnitOfWork>();
            _uow.Case.AddAsync(caseEntity, CancellationToken.None).Returns(Task.CompletedTask);
            _uow.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(false);

            var sut = new CreateEvidence(_logger, _uow, _mapper, _validator);

            //Act
            var act = async () => await sut.RunAsync(caseViewModel, CancellationToken.None);

            //Assert
            await act.Should()
                .ThrowExactlyAsync<InfrastructureException>()
                .WithMessage("An unexpected error ocurred");
        }
    }
}
