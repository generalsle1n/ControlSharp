using Microsoft.AspNetCore.Mvc;

namespace ControlSharp.Ui.Areas.DefaultWebUi.Controllers.AssetBanned
{
    [Area("DefaultWebUi")]
    public class AssetBannedController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
