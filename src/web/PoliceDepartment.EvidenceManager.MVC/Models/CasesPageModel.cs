using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.MVC.Models
{
    public class CasesPageModel
    {
        public IEnumerable<CaseViewModel> Cases { get; set; }
        public Guid OfficerId { get; set; }

        public CasesPageModel()
        {
            Cases = Enumerable.Empty<CaseViewModel>();
        }

        public CasesPageModel AsSuccess(IEnumerable<CaseViewModel> cases, Guid officerId)
        {
            Cases = cases;
            OfficerId= officerId;
            return this;
        }
    }
}
