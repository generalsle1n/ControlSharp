using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;

namespace ControlSharp.Ui.Controllers.Asset;

public class AssetController : Controller
{
    public IActionResult Asset()
    {
        return View();
    }
}