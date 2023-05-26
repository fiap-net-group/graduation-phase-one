using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
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
    public class UpdateCaseTests
    {
        private readonly ApiFixture _fixture;

        public UpdateCaseTests(ApiFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [InlineData(false,null,null, ResponseMessage.CaseDontExists)]
        [InlineData(true,"",null, ResponseMessage.InvalidCase)]
        [InlineData(true,null,"", ResponseMessage.InvalidCase)]
        [InlineData(true,"","", ResponseMessage.InvalidCase)]
        public async Task RunAsync_InvalidRequest_ShouldReturnError(bool exists, 
                                                                    string name,
                                                                    string description, 
                                                                    ResponseMessage expectedResponse)
        {
            //Arrange
            var logger = Substitute.For<ILoggerManager>();
            var uow = Substitute.For<IUnitOfWork>();

            var viewModel = new CaseViewModel { Name = name, Description = description };
            var entity = exists ? _fixture.Case.GenerateSingleEntity() : new CaseEntity();
            uow.Case.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(entity));

            var sut = new UpdateCase(logger, uow);

            //Act
            var response = await sut.RunAsync(Guid.NewGuid(), viewModel, CancellationToken.None);

            //Assert
            response.Success.Should().BeFalse();
            response.ResponseMessageEqual(expectedResponse);
        }

        [Fact]
        public async Task RunAsync_DatabaseError_ShouldThrow()
        {
            //Arrange
            var logger = Substitute.For<ILoggerManager>();
            var uow = Substitute.For<IUnitOfWork>();

            var viewModel = new CaseViewModel { Name = "name", Description = "description" };
            var entity = _fixture.Case.GenerateSingleEntity();
            uow.Case.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(entity));
            uow.SaveChangesAsync().Returns(Task.FromResult(false));
            
            var sut = new UpdateCase(logger, uow);

            //Act
            var act = () => sut.RunAsync(Guid.NewGuid(), viewModel, CancellationToken.None);

            //Assert
            await act.Should()
                .ThrowExactlyAsync<InfrastructureException>()
                .WithMessage("An unexpected error ocurred");
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("New name", null)]
        [InlineData(null, "New description")]
        [InlineData("New name", "New description")]
        public async Task RunAsync_ValidRequest_ShouldReturnSuccess(string name, string description)
        {
            //Arrange
            var logger = Substitute.For<ILoggerManager>();
            var uow = Substitute.For<IUnitOfWork>();

            var viewModel = new CaseViewModel { Name = name, Description = description };
            var entity = _fixture.Case.GenerateSingleEntity();
            uow.Case.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(entity));
            uow.SaveChangesAsync().Returns(Task.FromResult(true));

            var sut = new UpdateCase(logger, uow);

            //Act
            var response = await sut.RunAsync(Guid.NewGuid(), viewModel, CancellationToken.None);

            //Assert
            response.Success.Should().BeTrue();

            if(name is not null)
                entity.Name.Should().Be(name);
            else
                entity.Name.Should().NotBe(name);

            if (description is not null)
                entity.Description.Should().Be(description);
            else
                entity.Description.Should().NotBe(description);
        }
    }
}
