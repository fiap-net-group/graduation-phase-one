using PoliceDepartment.EvidenceManager.Domain.Evidence.UseCases;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.API.Application.Evidence.UseCases
{
    public class CreateEvidence : ICreateEvidence<EvidenceViewModel, BaseResponse>
    {
        public Task<BaseResponse> RunAsync(EvidenceViewModel evidence, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
