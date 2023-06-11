namespace PoliceDepartment.EvidenceManager.UnitTests.Fixtures.Api
{
    [CollectionDefinition(nameof(ApiFixtureCollection))]
    public class ApiFixtureCollection : ICollectionFixture<ApiFixture> { }

    public class ApiFixture
    {
        public AuthorizationFixture Authorization { get; set; }
        public OfficerFixture Officer{ get; set; }
        public CaseFixture Case { get; set; }
        public EvidenceFixture Evidence { get; set; }

        public ApiFixture()
        {
            Authorization = new();
            Officer = new();
            Case = new();
            Evidence = new();
        }
    }
}
