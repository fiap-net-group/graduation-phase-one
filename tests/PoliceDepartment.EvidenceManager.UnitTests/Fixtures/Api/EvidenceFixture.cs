using Bogus;
using PoliceDepartment.EvidenceManager.Domain.Evidence;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.UnitTests.Fixtures.Api
{
    public class EvidenceFixture
    {
        public EvidenceEntity GenerateSingleEntity(Guid imageId = default)
        {
            return GenerateEntityCollection(1, imageId).First();
        }
        public IEnumerable<EvidenceEntity> GenerateEntityCollection(int quantity, Guid imageId = default)
        {
            return new Faker<EvidenceEntity>()
                .RuleFor(x => x.Id, f => f.Random.Guid())
                .RuleFor(x => x.Name, f => f.Lorem.Sentence())
                .RuleFor(x => x.Description, f => f.Lorem.Sentence())
                .RuleFor(x => x.ImageId, f => imageId == Guid.Empty ? f.Random.Guid() : imageId)
                .RuleFor(x => x.CaseId, f => f.Random.Guid())
                .RuleFor(x => x.CreatedAt, f => f.Date.Past())
                .RuleFor(x => x.UpdatedAt, f => f.Date.Past())
                .Generate(quantity);
        }
        public CreateEvidenceViewModel GenerateViewModel()
        {
            return new Faker<CreateEvidenceViewModel>()
                .RuleFor(x => x.Name, f => f.Lorem.Sentence())
                .RuleFor(x => x.Description, f => f.Lorem.Sentence())
                .RuleFor(x => x.CaseId, f => f.Random.Guid())
                .RuleFor(x => x.ImageId, f => f.Random.Guid())
                .Generate();
        }
    }
}
