using Bogus;
using NSubstitute.Core;
using NSubstitute.ReceivedExtensions;
using PoliceDepartment.EvidenceManager.SharedKernel.Case;
using PoliceDepartment.EvidenceManager.SharedKernel.Evidence;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.UnitTests.Fixtures.Shared
{
    public class CaseFixture
    {
        public string GetByOfficerIdUrl => "http://localhost/api/fake/cases/officer*";
        public string CreateCaseUrl => "http://localhost/api/fake/cases/";
        public string GetDetailsUrl => "http://localhost/api/fake/cases/*";
        public string EditUrl => "http://localhost/api/fake/cases/*";
        public string DeleteUrl => "http://localhost/api/fake/cases/*";

        public CaseEntity GenerateSingleEntity(Guid officerId = default)
        {
            return GenerateEntityCollection(1, officerId).First();
        }

        public IEnumerable<CaseEntity> GenerateEntityCollection(int quantity, Guid officerId = default)
        {
            return new Faker<CaseEntity>()
                .RuleFor(c => c.Id, Guid.NewGuid())
                .RuleFor(c => c.OfficerId, officerId == Guid.Empty ? Guid.NewGuid() : officerId)
                .RuleFor(c => c.Name, $"Fake name {Guid.NewGuid()}")
                .RuleFor(c => c.Description, "Description fake")
                .RuleFor(c => c.CreatedAt, DateTime.UtcNow)
                .RuleFor(c => c.UpdatedAt, DateTime.UtcNow)
                .RuleFor(c => c.Evidences, new List<EvidenceEntity>())
                .Generate(quantity);
        }

        public IEnumerable<CaseViewModel> GenerateViewModelsByEntityCollection(IEnumerable<CaseEntity> cases)
        {
            var response = new List<CaseViewModel>();

            foreach (var entity in cases)
                response.Add(GenerateViewModelByEntity(entity));

            return response;
        }

        public CaseViewModel GenerateViewModelByEntity(CaseEntity entity)
        {
            return new CaseViewModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                CreatedAt = entity.CreatedAt,
                OfficerId = entity.OfficerId,
                Evidences = new List<EvidenceViewModel>()
            };
        }

        public CreateCaseViewModel GenerateSingleCreateCaseViewModel()
        {
            return new Faker<CreateCaseViewModel>()
               .RuleFor(c => c.Name, $"Fake name {Guid.NewGuid()}")
               .RuleFor(c => c.Description, "Description fake")
               .RuleFor(c => c.OfficerId, Guid.NewGuid())
               .Generate();
        }

        public CaseViewModel GenerateSingleViewModel()
        {
            return GenerateViewModelCollection(1).First();
        }

        public IEnumerable<CaseViewModel> GenerateViewModelCollection(int quantity, Guid officerId = default)
        {
            return new Faker<CaseViewModel>()
               .RuleFor(c => c.Name, $"Fake name {Guid.NewGuid()}")
               .RuleFor(c => c.Description, "Description fake")
               .RuleFor(c => c.OfficerId, officerId == Guid.Empty ? Guid.NewGuid() : officerId)
               .Generate(quantity);
        }
    }
}
