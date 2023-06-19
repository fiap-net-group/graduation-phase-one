using PoliceDepartment.EvidenceManager.MVC.Cases.Interfaces;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.MVC.Cases.UseCases
{
    public class GetCaseDetails : IGetCaseDetails
    {
        //TODO:
        //Implement logic
        public Task<BaseResponseWithValue<CaseViewModel>> RunAsync(Guid caseId, CancellationToken cancellationToken)
        {
            return Task.FromResult(new BaseResponseWithValue<CaseViewModel>().AsError());
        }
    }
}
