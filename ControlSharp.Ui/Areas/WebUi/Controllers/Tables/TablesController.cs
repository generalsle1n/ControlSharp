using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ControlSharp.Ui.Areas.WebUi.Controllers.Tables;

[Area("WebUi")]
public class TablesController : Controller
{
    public IActionResult Basic() => View();
}
