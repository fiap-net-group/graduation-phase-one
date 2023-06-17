using AutoMapper;
using FluentAssertions;
using NSubstitute;
using PoliceDepartment.EvidenceManager.API.Application.Evidence.UseCases;
using PoliceDepartment.EvidenceManager.Domain.Database;
using PoliceDepartment.EvidenceManager.Domain.Evidence;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;
using PoliceDepartment.EvidenceManager.UnitTests.Fixtures.Api;

namespace PoliceDepartment.EvidenceManager.UnitTests.Api.Evidence
{
    [Collection(nameof(ApiFixtureCollection))]
    public class GetEvidenceByIdTests
    {
        private readonly ApiFixture _fixture;

        public GetEvidenceByIdTests(ApiFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task RunAsync_EvidenceExists_ShouldReturnResponseAsSuccess()
        {
            //Arrange
            var logger = Substitute.For<ILoggerManager>();
            var uow = Substitute.For<IUnitOfWork>();
            var mapper = Substitute.For<IMapper>();

            var entity = _fixture.Evidence.GenerateSingleEntity();
            var viewModel = _fixture.Evidence.GenerateViewModelByEntity(entity);

            uow.Evidence.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(entity));
            mapper.Map<EvidenceViewModel>(Arg.Any<EvidenceEntity>()).Returns(viewModel);

            var sut = new GetEvidenceById(logger, uow, mapper);

            //Act
            var response = await sut.RunAsync(Guid.NewGuid(), CancellationToken.None);

            //Assert
            response.Success.Should().BeTrue();
            response.Value.Should().Be(viewModel);
        }

        [Fact]
        public async Task RunAsync_EvidenceDontExists_ShouldReturnResponseAsError()
        {
            //Arrange
            var logger = Substitute.For<ILoggerManager>();
            var uow = Substitute.For<IUnitOfWork>();
            var mapper = Substitute.For<IMapper>();

            var entity = new EvidenceEntity();

            uow.Evidence.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(entity));

            var sut = new GetEvidenceById(logger, uow, mapper);

            //Act
            var response = await sut.RunAsync(Guid.NewGuid(), CancellationToken.None);

            //Assert
            response.ResponseMessageEqual(ResponseMessage.EvidenceDontExists);
            response.Success.Should().BeFalse();
        }
        
    }
}