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

        public bool Update(string name, string description)
        {
            return UpdateName(name) && 
                   UpdateDescription(description);
        }

        private bool UpdateName(string name)
        {
            if (name is null)
                return true;

            if (string.IsNullOrWhiteSpace(name))
                return false;

            Name = name;

            return true;
        }

        private bool UpdateDescription(string description)
        {
            if (description is null)
                return true;

            if (string.IsNullOrWhiteSpace(description))
                return false;

            Description = description;

            return true;
        }
    }
}
