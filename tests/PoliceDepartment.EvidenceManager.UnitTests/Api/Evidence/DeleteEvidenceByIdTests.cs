using FluentAssertions;
using NSubstitute;
using PoliceDepartment.EvidenceManager.API.Application.Evidence.UseCases;
using PoliceDepartment.EvidenceManager.SharedKernel.Database;
using PoliceDepartment.EvidenceManager.SharedKernel.Evidence;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.UnitTests.Fixtures.Api;

namespace PoliceDepartment.EvidenceManager.UnitTests.Api.Evidence
{
    [Collection(nameof(ApiFixtureCollection))]
    public class DeleteEvidenceByIdTests
    {
        private readonly ApiFixture _fixture;

        public DeleteEvidenceByIdTests(ApiFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task RunAsyn_ValidEvidence_ShouldDelete()
        {
            //Arrange
            var logger = Substitute.For<ILoggerManager>();
            var uow = Substitute.For<IUnitOfWork>();

            var id = Guid.NewGuid();
            var officerId = Guid.NewGuid();
            var evidenceEntity = _fixture.Evidence.GenerateSingleEntity();
            var caseEntity = _fixture.Case.GenerateSingleEntity(officerId);

            uow.Evidence.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(evidenceEntity));
            uow.Case.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(caseEntity));
            uow.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(true));
            uow.CommmitAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(true));

            var sut = new DeleteEvidence(uow, logger);

            //Act
            var response = await sut.RunAsync(id,officerId, CancellationToken.None);

            //Assert
            response.Success.Should().BeTrue();
        }

        [Theory]
        [InlineData(false, true)]
        [InlineData(true, false)]
        public async Task RunAsync_InvalidRequest_ShouldReturnErrorResponse(bool exists, bool isFromOfficerId)
        {
            //Arrange
            var logger = Substitute.For<ILoggerManager>();
            var uow = Substitute.For<IUnitOfWork>();

            var id = Guid.NewGuid();
            var officerId = Guid.NewGuid();

            var evidenceEntity = _fixture.Evidence.GenerateSingleEntity();
            var caseEntity = _fixture.Case.GenerateSingleEntity(isFromOfficerId ? officerId : Guid.Empty);

            if (!exists)
                evidenceEntity = new EvidenceEntity();

            uow.Evidence.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(evidenceEntity));
            uow.Case.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(caseEntity));

            var sut = new DeleteEvidence(uow, logger);

            //Act
            var response = await sut.RunAsync(id, officerId, CancellationToken.None);

            //Assert
            response.Success.Should().BeFalse();

            if(!exists)
                response.ResponseMessageEqual(ResponseMessage.EvidenceDontExists);

            if(!isFromOfficerId)
                response.ResponseMessageEqual(ResponseMessage.Forbidden);

        }
        
    }
}