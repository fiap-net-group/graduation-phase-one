using PoliceDepartment.EvidenceManager.Domain.Evidence.UseCases;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.API.Application.Evidence.UseCases
{
    public class GetPaginatedEvidences : IGetPaginatedEvidences<BaseResponse<IEnumerable<EvidenceViewModel>>>
    {
        public Task<BaseResponse<IEnumerable<EvidenceViewModel>>> RunAsync(int page, int rows, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
