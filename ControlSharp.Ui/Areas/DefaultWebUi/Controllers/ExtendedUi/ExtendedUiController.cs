using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ControlSharp.Ui.Areas.WebUi.Controllers.ExtendedUi;

[Area("DefaultWebUi")]
public class ExtendedUiController : Controller
{
    public IActionResult PerfectScrollbar() => View();
    public IActionResult TextDivider() => View();
}
