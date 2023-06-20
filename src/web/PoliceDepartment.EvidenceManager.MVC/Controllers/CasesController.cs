using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PoliceDepartment.EvidenceManager.MVC.Authorization.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Cases.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Models;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.MVC.Controllers
{
    [Authorize]
    [Route("cases")]
    public class CasesController : BaseController
    {
        private readonly IOfficerUser _officerUser;

        private readonly IGetCasesByOfficerId _getCasesByOfficerId;
        private readonly ICreateCase _createCase;
        private readonly IGetCaseDetails _getCaseDetails;
        private readonly IEditCase _editCase;
        private readonly IDeleteCase _deleteCase;

        private readonly CasesPageModel _pageModel;

        public CasesController(ILoggerManager logger,
                               IOfficerUser officerUser,
                               IGetCasesByOfficerId getCasesByOfficerId,
                               ICreateCase createCase,
                               IGetCaseDetails getCaseDetails,
                               IEditCase editCase,
                               IDeleteCase deleteCase) : base(logger)
        {
            _officerUser = officerUser;
            _getCasesByOfficerId = getCasesByOfficerId;
            _createCase = createCase;
            _getCaseDetails = getCaseDetails;
            _editCase = editCase;
            _deleteCase = deleteCase;

            _pageModel = new();
        }

        [HttpGet]
        [Route("")]
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
        [Route("create")]
        public IActionResult Create()
        {
            Logger.LogDebug("MVC - Create Case", ("officerId", _officerUser.Id));

            return View();
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> PostCreate(CreateCasePageViewModel viewModel, CancellationToken cancellationToken)
        {
            Logger.LogDebug("MVC - Begin create case", ("officerId", _officerUser.Id));

            if (!ModelState.IsValid)
            {
                Logger.LogDebug("MVC - Create case - Invalid case", ("officerId", _officerUser.Id));

                return View(nameof(Create), viewModel);
            }

            var createCaseResponse = await _createCase.RunAsync(viewModel, cancellationToken);

            if (createCaseResponse.Success)
            {
                Logger.LogDebug("MVC - Create case - Success", ("officerId", _officerUser.Id));

                return RedirectToAction("Index", "Home");
            }

            Logger.LogDebug("MVC - Create case - Error", ("officerId", _officerUser.Id), ("response", createCaseResponse));

            AddErrorsToModelState(createCaseResponse);

            return View(nameof(Create), viewModel);
        }

        [HttpGet]
        [Route("details/{id:guid}")]
        public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
        {
            Logger.LogDebug("MVC - Begin get case details", ("officerId", _officerUser.Id), ("caseId", id));

            if (id != Guid.Empty)
            {
                var details = await _getCaseDetails.RunAsync(id, cancellationToken);

                if (details.Success && details.Value is not null && details.Value.Valid())
                {
                    Logger.LogDebug("MVC - Success getting case details", ("officerId", _officerUser.Id), ("caseId", id));

                    return View(details.Value);
                }
            }

            Logger.LogDebug("MVC - Can't return case details because it doesn't exists", ("officerId", _officerUser.Id), ("caseId", id));
            
            return RedirectToAction("Error", "Home", 404);
        }

        [HttpGet]
        [Route("edit/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
        {
            Logger.LogDebug("MVC - Begin loading edit case page", ("officerId", _officerUser.Id), ("caseId", id));

            if (id != Guid.Empty)
            {
                var details = await _getCaseDetails.RunAsync(id, cancellationToken);

                if (details.Success && details.Value is not null && details.Value.Valid())
                {
                    Logger.LogDebug("MVC - Success loading edit case page", ("officerId", _officerUser.Id), ("caseId", id));

                    return View(details.Value);
                }
            }

            Logger.LogDebug("MVC - Can't edit case because it doesn't exists", ("officerId", _officerUser.Id), ("caseId", id));

            return RedirectToAction("Error", "Home", 404);
        }

        [HttpPost]
        [Route("edit")]
        public async Task<IActionResult> PostEdit(CaseViewModel viewModel, CancellationToken cancellationToken)
        {
            Logger.LogDebug("MVC - Begin edit case", ("officerId", _officerUser.Id), ("caseId", viewModel.Id));

            if (viewModel.Id != null && viewModel.Id == Guid.Empty)
                ModelState.AddModelError(string.Empty, "Invalid id");

            if (!ModelState.IsValid)
            {
                Logger.LogDebug("MVC - Edit case - Invalid case", ("officerId", _officerUser.Id), ("caseId", viewModel.Id));
                
                return View(nameof(Edit), viewModel);
            }

            var editCaseResponse = await _editCase.RunAsync(viewModel.Id.Value, viewModel, cancellationToken);

            if (editCaseResponse.Success)
            {
                Logger.LogDebug("MVC - Edit case - Success", ("officerId", _officerUser.Id), ("caseId", viewModel.Id));

                return RedirectToAction("Index", "Home");
            }

            Logger.LogDebug("MVC - Edit case - Error", ("officerId", _officerUser.Id), ("caseId", viewModel.Id), ("response", editCaseResponse));

            AddErrorsToModelState(editCaseResponse);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> PostDelete(Guid id, CancellationToken cancellationToken)
        {
            Logger.LogDebug("MVC - Begin deleting case", ("officerId", _officerUser.Id), ("caseId", id));

            var response = await _deleteCase.RunAsync(id, cancellationToken);

            if(response.Success)
            {
                Logger.LogDebug("MVC - Success deleting case", ("officerId", _officerUser.Id), ("caseId", id));

                return RedirectToAction(nameof(Index));
            }

            AddErrorsToModelState(response);

            Logger.LogDebug("MVC - Error deleting case", ("officerId", _officerUser.Id), ("caseId", id));

            return View(nameof(Index));
        }
    }
}
