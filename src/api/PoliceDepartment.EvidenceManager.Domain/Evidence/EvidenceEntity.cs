using PoliceDepartment.EvidenceManager.SharedKernel.Case;

namespace PoliceDepartment.EvidenceManager.SharedKernel.Evidence
{
    public class EvidenceEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid ImageId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Guid CaseId { get; set; }
        public CaseEntity Case { get; set; }

        public bool Exists()
        {
            return Id != Guid.Empty;
        }
    }
}
