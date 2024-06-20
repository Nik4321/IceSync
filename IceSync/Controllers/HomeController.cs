using IceSync.Models;
using IceSync.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace IceSync.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUniversalLoaderService _universalLoaderService;

        public HomeController(IUniversalLoaderService universalLoaderService)
        {
            _universalLoaderService = universalLoaderService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var workflows = await _universalLoaderService.GetWorkflowsAsync();
            return View(workflows);
        }

        [HttpPost("home/runworkflow/{workflowId}")]
        public async Task<IActionResult> RunWorkflow(string workflowId)
        {
            var success = await _universalLoaderService.RunWorkflowAsync(workflowId);
            return Json(new { success });
        }

        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
