using Bogus;
using PoliceDepartment.EvidenceManager.Domain.Officer;

namespace PoliceDepartment.EvidenceManager.UnitTests.Fixtures.Api
{
    public class OfficerFixture
    {
        public OfficerEntity GenerateSingleEntity()
        {
            return GenerateEntityCollection(1).First();
        }

        private IEnumerable<OfficerEntity> GenerateEntityCollection(int quantity)
        {
            return new Faker<OfficerEntity>()
                .RuleFor(o => o.Id, Guid.NewGuid())
                .RuleFor(o => o.Name, $"Fake name {Guid.NewGuid()}")
                .Generate(quantity);
        }
    }
}
