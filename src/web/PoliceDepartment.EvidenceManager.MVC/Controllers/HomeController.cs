using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PoliceDepartment.EvidenceManager.MVC.Models;
using PoliceDepartment.EvidenceManager.SharedKernel.Extensions;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using System.Diagnostics;

namespace PoliceDepartment.EvidenceManager.MVC.Controllers
{
    public class HomeController : BaseController
    {

        public HomeController(ILoggerManager logger) : base(logger) { }

        public IActionResult Index()
        {
            Logger.LogDebug("MVC - Home Index - Start");

            if (!IsAuthenticated())
            {
                Logger.LogInformation("MVC - Home Index - User not authenticated, redirecting to login");

                return RedirectToAction("Login", "Authorization");
            }

            var officerType = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "OfficerType");

            if (officerType is not null && officerType.Value == Enum.GetName(OfficerType.Administrator))
            {
                Logger.LogDebug("MVC - Home Index - User is admin");

                return RedirectToAction("Error", "Home", 403);
            }

            Logger.LogDebug("MVC - Home Index - User is officer, redirecting to cases");

            return RedirectToAction("Index", "Cases");
        }

        [Route("error/{statusCode:length(3,3)}")]
        public IActionResult Errors(int statusCode)
        {
            var modelErro = new ErrorModel(statusCode);

            return View("Error", modelErro);
        }
    }
}