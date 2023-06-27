using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PoliceDepartment.EvidenceManager.MVC.Authorization.Interfaces;
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
        private readonly IDeleteEvidence _deleteEvidence;
        private readonly string _serverPath;
        public EvidencesController(ILoggerManager logger, IOfficerUser officerUser, ICreateEvidence createEvidence, IWebHostEnvironment webHostEnvironment, IDeleteEvidence deleteEvidence) : base(logger)
        {
            _officerUser = officerUser;
            _createEvidence = createEvidence;
            _serverPath = webHostEnvironment.WebRootPath;
            _deleteEvidence = deleteEvidence;
        }

        [HttpGet]
        [Route("create/{id:guid}")]
        public IActionResult Create(Guid id)
        {
            ViewBag.CaseId = id;
            ViewBag.OfficerId = _officerUser.Id;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PostCreate(CreateEvidencePageViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View("Create", model);
            }

            await UploadImageToLocal(model, cancellationToken);         

            var createEvidenceResponse = await _createEvidence.RunAsync(model, cancellationToken);

            var returnUrl = Request.Query["returnUrl"].ToString();

            if (createEvidenceResponse.Success)
            {
                if (!String.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl.ToString());
                }
                else
                {
                    return View(nameof(Create), model);
                }
            };

            AddErrorsToModelState(createEvidenceResponse);

            return View(nameof(Create), model);
        }

        private async Task UploadImageToLocal(CreateEvidencePageViewModel model, CancellationToken cancellationToken)
        {
            string fullPath = _serverPath + "/evidences/";
            string fullName = model.ImageId.ToString() + Path.GetExtension(model.Image.FileName); 

            if (!Directory.Exists(fullPath))
                Directory.CreateDirectory(fullPath);

            using (var stream = System.IO.File.Create(fullPath + fullName))
            {
                await model.Image.CopyToAsync(stream, cancellationToken);
            }
        }

        [HttpGet]
        [Route("delete/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id,CancellationToken cancellationToken)
        {
            Logger.LogDebug("MVC - Begin deleting evidence", ("officerId", _officerUser.Id), ("evidenceId", id));

            var deleteEvidenceResponse = await _deleteEvidence.RunAsync(id, cancellationToken);

            var returnUrl = Request.Query["returnUrl"].ToString();

            if(string.IsNullOrEmpty(returnUrl))
                returnUrl = Url.Action("Index", "Cases");

            if (deleteEvidenceResponse.Success)
            {
                Logger.LogDebug("MVC - Delete evidence - Success", ("officerId", _officerUser.Id), ("evidenceId", id));

                return Redirect(returnUrl);
            }

            AddErrorsToModelState(deleteEvidenceResponse);

            Logger.LogWarning("MVC - Delete evidence - Error", ("officerId", _officerUser.Id), ("evidenceId", id));

            return Redirect(returnUrl);
        }
    }
}
