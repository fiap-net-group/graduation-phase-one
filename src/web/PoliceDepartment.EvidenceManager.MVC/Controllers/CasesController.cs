using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PoliceDepartment.EvidenceManager.MVC.Authorization.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Cases.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Models;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoliceDepartment.EvidenceManager.MVC.Controllers
{
    [Authorize]
    public class CasesController : BaseController
    {
        private readonly IGetCasesByOfficerId _getCasesByOfficerId;
        private readonly IOfficerUser _officerUser;
        private readonly CasesPageModel _pageModel;

        public CasesController(ILoggerManager logger,
                               IOfficerUser officerUser,
                               IGetCasesByOfficerId getCasesByOfficerId) : base(logger)
        {
            _officerUser = officerUser;
            _getCasesByOfficerId = getCasesByOfficerId;

            _pageModel = new();
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            Logger.LogDebug("MVC - Cases Index");

            var cases = await _getCasesByOfficerId.RunAsync(_officerUser.Id, cancellationToken);

            if (cases.Success)
            {
                Logger.LogDebug("MVC - Cases Index - Success");

                return View(_pageModel.AsSuccess(cases.Value, _officerUser.Id));
            }

            Logger.LogWarning("MVC - Cases Index - Error");

            return View(_pageModel);
        }
    }
}
