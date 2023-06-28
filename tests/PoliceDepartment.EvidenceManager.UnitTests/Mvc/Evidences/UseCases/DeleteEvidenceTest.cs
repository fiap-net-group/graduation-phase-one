using FluentAssertions;
using NSubstitute;
using PoliceDepartment.EvidenceManager.MVC.Authorization.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Evidences.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Evidences.UseCases;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.UnitTests.Fixtures.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoliceDepartment.EvidenceManager.UnitTests.Mvc.Evidences.UseCases
{
    [Collection(nameof(MvcFixtureCollection))]
    public class DeleteEvidenceTest
    {
        private readonly MvcFixture _fixture;

        private readonly ILoggerManager _logger;
        private readonly IEvidencesClient _client;
        private readonly IOfficerUser _officerUser;

        public DeleteEvidenceTest(MvcFixture fixture)
        {
            _fixture = fixture;

            _logger = Substitute.For<ILoggerManager>();
            _client = Substitute.For<IEvidencesClient>();
            _officerUser = Substitute.For<IOfficerUser>();
        }

        [Fact]
        public void RunAsync_InvalidRequest_ShouldReturnError()
        {
            //Arrange
            _officerUser.Id.Returns(Guid.NewGuid());
            _officerUser.AccessToken.Returns(_fixture.Authorization.GenerateFakeJwtToken());
            _client.DeleteAsync(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                    .Returns(Task.FromResult(new BaseResponse().AsError()));

            var sut = new DeleteEvidence(_logger, _client, _officerUser);

            //Act
            var response = sut.RunAsync(Guid.NewGuid(), CancellationToken.None).Result;

            //Assert
            response.Success.Should().BeFalse();
        }

        [Fact]
        public void RunAsync_ValidRequest_ShouldReturnSuccess()
        {
            //Arrange
            _officerUser.Id.Returns(Guid.NewGuid());
            _officerUser.AccessToken.Returns(_fixture.Authorization.GenerateFakeJwtToken());
            _client.DeleteAsync(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                    .Returns(Task.FromResult(new BaseResponse().AsSuccess()));

            var sut = new DeleteEvidence(_logger, _client, _officerUser);

            //Act
            var response = sut.RunAsync(Guid.NewGuid(), CancellationToken.None).Result;

            //Assert
            response.Success.Should().BeTrue();
        }

    }
}
