using FluentAssertions;
using NSubstitute;
using PoliceDepartment.EvidenceManager.Application.Case.UseCases;
using PoliceDepartment.EvidenceManager.Domain.Case;
using PoliceDepartment.EvidenceManager.Domain.Database;
using PoliceDepartment.EvidenceManager.Domain.Exceptions;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.UnitTests.Fixtures.Api;

namespace PoliceDepartment.EvidenceManager.UnitTests.Api.Case
{
    [Collection(nameof(ApiFixtureCollection))]
    public class DeleteCaseTests
    {
        private readonly ApiFixture _fixture;

        public DeleteCaseTests(ApiFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task RunAsync_ValidCase_ShouldDelete()
        {
            //Arrange
            var logger = Substitute.For<ILoggerManager>();
            var uow = Substitute.For<IUnitOfWork>();

            var id = Guid.NewGuid();
            var officerId = Guid.NewGuid();
            var entity = _fixture.Case.GenerateSingleEntity(officerId);

            uow.Case.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(entity));
            uow.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(true));
            uow.CommmitAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(true));

            var sut = new DeleteCase(logger, uow);

            //Act
            var response = await sut.RunAsync(id, officerId, CancellationToken.None);

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

            var entity = _fixture.Case.GenerateSingleEntity(isFromOfficerId ? officerId : Guid.Empty);

            uow.Case.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                    .Returns(Task.FromResult(exists ? entity : new CaseEntity()));

            var sut = new DeleteCase(logger, uow);

            //Act
            var response = await sut.RunAsync(id, officerId, CancellationToken.None);

            //Assert
            response.Success.Should().BeFalse();

            if (!exists)
                response.ResponseMessageEqual(ResponseMessage.CaseDontExists).Should().BeTrue();

            if (!isFromOfficerId)
                response.ResponseMessageEqual(ResponseMessage.Forbidden).Should().BeTrue();
        }

        [Theory]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(false, false)]
        public void RunAsync_DatabaseErrors_ShouldThrow(bool successSavingChanges, bool successCommitTran)
        {
            //Arrange
            var logger = Substitute.For<ILoggerManager>();
            var uow = Substitute.For<IUnitOfWork>();

            var id = Guid.NewGuid();
            var officerId = Guid.NewGuid();
            var entity = _fixture.Case.GenerateSingleEntity(officerId);

            uow.Case.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(entity));
            uow.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(successSavingChanges));
            uow.CommmitAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(successCommitTran));

            var sut = new DeleteCase(logger, uow);

            //Act
            var response = () => sut.RunAsync(id, officerId, CancellationToken.None);

            //Assert
            response.Should().ThrowExactlyAsync<InfrastructureException>()
                             .WithMessage("An unexpected error ocurred");
        }
    }
}
