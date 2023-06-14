using PoliceDepartment.EvidenceManager.MVC.Authorization.Interfaces;
using System.Security.Claims;

namespace PoliceDepartment.EvidenceManager.MVC.Authorization
{
    public sealed class OfficerUser : IOfficerUser
    {
        private readonly IHttpContextAccessor _accessor;

        public OfficerUser(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public HttpContext HttpContext
        {
            get
            {
                ArgumentNullException.ThrowIfNull(_accessor);

                return _accessor.HttpContext;
            }
        }

        public bool IsAuthenticated => HttpContext.User is not null &&
                                       HttpContext.User.Identity is not null &&
                                       HttpContext.User.Identity.IsAuthenticated;

        public string Name => IsAuthenticated ? HttpContext.User.Identity.Name : string.Empty;

        public Guid Id
        {
            get
            {
                if(!IsAuthenticated)
                    return Guid.Empty;

                var claim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

                if(claim is not null)
                    return Guid.Parse(claim.Value);

                return Guid.Empty;
            }
        }

        public string AccessToken
        {
            get
            {
                if (!IsAuthenticated)
                    return string.Empty;

                var claim = HttpContext.User.FindFirst(AuthorizationExtensions.AccessTokenClaimName);

                return claim?.Value;
            }
        }
    }
}
