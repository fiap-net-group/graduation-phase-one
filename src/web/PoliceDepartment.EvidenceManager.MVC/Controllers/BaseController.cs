using Microsoft.AspNetCore.Mvc;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;

namespace PoliceDepartment.EvidenceManager.MVC.Controllers
{
    public class BaseController : Controller
    {
        protected ILoggerManager Logger { get; }

        public BaseController(ILoggerManager logger)
        {
            Logger = logger;
        }

        protected bool IsAuthenticated()
        {
            return HttpContext.User is not null &&
                HttpContext.User.Identity is not null &&
                HttpContext.User.Identity.IsAuthenticated;
        }

        protected void AddErrorsToModelState(BaseResponse response)
        {
            if (response.ResponseDetails.Errors is null || !response.ResponseDetails.Errors.Any())
            {
                ModelState.AddModelError(string.Empty, response.ResponseDetails.Message);
                return;
            }

            foreach (var error in response.ResponseDetails.Errors)
                ModelState.AddModelError(string.Empty, error);
        }
    }
}
