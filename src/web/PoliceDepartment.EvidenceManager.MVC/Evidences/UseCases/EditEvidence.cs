using PoliceDepartment.EvidenceManager.MVC.Evidences.Interfaces;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.MVC.Evidences.UseCases
{
    public class EditEvidence : IEditEvidence
    {
        public EditEvidence()
        {
            
        }

        public Task<BaseResponse> RunAsync(Guid id, EvidenceViewModel evidenceViewModel, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
