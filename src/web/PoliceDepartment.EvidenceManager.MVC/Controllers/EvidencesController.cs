using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PoliceDepartment.EvidenceManager.MVC.Authorization.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Cases.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Cases.UseCases;
using PoliceDepartment.EvidenceManager.MVC.Evidences.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Models;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;

namespace PoliceDepartment.EvidenceManager.MVC.Controllers
{
    [Authorize]
    [Route("evidences")]
    public class EvidencesController : BaseController
    {
        private readonly IOfficerUser _officerUser;
        private readonly ICreateEvidence _createEvidence;
        private readonly IGetEvidenceDetails _getEvidenceDetails;
        private readonly IDeleteEvidence _deleteEvidence;
        private readonly IEditEvidence _editEvidence;

        public EvidencesController(ILoggerManager logger,
                                   IOfficerUser officerUser,
                                   ICreateEvidence createEvidence,
                                   IGetEvidenceDetails getEvidenceDetails,
                                   IDeleteEvidence deleteEvidence,
                                   IEditEvidence editEvidence = null) : base(logger)
        {
            _officerUser = officerUser;
            _createEvidence = createEvidence;
            _getEvidenceDetails = getEvidenceDetails;
            _deleteEvidence = deleteEvidence;
            _editEvidence = editEvidence;
        }

        [HttpGet]
        [Route("create/{id:guid}")]
        public IActionResult Create(Guid id, CreateEvidencePageViewModel model)
        {
            ViewBag.CaseId = id;
            ViewBag.OfficerId = _officerUser.Id;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> PostCreate(CreateEvidencePageViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View("Create", model);
            }

            var createEvidenceResponse = await _createEvidence.RunAsync(model, cancellationToken);

            if (createEvidenceResponse.Success)
            {
                return RedirectToReturnUrlIfSpecfied(View(nameof(Create), model));
            }

            AddErrorsToModelState(createEvidenceResponse);

            return View(nameof(Create), model);
        }

        [HttpGet]
        [Route("details/{id:guid}")]
        public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
        {
            Logger.LogDebug("MVC - Begin get evidence details", ("officerId", _officerUser.Id), ("evidenceId", id));

            if (id != Guid.Empty)
            {
                var details = await _getEvidenceDetails.RunAsync(id, cancellationToken);

                if (details.Success && details.Value is not null && details.Value.Valid())
                {
                    Logger.LogDebug("MVC - Success getting evidenceId details", ("officerId", _officerUser.Id), ("evidenceId", id));

                    return View(details.Value);
                }
            }

            Logger.LogDebug("MVC - Can't return case details because it doesn't exists", ("officerId", _officerUser.Id), ("evidenceId", id));

            return RedirectToAction("Error", "Home", 404);
        }

        [HttpGet]
        [Route("delete/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            Logger.LogDebug("MVC - Begin deleting evidence", ("officerId", _officerUser.Id), ("caseId", id));

            var response = await _deleteEvidence.RunAsync(id, cancellationToken);

            if (response.Success)
            {
                Logger.LogDebug("MVC - Success deleting evidence", ("officerId", _officerUser.Id), ("caseId", id));

                return RedirectToReturnUrl();
            }

            AddErrorsToModelState(response);

            Logger.LogDebug("MVC - Error deleting evidence", ("officerId", _officerUser.Id), ("caseId", id));

            return RedirectToReturnUrl();
        }

        [HttpGet]
        [Route("edit/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
        {
            Logger.LogDebug("MVC - Begin loading edit evidence page", ("officerId", _officerUser.Id), ("evidenceId", id));

            if (id != Guid.Empty)
            {
                var details = await _getEvidenceDetails.RunAsync(id, cancellationToken);

                if (details.Success && details.Value is not null && details.Value.Valid())
                {
                    Logger.LogDebug("MVC - Success loading edit evidence page", ("officerId", _officerUser.Id), ("evidenceId", id));

                    return View(new EditEvidencePageViewModel(details.Value));
                }
            }

            Logger.LogDebug("MVC - Can't load edit evidence because it doesn't exists", ("officerId", _officerUser.Id), ("evidenceId", id));

            return RedirectToAction("Error", "Home", 404);
        }

        [HttpPost]
        [Route("edit")]
        public async Task<IActionResult> PostEdit(EditEvidencePageViewModel viewModel, CancellationToken cancellationToken)
        {
            Logger.LogDebug("MVC - Begin editing evidence ", ("officerId", _officerUser.Id), ("evidenceId", viewModel.Id));

            if (viewModel.Id == Guid.Empty)
                ModelState.AddModelError(string.Empty, "Invalid id");

            if (!ModelState.IsValid)
            {
                Logger.LogDebug("MVC - Edit evidence - Invalid case", ("officerId", _officerUser.Id), ("evidenceId", viewModel.Id));

                return View(nameof(Edit), viewModel);
            }

            var editResponse = await _editEvidence.RunAsync(viewModel.Id, viewModel, cancellationToken);

            if (editResponse.Success)
            {
                Logger.LogDebug("MVC - Edit evidence - Success", ("officerId", _officerUser.Id), ("evidenceId", viewModel.Id));

                return RedirectToAction("Index", "Home");
            }

            Logger.LogDebug("MVC - Edit evidence - Error", ("officerId", _officerUser.Id), ("evidenceId", viewModel.Id), ("response", editResponse));

            AddErrorsToModelState(editResponse);

            return RedirectToAction("Index", "Home");
        }
    }
}
