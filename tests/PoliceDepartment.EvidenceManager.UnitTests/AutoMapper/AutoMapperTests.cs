using AutoMapper;
using PoliceDepartment.EvidenceManager.Application.Case;

namespace PoliceDepartment.EvidenceManager.UnitTests.AutoMapper
{
    public class AutoMapperTests
    {
        [Fact]
        public void AutoMapper_Configuration_IsValid()
        {
            // Arrange
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CaseMapperProfile>();
            });

            // Act & Assert
            configuration.AssertConfigurationIsValid();
        }
    }
}
