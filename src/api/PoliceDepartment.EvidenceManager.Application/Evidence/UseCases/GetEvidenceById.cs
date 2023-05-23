using PoliceDepartment.EvidenceManager.Domain.Evidence.UseCases;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.API.Application.Evidence.UseCases
{
    public class GetEvidenceById : IGetEvidenceById<BaseResponseWithValue<EvidenceViewModel>>
    {
        public Task<BaseResponseWithValue<EvidenceViewModel>> RunAsync(Guid id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
