using PoliceDepartment.EvidenceManager.SharedKernel.Evidence.UseCases;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.API.Application.Evidence.UseCases
{
    public class GetPaginatedEvidences : IGetPaginatedEvidences<BaseResponseWithValue<IEnumerable<EvidenceViewModel>>>
    {
        public Task<BaseResponseWithValue<IEnumerable<EvidenceViewModel>>> RunAsync(int page, int rows, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
