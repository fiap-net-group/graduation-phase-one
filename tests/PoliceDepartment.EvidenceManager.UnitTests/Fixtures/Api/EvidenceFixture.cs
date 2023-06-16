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
        public EvidenceEntity GenerateSingleEntity(Guid imageId = default, Guid caseId = default)
        {
            return GenerateEntityCollection(1, imageId, caseId).First();
        }
      
        public IEnumerable<EvidenceEntity> GenerateEntityCollection(int quantity, Guid imageId = default, Guid caseId = default)
        {
            return new Faker<EvidenceEntity>()
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

        public CreateEvidenceViewModel GenerateViewModel()
        { 
            return new Faker<CreateEvidenceViewModel>()
                .RuleFor(x => x.Name, f => f.Lorem.Sentence())
                .RuleFor(x => x.Description, f => f.Lorem.Sentence())
                .RuleFor(x => x.OfficerId, f => f.Random.Guid())
                .RuleFor(x => x.CaseId, f => f.Random.Guid())
                .RuleFor(x => x.ImageId, f => f.Random.Guid())
                .Generate();
        }
      
        public IEnumerable<EvidenceViewModel> GenerateViewModelCollectionByEntityCollection(IEnumerable<EvidenceEntity> entities)
        {
            return entities.Select(GenerateViewModelByEntity);
        }
    }
}