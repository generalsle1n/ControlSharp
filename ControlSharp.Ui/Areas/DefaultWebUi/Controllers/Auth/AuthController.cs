using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ControlSharp.Ui.Areas.WebUi.Controllers.Auth;

[Area("DefaultWebUi")]
public class AuthController : Controller
{
    public IActionResult ForgotPasswordBasic() => View();
    public IActionResult LoginBasic() => View();
    public IActionResult RegisterBasic() => View();
}
