using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PoliceDepartment.EvidenceManager.MVC.Authorization.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Models;
using PoliceDepartment.EvidenceManager.SharedKernel.Extensions;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;

namespace PoliceDepartment.EvidenceManager.MVC.Controllers
{
    public sealed class AuthorizationController : BaseController
    {
        private readonly ILogin _login;
        private readonly ILogout _logout;

        public AuthorizationController(ILoggerManager logger, 
                                       ILogin login, 
                                       ILogout logout) : base(logger)
        {
            _login = login;
            _logout = logout;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            Logger.LogDebug("MVC - Page Login");

            if(IsAuthenticated())
            {
                Logger.LogDebug("MVC - Page Login - User already authenticated");

                return RedirectToAction("Index", "Home");
            }

            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PostLogin(LoginModel viewModel, CancellationToken cancellationToken)
        {
            Logger.LogDebug("MVC - Begin Login", ("username", viewModel.Username));

            if (!ModelState.IsValid)
            {
                Logger.LogWarning("MVC - Invalid login properties", (nameof(ModelState), ModelState));

                return View(viewModel);
            }

            var loginResponse = await _login.RunAsync(viewModel.Username, viewModel.Password, cancellationToken);

            if (!loginResponse.Success)
            {
                Logger.LogWarning("MVC - Invalid login", ("username", viewModel.Username), ("loginResponse", loginResponse));

                AddErrorsToModelState(loginResponse);

                return View(viewModel);
            }

            Logger.LogDebug("MVC - Success Login", ("username", viewModel.Username));

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            if (!IsAuthenticated())
            {
                Logger.LogWarning("MVC - User can't logout because is not authenticated");

                return RedirectToAction("Index", "Home");
            }

            await _logout.RunAsync(cancellationToken);

            return RedirectToAction("Index", "Home");
        }
    }
}
