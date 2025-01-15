using Microsoft.AspNetCore.Mvc;

namespace ControlSharp.Ui.Areas.DefaultWebUi.Controllers.AssetUnregistered
{
    [Area("DefaultWebUi")]
    public class AssetUnregisteredController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
