using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PoliceDepartment.EvidenceManager.SharedKernel.Evidence.UseCases
{
    public interface IGetEvidencesByCaseId<TResult>
    {
        Task<TResult> RunAsync(Guid caseId, CancellationToken cancellationToken);
    }
}