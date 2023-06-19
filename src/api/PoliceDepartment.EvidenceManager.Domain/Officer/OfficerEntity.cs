using Microsoft.AspNetCore.Identity;
using PoliceDepartment.EvidenceManager.SharedKernel.Case;

namespace PoliceDepartment.EvidenceManager.SharedKernel.Officer
{
    public class OfficerEntity : IdentityUser
    {
        public new Guid Id
        {
            get
            {
                return Guid.Parse(base.Id);
            }
            set
            {
                base.Id = value.ToString();
            }
        }

        public string Name { get; set; }

        public ICollection<CaseEntity> Cases { get; set; }

        public OfficerEntity()
        {
            Id = Guid.Empty;
        }

        public bool Exists()
        {
            return Id != Guid.Empty;
        }

        public OfficerEntity AsAdmin()
        {
            Id = Guid.NewGuid();
            Name = "Admin";
            return this;
        }
    }
}
