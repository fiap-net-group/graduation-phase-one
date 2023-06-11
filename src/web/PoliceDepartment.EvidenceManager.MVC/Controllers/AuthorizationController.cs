using Microsoft.AspNetCore.Mvc;
using PoliceDepartment.EvidenceManager.MVC.Authorization.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Models;
using PoliceDepartment.EvidenceManager.SharedKernel.Extensions;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using System.Linq;

namespace PoliceDepartment.EvidenceManager.MVC.Controllers
{
    public sealed class AuthorizationController : Controller
    {
        private readonly ILoggerManager _logger;
        private readonly ILogin _login;

        public AuthorizationController(ILoggerManager logger, ILogin login)
        {
            _logger = logger;
            _login = login;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            _logger.LogDebug("MVC - Page Login");

            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel viewModel, CancellationToken cancellationToken)
        {
            _logger.LogDebug("MVC - Begin Login", ("username", viewModel.Username));

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("MVC - Invalid login properties", (nameof(ModelState), ModelState));

                return View(viewModel);
            }

            var loginResponse = await _login.RunAsync(viewModel.Username, viewModel.Password, cancellationToken);

            if (!loginResponse.Success || !HttpContext.User.Identity.IsAuthenticated)
            {
                _logger.LogWarning("MVC - Invalid login", ("username", viewModel.Username), ("loginResponse", loginResponse));

                foreach (var error in loginResponse.ResponseDetails.Errors)
                    ModelState.AddModelError(string.Empty, error);

                return View();
            }

            _logger.LogDebug("MVC - Success Login", ("username", viewModel.Username));

            var officerType = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "OfficerType");

            //TODO:
            //Add the create officer page
            if (officerType is not null && officerType.Value == Enum.GetName(OfficerType.Administrator))
                return RedirectToAction("Index", "Home");

            //TODO:
            //Add the cases page
            return RedirectToAction("Index", "Home");
        }
    }
}
