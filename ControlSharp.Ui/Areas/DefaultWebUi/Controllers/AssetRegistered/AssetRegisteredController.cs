using Microsoft.AspNetCore.Mvc;

namespace ControlSharp.Ui.Areas.DefaultWebUi.Controllers.AssetRegistered
{
    [Area("DefaultWebUi")]
    public class AssetRegisteredController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
