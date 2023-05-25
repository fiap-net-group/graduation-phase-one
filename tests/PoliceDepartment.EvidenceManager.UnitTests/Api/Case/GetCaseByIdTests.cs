using AutoMapper;
using FluentAssertions;
using NSubstitute;
using PoliceDepartment.EvidenceManager.Application.Case.UseCases;
using PoliceDepartment.EvidenceManager.Domain.Case;
using PoliceDepartment.EvidenceManager.Domain.Database;
using PoliceDepartment.EvidenceManager.Domain.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;
using PoliceDepartment.EvidenceManager.UnitTests.Fixtures.Api;

namespace PoliceDepartment.EvidenceManager.UnitTests.Api.Case
{
    [Collection(nameof(ApiFixtureCollection))]
    public class GetCaseByIdTests
    {
        private readonly ApiFixture _fixture;

        public GetCaseByIdTests(ApiFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task RunAsync_CaseDontExists_ShouldReturnResponseAsError()
        {
            //Arrange
            var logger = Substitute.For<ILoggerManager>();
            var uow = Substitute.For<IUnitOfWork>();
            var mapper = Substitute.For<IMapper>();

            var entity = new CaseEntity();

            uow.Case.GetById(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(entity));
            
            var sut = new GetCaseById(logger, uow, mapper);

            //Act
            var response = await sut.RunAsync(Guid.NewGuid(), CancellationToken.None);

            //Assert
            response.Success.Should().BeFalse();
        }

        [Fact]
        public async Task RunAsync_CaseExists_ShouldReturnResponseAsSuccess()
        {
            //Arrange
            var logger = Substitute.For<ILoggerManager>();
            var uow = Substitute.For<IUnitOfWork>();
            var mapper = Substitute.For<IMapper>();

            var entity = _fixture.Case.GenerateSingleEntity();
            var viewModel = _fixture.Case.GenerateViewModelByEntity(entity);

            uow.Case.GetById(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(entity));
            mapper.Map<CaseViewModel>(Arg.Any<CaseEntity>()).Returns(viewModel);

            var sut = new GetCaseById(logger, uow, mapper);

            //Act
            var response = await sut.RunAsync(Guid.NewGuid(), CancellationToken.None);

            //Assert
            response.Success.Should().BeTrue();
            response.Value.Should().Be(viewModel);
        }
    }
}
