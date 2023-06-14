using Microsoft.AspNetCore.Identity;

namespace PoliceDepartment.EvidenceManager.Domain.Authorization
{
    public interface IIdentityManager
    {
        Task<AccessTokenModel> AuthenticateAsync(string email, string password, string name, CancellationToken cancellationToken);

        Task<IdentityResult> CreateAsync(string email, string password, string officerType);

        Task<IdentityUser> FindByEmailAsync(string email);

        Task<IdentityResult> SignOutAsync(Guid userId);
    }
}
