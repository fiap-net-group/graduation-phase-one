using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using PoliceDepartment.EvidenceManager.Application.Evidence.UseCases;
using PoliceDepartment.EvidenceManager.Domain.Database;
using PoliceDepartment.EvidenceManager.Domain.Evidence;
using PoliceDepartment.EvidenceManager.Domain.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;
using PoliceDepartment.EvidenceManager.UnitTests.Fixtures.Api;

namespace PoliceDepartment.EvidenceManager.UnitTests.Api.Evidence
{
    [Collection(nameof(ApiFixtureCollection))]
    public class GetByCaseIdTests
    {
        private readonly ApiFixture _fixture;

        public GetByCaseIdTests(ApiFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public async Task RunAsync_ShouldReturnTheNumberOfCases(int quantity)
        {
            //Arrange
            var logger = Substitute.For<ILoggerManager>();
            var uow = Substitute.For<IUnitOfWork>();
            var mapper = Substitute.For<IMapper>();

            var caseId = Guid.NewGuid();
            var entities = quantity > 0 ? _fixture.Evidence.GenerateEntityCollection(quantity,caseId) : Enumerable.Empty<EvidenceEntity>();
            var viewModels = quantity > 0 ? _fixture.Evidence.GenerateViewModelCollectionByEntityCollection(entities) : Enumerable.Empty<EvidenceViewModel>();

            uow.Evidence.GetByCaseIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(entities));
            mapper.Map<IEnumerable<EvidenceViewModel>>(Arg.Any<IEnumerable<EvidenceEntity>>()).Returns(viewModels);

            var sut = new GetEvidencesByCaseId(logger, uow, mapper);

            //Act
            var response = await sut.RunAsync(caseId, CancellationToken.None);

            //Assert
            response.Success.Should().BeTrue();
            response.Value.Count().Should().Be(quantity);
            if (quantity > 0)
                response.Value.Select(c => c.CaseId).First().Should().Be(caseId.ToString());
        }
    }
}