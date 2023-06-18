using PoliceDepartment.EvidenceManager.UnitTests.Fixtures.Shared;

namespace PoliceDepartment.EvidenceManager.UnitTests.Fixtures.Mvc
{
    [CollectionDefinition(nameof(MvcFixtureCollection))]
    public class MvcFixtureCollection : ICollectionFixture<MvcFixture> { }

    public class MvcFixture
    {
        public AuthorizationFixture Authorization { get; set; }
        public CaseFixture Cases { get; set; }

        public MvcFixture()
        {
            Authorization = new();
            Cases = new();
        }
    }
}
