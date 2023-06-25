using PoliceDepartment.EvidenceManager.MVC.Evidences.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Models;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.MVC.Evidences.UseCases
{
    public class GetEvidenceDetails : IGetEvidenceDetails
    {
        public GetEvidenceDetails()
        {
            
        }

        public Task<BaseResponseWithValue<EvidenceDetailViewModel>> RunAsync(Guid id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
