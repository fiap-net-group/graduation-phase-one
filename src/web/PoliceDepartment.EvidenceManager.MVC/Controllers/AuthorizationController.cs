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

            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel viewModel, CancellationToken cancellationToken)
        {
            Logger.LogDebug("MVC - Begin Login", ("username", viewModel.Username));

            if (!ModelState.IsValid)
            {
                Logger.LogWarning("MVC - Invalid login properties", (nameof(ModelState), ModelState));

                return View(viewModel);
            }

            var loginResponse = await _login.RunAsync(viewModel.Username, viewModel.Password, cancellationToken);

            if (!loginResponse.Success || !IsAuthenticated())
            {
                Logger.LogWarning("MVC - Invalid login", ("username", viewModel.Username), ("loginResponse", loginResponse));

                if ((loginResponse.ResponseDetails.Errors is null || !loginResponse.ResponseDetails.Errors.Any()) && !loginResponse.Success)
                {
                    ModelState.AddModelError(string.Empty, loginResponse.ResponseDetails.Message);
                    return View(viewModel);
                }

                if (loginResponse.Success)
                {
                    ModelState.AddModelError(string.Empty, ResponseMessage.GenericError.GetDescription());
                    return View(viewModel);
                }

                foreach (var error in loginResponse.ResponseDetails.Errors)
                    ModelState.AddModelError(string.Empty, error);

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
