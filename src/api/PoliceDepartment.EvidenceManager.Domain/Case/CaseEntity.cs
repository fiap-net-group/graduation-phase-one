using PoliceDepartment.EvidenceManager.Domain.Evidence;
using PoliceDepartment.EvidenceManager.Domain.Officer;

namespace PoliceDepartment.EvidenceManager.Domain.Case
{
    public class CaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Guid OfficerId { get; set; }
        public OfficerEntity Officer { get; set; }
        public ICollection<EvidenceEntity> Evidences { get; set; }

        public bool Exists()
        {
            return Id != Guid.Empty;
        }
    }
}
