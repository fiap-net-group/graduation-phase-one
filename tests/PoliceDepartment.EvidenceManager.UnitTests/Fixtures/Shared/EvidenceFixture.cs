using Bogus;
using Microsoft.AspNetCore.Hosting;
using NSubstitute;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.UnitTests.Fixtures.Shared
{
    public class EvidenceFixture
    {
        public string CreateEvidenceUrl => "http://localhost/api/fake/evidences/";
        public string DeleteUrl => "http://localhost/api/fake/evidences/*";

        public IWebHostEnvironment webHosting = Substitute.For<IWebHostEnvironment>();

        public CreateEvidenceViewModel GenerateSingleCreateViewModel()
        {
            return new Faker<CreateEvidenceViewModel>()
                .RuleFor(e => e.Name, $"Fake name {Guid.NewGuid()}")
                .RuleFor(e => e.Description, "Description fake")
                .RuleFor(e => e.CaseId, Guid.NewGuid())
                .RuleFor(e => e.OfficerId, Guid.NewGuid())
                .Generate(); 
        }
    }
}
