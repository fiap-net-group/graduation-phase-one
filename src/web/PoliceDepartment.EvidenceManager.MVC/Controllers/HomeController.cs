using Microsoft.AspNetCore.Mvc;
using PoliceDepartment.EvidenceManager.MVC.Models;
using System.Diagnostics;

namespace PoliceDepartment.EvidenceManager.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Route("error/{statusCode:length(3,3)}")]
        public IActionResult Errors(int statusCode)
        {
            var modelErro = new ErrorModel(statusCode);

            return View("Error", modelErro);
        }
    }
}