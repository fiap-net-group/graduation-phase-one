using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PoliceDepartment.EvidenceManager.MVC.Authorization.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Cases.Interfaces;
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

        public EvidencesController(ILoggerManager logger,
                                   IOfficerUser officerUser,
                                   ICreateEvidence createEvidence,
                                   IGetEvidenceDetails getEvidenceDetails,
                                   IDeleteEvidence deleteEvidence) : base(logger)
        {
            _officerUser = officerUser;
            _createEvidence = createEvidence;
            _getEvidenceDetails = getEvidenceDetails;
            _deleteEvidence = deleteEvidence;
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
    }
}
