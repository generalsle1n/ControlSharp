using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ControlSharp.Ui.Areas.WebUi.Controllers.ExtendedUi;

[Area("WebUi")]
public class ExtendedUiController : Controller
{
    public IActionResult PerfectScrollbar() => View();
    public IActionResult TextDivider() => View();
}
