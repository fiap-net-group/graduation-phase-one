using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoliceDepartment.EvidenceManager.MVC.Cases.Interfaces
{
    public interface IGetCasesByOfficerId
    {
        Task<BaseResponseWithValue<IEnumerable<CaseViewModel>>> RunAsync(Guid officerId, CancellationToken cancellationToken);
    }
}
