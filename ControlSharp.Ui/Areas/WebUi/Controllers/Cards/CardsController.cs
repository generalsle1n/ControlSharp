using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ControlSharp.Ui.Areas.WebUi.Controllers.Cards;

[Area("WebUi")]
public class CardsController : Controller
{
    public IActionResult Basic() => View();
}
