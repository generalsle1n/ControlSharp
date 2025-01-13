using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ControlSharp.Ui.Areas.WebUi.Controllers.Pages;

[Area("DefaultWebUi")]
public class PagesController : Controller
{
    public IActionResult AccountSettings() => View();
    public IActionResult AccountSettingsConnections() => View();
    public IActionResult AccountSettingsNotifications() => View();
    public IActionResult MiscError() => View();
    public IActionResult MiscUnderMaintenance() => View();
}
