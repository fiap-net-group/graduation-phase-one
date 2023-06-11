using PoliceDepartment.EvidenceManager.SharedKernel.Extensions;

namespace PoliceDepartment.EvidenceManager.SharedKernel.ViewModels
{
    public sealed record CreateOfficerViewModel(string Name, string Email,  string Password, OfficerType OfficerType);

    
}