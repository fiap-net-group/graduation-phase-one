using Microsoft.AspNetCore.Identity;

namespace PoliceDepartment.EvidenceManager.Domain.Authorization
{
    public interface IIdentityManager
    {
        Task<AccessTokenModel> AuthenticateAsync(string email, string password);

        Task<IdentityResult> CreateAsync(string email, string userName, string password, string officerType);

        Task<IdentityUser> FindByEmailAsync(string email);
    }
}
