using AutoMapper;
using FluentAssertions;
using NSubstitute;
using PoliceDepartment.EvidenceManager.Application.Case.UseCases;
using PoliceDepartment.EvidenceManager.SharedKernel.Case;
using PoliceDepartment.EvidenceManager.SharedKernel.Database;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;
using PoliceDepartment.EvidenceManager.UnitTests.Fixtures.Api;

namespace PoliceDepartment.EvidenceManager.UnitTests.Api.Case
{
    [Collection(nameof(ApiFixtureCollection))]
    public class GetByOfficerIdTests
    {
        private readonly ApiFixture _fixture;

        public GetByOfficerIdTests(ApiFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [InlineData(0)]
        [InlineData(10)]
        public async Task RunAsync_ShouldReturnTheNumberOfCases(int caseQuantity)
        {
            //Arrange
            var logger = Substitute.For<ILoggerManager>();
            var uow = Substitute.For<IUnitOfWork>();
            var mapper = Substitute.For<IMapper>();

            var officerId = Guid.NewGuid();
            var entities = caseQuantity > 0 ? _fixture.Case.GenerateEntityCollection(caseQuantity, officerId) : Enumerable.Empty<CaseEntity>();
            var viewModels = caseQuantity > 0 ? _fixture.Case.GenerateViewModelsByEntityCollection(entities) : Enumerable.Empty<CaseViewModel>();

            uow.Case.GetByOfficerId(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(entities));
            mapper.Map<IEnumerable<CaseViewModel>>(Arg.Any<IEnumerable<CaseEntity>>()).Returns(viewModels);

            var sut = new GetCaseByOfficerId(logger, uow, mapper);

            //Act
            var response = await sut.RunAsync(officerId, CancellationToken.None);

            //Assert
            response.Success.Should().BeTrue();
            response.Value.Count().Should().Be(caseQuantity);
            if (caseQuantity > 0)
                response.Value.Select(c => c.OfficerId).First().Should().Be(officerId);
        }
    }
}
