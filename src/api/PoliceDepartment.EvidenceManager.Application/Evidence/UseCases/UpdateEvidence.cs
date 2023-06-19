using PoliceDepartment.EvidenceManager.SharedKernel.Evidence.UseCases;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.API.Application.Evidence.UseCases
{
    public class UpdateEvidence : IUpdateEvidence<EvidenceViewModel, BaseResponse>
    {
        public Task<BaseResponse> RunAsync(Guid id, EvidenceViewModel enterprise, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
