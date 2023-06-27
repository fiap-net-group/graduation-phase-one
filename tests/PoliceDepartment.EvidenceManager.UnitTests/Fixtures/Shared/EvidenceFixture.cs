using Bogus;
using PoliceDepartment.EvidenceManager.SharedKernel.Evidence;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.UnitTests.Fixtures.Shared
{
    public class EvidenceFixture
    {
        public string CreateEvidenceUrl => "http://localhost/api/fake/evidences/";
        public string CreateEvidenceImageUrl => "http://localhost/api/fake/evidencesFiles/upload/";
        public string DeleteEvidenceImageUrl => "http://localhost/api/fake/evidencesFiles/*";
        public string GetEvidenceByIdUrl => "http://localhost/api/fake/evidences/*";
        public string GetEvidenceImageUrl => "http://localhost/api/fake/evidencesFiles/download/*";

        private readonly Faker<EvidenceEntity> _entityFaker;
        private readonly Faker<EvidenceViewModel> _genericViewModelFaker;
        private readonly Faker<CreateEvidenceViewModel> _createFaker;

        public EvidenceFixture()
        {
            _entityFaker = new();
            _genericViewModelFaker = new();
            _createFaker = new();
        }

        public EvidenceEntity GenerateSingleEntity(Guid imageId = default, Guid caseId = default)
        {
            return GenerateEntityCollection(1, imageId, caseId).First();
        }

        public IEnumerable<EvidenceEntity> GenerateEntityCollection(int quantity, Guid imageId = default, Guid caseId = default)
        {
            return _entityFaker
                .RuleFor(e => e.Id, Guid.NewGuid())
                .RuleFor(e => e.CaseId, caseId == Guid.Empty ? Guid.NewGuid() : caseId)
                .RuleFor(x => x.ImageId, f => imageId == Guid.Empty ? f.Random.Guid() : imageId)
                .RuleFor(e => e.Name, $"Fake name {Guid.NewGuid()}")
                .RuleFor(e => e.Description, "Description fake")
                .RuleFor(e => e.ImageId, Guid.NewGuid())
                .RuleFor(e => e.CreatedAt, DateTime.UtcNow)
                .RuleFor(e => e.UpdatedAt, DateTime.UtcNow)
                .Generate(quantity);
        }

        public CreateEvidenceViewModel GenerateSingleCreateEvidenceViewModel()
        {
            return _createFaker
                .RuleFor(x => x.Name, f => f.Lorem.Sentence())
                .RuleFor(x => x.Description, f => f.Lorem.Sentence())
                .RuleFor(x => x.OfficerId, f => f.Random.Guid())
                .RuleFor(x => x.CaseId, f => f.Random.Guid())
                .RuleFor(x => x.ImageId, f => f.Random.Guid())
                .Generate();
        }

        public EvidenceViewModel GenerateSingleViewModel()
        {
            return _genericViewModelFaker
                .RuleFor(x => x.Id, f => f.Random.Guid())
                .RuleFor(x => x.Name, f => f.Lorem.Sentence())
                .RuleFor(x => x.Description, f => f.Lorem.Sentence())
                .RuleFor(x => x.CaseId, f => f.Random.Guid().ToString())
                .RuleFor(x => x.ImageId, f => f.Random.Guid().ToString())
                .Generate();
        }

        public IEnumerable<EvidenceViewModel> GenerateViewModelCollectionByEntityCollection(IEnumerable<EvidenceEntity> entities)
        {
            return entities.Select(GenerateViewModelByEntity);
        }

        public EvidenceViewModel GenerateViewModelByEntity(EvidenceEntity entity)
        {
            return new EvidenceViewModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                CaseId = entity.CaseId.ToString(),
                ImageId = entity.ImageId.ToString()
            };
        }
    }
}
