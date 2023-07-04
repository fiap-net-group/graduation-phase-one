using PoliceDepartment.EvidenceManager.SharedKernel.Authorization;
using PoliceDepartment.EvidenceManager.SharedKernel.Authorization.UseCases;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;

namespace PoliceDepartment.EvidenceManager.Application.Authorization.UseCases
{
    public class LogOut : ILogOut<LogOutViewModel,BaseResponse>
    {
        private readonly BaseResponse _response;
        private readonly IIdentityManager _identityManager;
        
        public LogOut(IIdentityManager identityManager)
        {
            _response = new();
            _identityManager = identityManager;
        }

        public async Task<BaseResponse> RunAsync(Guid userId, CancellationToken cancellationToken)
        {
            var result = await _identityManager.SignOutAsync(userId);

            if(result.Succeeded)
                return _response.AsSuccess();

            return _response.AsError();
        }
    }
}