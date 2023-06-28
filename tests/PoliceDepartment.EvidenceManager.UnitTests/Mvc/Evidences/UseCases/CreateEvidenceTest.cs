using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using PoliceDepartment.EvidenceManager.MVC.Authorization.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Evidences.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Evidences.UseCases;
using PoliceDepartment.EvidenceManager.MVC.Models;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;
using PoliceDepartment.EvidenceManager.UnitTests.Fixtures.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoliceDepartment.EvidenceManager.UnitTests.Mvc.Evidences.UseCases
{
    [Collection(nameof(MvcFixtureCollection))]
    public class CreateEvidenceTest
    {
        private readonly MvcFixture _fixture;

        private readonly ILoggerManager _logger;
        private readonly IEvidencesClient _client;
        private readonly IOfficerUser _officerUser;

        public CreateEvidenceTest(MvcFixture fixture)
        {
            _fixture = fixture;

            _logger = Substitute.For<ILoggerManager>();
            _client = Substitute.For<IEvidencesClient>();
            _officerUser = Substitute.For<IOfficerUser>();
        }

        [Fact]
        public void RunAsync_ValidRequest_ShouldReturnSuccess()
        {
            //Arrange
            var viewModel = new CreateEvidencePageViewModel
            {
                Name = "Fake name",
                Description = "Fake description",
                CaseId = Guid.NewGuid(),
                OfficerId = Guid.NewGuid(),
                Image = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, "Data", "dummy.txt"),
                ImageId = Guid.NewGuid()
            };
            _officerUser.Id.Returns(Guid.NewGuid());
            _officerUser.AccessToken.Returns(_fixture.Authorization.GenerateFakeJwtToken());
            _client.CreateEvidenceAsync(Arg.Any<CreateEvidenceViewModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                    .Returns(Task.FromResult(new BaseResponse().AsSuccess()));

            var sut = new CreateEvidence(_logger, _officerUser, _client);

            //Act
            var response = sut.RunAsync(viewModel, CancellationToken.None).Result;

            //Assert
            response.Success.Should().BeTrue();
        }

        [Fact]
        public void RunAsync_InvalidRequest_ShouldReturnError()
        {
            //Arrange
            var viewModel = new CreateEvidencePageViewModel
            {
                Name = "Fake name",
                Description = "Fake description",
                CaseId = Guid.NewGuid(),
                OfficerId = Guid.NewGuid(),
                Image = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, "Data", "dummy.txt"),
                ImageId = Guid.NewGuid()
            };
            _officerUser.Id.Returns(Guid.NewGuid());
            _officerUser.AccessToken.Returns(_fixture.Authorization.GenerateFakeJwtToken());
            _client.CreateEvidenceAsync(Arg.Any<CreateEvidenceViewModel>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                    .Returns(Task.FromResult(new BaseResponse().AsError()));

            var sut = new CreateEvidence(_logger, _officerUser, _client);

            //Act
            var response = sut.RunAsync(viewModel, CancellationToken.None).Result;

            //Assert
            response.Success.Should().BeFalse();
        }
    }
}
