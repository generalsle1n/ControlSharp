using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ControlSharp.Ui.Areas.WebUi.Controllers.Forms;

[Area("DefaultWebUi")]
public class FormsController : Controller
{
    public IActionResult BasicInputs() => View();
    public IActionResult InputGroups() => View();
}
