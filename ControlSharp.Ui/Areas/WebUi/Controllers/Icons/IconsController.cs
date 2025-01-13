using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ControlSharp.Ui.Areas.WebUi.Controllers.Icons;

[Area("WebUi")]
public class IconsController : Controller
{
    public IActionResult Boxicons() => View();
}
