using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PoliceDepartment.EvidenceManager.MVC.Authorization.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Cases.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Models;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;

namespace PoliceDepartment.EvidenceManager.MVC.Controllers
{
    [Authorize]
    public class CasesController : BaseController
    {
        private readonly IOfficerUser _officerUser;

        private readonly IGetCasesByOfficerId _getCasesByOfficerId;
        private readonly ICreateCase _createCase;

        private readonly CasesPageModel _pageModel;

        public CasesController(ILoggerManager logger,
                               IOfficerUser officerUser,
                               IGetCasesByOfficerId getCasesByOfficerId,
                               ICreateCase createCase) : base(logger)
        {
            _officerUser = officerUser;
            _getCasesByOfficerId = getCasesByOfficerId;
            _createCase = createCase;

            _pageModel = new();
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            Logger.LogDebug("MVC - Cases Index", ("officerId", _officerUser.Id));

            var cases = await _getCasesByOfficerId.RunAsync(_officerUser.Id, cancellationToken);

            if (cases.Success)
            {
                Logger.LogDebug("MVC - Cases Index - Success", ("officerId", _officerUser.Id));

                return View(_pageModel.AsSuccess(cases.Value, _officerUser.Id));
            }

            Logger.LogWarning("MVC - Cases Index - Error", ("officerId", _officerUser.Id));

            return View(_pageModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            Logger.LogDebug("MVC - Create Case", ("officerId", _officerUser.Id));

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PostCreate(CreateCasePageViewModel viewModel, CancellationToken cancellationToken)
        {
            Logger.LogDebug("MVC - Begin create case", ("officerId", _officerUser.Id));

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var createCaseResponse = await _createCase.RunAsync(viewModel, cancellationToken);

            if (createCaseResponse.Success)
            {
                Logger.LogDebug("MVC - Create case - Success", ("officerId", _officerUser.Id));

                return RedirectToAction("Index", "Home");
            }

            Logger.LogDebug("MVC - Create case - Error", ("officerId", _officerUser.Id), ("response", createCaseResponse));

            AddErrorsToModelState(createCaseResponse);

            return View(viewModel);
        }
    }
}
