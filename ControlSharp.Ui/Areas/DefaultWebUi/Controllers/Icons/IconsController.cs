using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ControlSharp.Ui.Areas.WebUi.Controllers.Icons;

[Area("DefaultWebUi")]
public class IconsController : Controller
{
    public IActionResult Boxicons() => View();
}
