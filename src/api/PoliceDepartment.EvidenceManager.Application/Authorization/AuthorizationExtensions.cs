using System.Security.Claims;

namespace PoliceDepartment.EvidenceManager.Application.Authorization
{
    public static class AuthorizationExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal userClaims)
        {
            var userId = userClaims.Claims.FirstOrDefault(i => i.Type == "sub");

            userId ??= userClaims.Claims.FirstOrDefault(i => i.Type == ClaimTypes.NameIdentifier);

            if (userId is null)
                return Guid.Empty;

            return Guid.Parse(userId.Value);
        }
    }
}
