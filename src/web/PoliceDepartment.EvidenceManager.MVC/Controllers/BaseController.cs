using Microsoft.AspNetCore.Mvc;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;

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
    }
}
