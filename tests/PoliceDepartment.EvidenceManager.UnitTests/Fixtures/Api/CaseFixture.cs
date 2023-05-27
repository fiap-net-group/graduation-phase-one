using Bogus;
using PoliceDepartment.EvidenceManager.Domain.Case;
using PoliceDepartment.EvidenceManager.Domain.Evidence;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.UnitTests.Fixtures.Api
{
    public class CaseFixture
    {
        public CaseEntity GenerateSingleEntity()
        {
            return GenerateEntityCollection(1).First();
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

        public CreateCaseViewModel GenerateViewModel()
        {
            return new Faker<CreateCaseViewModel>()
                .RuleFor(c => c.Name, $"Fake name {Guid.NewGuid()}")
                .RuleFor(c => c.Description, "Description fake")
                .RuleFor(c => c.OfficerId, Guid.NewGuid())
                .Generate();
        }
    }
}
