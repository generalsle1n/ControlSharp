using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ControlSharp.Ui.Areas.WebUi.Controllers.FormLayouts;

[Area("DefaultWebUi")]
public class FormLayoutsController : Controller
{
    public IActionResult Horizontal() => View();
    public IActionResult Vertical() => View();
}
