using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using PoliceDepartment.EvidenceManager.API.Application.Evidence.UseCases;
using PoliceDepartment.EvidenceManager.Application.Case.UseCases;
using PoliceDepartment.EvidenceManager.SharedKernel.Case;
using PoliceDepartment.EvidenceManager.SharedKernel.Database;
using PoliceDepartment.EvidenceManager.SharedKernel.Evidence;
using PoliceDepartment.EvidenceManager.SharedKernel.Evidence.UseCases;
using PoliceDepartment.EvidenceManager.SharedKernel.Exceptions;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Extensions;
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

            var caseEntity = _fixture.Case.GenerateSingleEntity();
            caseEntity.OfficerId = evidenceViewModel.OfficerId;

            var _uow = Substitute.For<IUnitOfWork>();
            _uow.Case.GetByIdAsync(evidenceViewModel.CaseId, CancellationToken.None).Returns(caseEntity);
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
        [InlineData(null, null, "be2a3f88-e30f-4005-9412-9cc195a14476", "a31a1a7b-5c2d-42b7-aa4d-edfefad77bb9", ResponseMessage.InvalidEvidence)]
        [InlineData("", null, "be2a3f88-e30f-4005-9412-9cc195a14476", "a31a1a7b-5c2d-42b7-aa4d-edfefad77bb9", ResponseMessage.InvalidEvidence)]
        [InlineData(null, "", "be2a3f88-e30f-4005-9412-9cc195a14476", "a31a1a7b-5c2d-42b7-aa4d-edfefad77bb9", ResponseMessage.InvalidEvidence)]
        [InlineData("", "", "be2a3f88-e30f-4005-9412-9cc195a14476", "a31a1a7b-5c2d-42b7-aa4d-edfefad77bb9", ResponseMessage.InvalidEvidence)]
        public async Task RunAsync_InvalidRequest_ShouldReturnError(string name, string description, Guid imageId, Guid caseId, ResponseMessage expectedResponse)
        {
            // Arrange
            CreateEvidenceViewModel viewModel = new() { Name = name, Description = description, CaseId = caseId, ImageId = imageId };

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
            CreateEvidenceViewModel evidenceViewModel = _fixture.Evidence.GenerateViewModel();
            var validationResult = new ValidationResult();
            _validator.ValidateAsync(evidenceViewModel, CancellationToken.None).Returns(validationResult);

            var evidenceEntity = _fixture.Evidence.GenerateSingleEntity();
            _mapper.Map<EvidenceEntity>(evidenceViewModel).Returns(evidenceEntity);
            var caseEntity = _fixture.Case.GenerateSingleEntity();
            caseEntity.OfficerId = evidenceViewModel.OfficerId;

            var _uow = Substitute.For<IUnitOfWork>();
            _uow.Case.GetByIdAsync(evidenceViewModel.CaseId, CancellationToken.None).Returns(caseEntity);
            _uow.Evidence.CreateAsync(evidenceEntity, CancellationToken.None).Returns(Task.CompletedTask);
            _uow.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(false);

            var sut = new CreateEvidence(_logger, _uow, _mapper, _validator);

            //Act
            var act = async () => await sut.RunAsync(evidenceViewModel, CancellationToken.None);

            //Assert
            await act.Should()
                .ThrowExactlyAsync<InfrastructureException>()
                .WithMessage("An unexpected error ocurred");
        }

        [Fact]
        public async Task RunAsync_OwnerValid_ReturnsSuccess()
        {
            // Arrange
            var evidenceViewModel = new CreateEvidenceViewModel
            {
                CaseId = Guid.NewGuid(),
                OfficerId = Guid.NewGuid(),
            };

            var validationResult = new ValidationResult();
            _validator.ValidateAsync(evidenceViewModel, CancellationToken.None).Returns(validationResult);

            var caseEntity = new CaseEntity { OfficerId = evidenceViewModel.OfficerId };
            _uow.Case.GetByIdAsync(evidenceViewModel.CaseId, CancellationToken.None).Returns(caseEntity);

            var evidenceEntity = new EvidenceEntity();
            _mapper.Map<EvidenceEntity>(evidenceViewModel).Returns(evidenceEntity);

            _uow.Evidence.CreateAsync(evidenceEntity, CancellationToken.None).Returns(Task.CompletedTask);
            _uow.SaveChangesAsync(CancellationToken.None).Returns(true);

            var sut = new CreateEvidence(_logger, _uow, _mapper, _validator);

            //Act
            var response = await sut.RunAsync(evidenceViewModel, CancellationToken.None);

            // Assert
            response.Should().BeOfType<BaseResponse>();
            await _uow.Case.Received(1).GetByIdAsync(evidenceViewModel.CaseId, CancellationToken.None);
            await _uow.Evidence.Received(1).CreateAsync(evidenceEntity, CancellationToken.None);
            await _uow.Received(1).SaveChangesAsync(CancellationToken.None);
        }

        [Fact]
        public async Task RunAsync_OwnerInvalid_ReturnsError()
        {
            // Arrange
            var evidenceViewModel = new CreateEvidenceViewModel
            {
                CaseId = Guid.NewGuid(),
                OfficerId = Guid.NewGuid(),
            };

            var validationResult = new ValidationResult();
            _validator.ValidateAsync(evidenceViewModel, CancellationToken.None).Returns(validationResult);

            var caseEntity = new CaseEntity { OfficerId = Guid.NewGuid() };
            _uow.Case.GetByIdAsync(evidenceViewModel.CaseId, CancellationToken.None).Returns(caseEntity);

            var sut = new CreateEvidence(_logger, _uow, _mapper, _validator);

            //Act
            var response = await sut.RunAsync(evidenceViewModel, CancellationToken.None);

            // Assert
            response.Should().BeOfType<BaseResponse>();
            response.Success.Should().BeFalse();
            response.ResponseDetails.Message.Should().Be(ResponseMessage.Forbidden.GetDescription());
            await _uow.Case.Received(1).GetByIdAsync(evidenceViewModel.CaseId, CancellationToken.None);
        }
    }
}
