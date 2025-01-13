using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ControlSharp.Ui.Areas.WebUi.Controllers.Dashboards;

[Area("WebUi")]
public class DashboardsController : Controller
{
    public IActionResult Index() => View();
}
