using AutoMapper;
using PoliceDepartment.EvidenceManager.Application.Case;
using PoliceDepartment.EvidenceManager.Application.Evidence;

namespace PoliceDepartment.EvidenceManager.UnitTests.AutoMapper
{
    public class AutoMapperTests
    {
        [Fact]
        public void AutoMapper_CaseConfiguration_IsValid()
        {
            // Arrange
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CaseMapperProfile>();
            });

            // Act & Assert
            configuration.AssertConfigurationIsValid();
        }

        [Fact]
        public void AutoMapper_EvidenceConfiguration_IsValid()
        {
            // Arrange
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<EvidenceMapperProfile>();
            });

            // Act & Assert
            configuration.AssertConfigurationIsValid();
        }
    }
}
