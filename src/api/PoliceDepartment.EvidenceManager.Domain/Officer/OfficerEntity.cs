using Microsoft.AspNetCore.Identity;
using PoliceDepartment.EvidenceManager.Domain.Case;

namespace PoliceDepartment.EvidenceManager.Domain.Officer
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

        public ICollection<CaseEntity> Cases { get; set; }
    }
}
