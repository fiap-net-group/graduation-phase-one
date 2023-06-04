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
        {
            return GenerateEntityCollection(1, caseId).First();
        }

        private IEnumerable<EvidenceEntity> GenerateEntityCollection(int v, Guid caseId)
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