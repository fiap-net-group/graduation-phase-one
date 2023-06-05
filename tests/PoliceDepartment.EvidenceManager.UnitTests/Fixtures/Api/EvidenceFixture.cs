using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using PoliceDepartment.EvidenceManager.Domain.Evidence;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.UnitTests.Fixtures.Api
{
    public class EvidenceFixture
    {
        public EvidenceEntity GenerateSingleEntity(Guid caseId = default)
        public EvidenceEntity GenerateSingleEntity(Guid imageId = default)
        {
            return GenerateEntityCollection(1, caseId).First();
            return GenerateEntityCollection(1, imageId).First();
        }

        private IEnumerable<EvidenceEntity> GenerateEntityCollection(int v, Guid caseId)
        public IEnumerable<EvidenceEntity> GenerateEntityCollection(int quantity, Guid imageId = default)
        {
            return new Faker<EvidenceEntity>()
                .RuleFor(e => e.Id, Guid.NewGuid())
                .RuleFor(e => e.CaseId, caseId == Guid.Empty ? Guid.NewGuid() : caseId)
                .RuleFor(e => e.Name, $"Fake name {Guid.NewGuid()}")
                .RuleFor(e => e.Description, "Description fake")
                .RuleFor(e => e.ImageId, Guid.NewGuid())
                .RuleFor(e => e.CreatedAt, DateTime.UtcNow)
                .RuleFor(e => e.UpdatedAt, DateTime.UtcNow)
                .Generate(v);
                .RuleFor(x => x.Id, f => f.Random.Guid())
                .RuleFor(x => x.Name, f => f.Lorem.Sentence())
                .RuleFor(x => x.Description, f => f.Lorem.Sentence())
                .RuleFor(x => x.ImageId, f => imageId == Guid.Empty ? f.Random.Guid() : imageId)
                .RuleFor(x => x.CaseId, f => f.Random.Guid())
                .RuleFor(x => x.CreatedAt, f => f.Date.Past())
                .RuleFor(x => x.UpdatedAt, f => f.Date.Past())
                .Generate(quantity);
        }

        public EvidenceViewModel GenerateViewModelByEntity(EvidenceEntity entity)
        {
            return new EvidenceViewModel
        public CreateEvidenceViewModel GenerateViewModel()
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                CaseId = entity.CaseId.ToString(),
                ImageId = entity.ImageId.ToString()
            };
            return new Faker<CreateEvidenceViewModel>()
                .RuleFor(x => x.Name, f => f.Lorem.Sentence())
                .RuleFor(x => x.Description, f => f.Lorem.Sentence())
                .RuleFor(x => x.CaseId, f => f.Random.Guid())
                .RuleFor(x => x.ImageId, f => f.Random.Guid())
                .Generate();
        }
    }
}