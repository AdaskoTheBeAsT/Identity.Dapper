using Microsoft.AspNetCore.Mvc;

namespace Identity.Dapper.Samples.Web.Controllers
{
    public class HomeController : Controller
    {
#pragma warning disable SEC0120 // Missing Authorization Attribute
        public IActionResult Index()
#pragma warning restore SEC0120 // Missing Authorization Attribute
        {
            return View();
        }

#pragma warning disable SEC0120 // Missing Authorization Attribute
        public IActionResult About()
#pragma warning restore SEC0120 // Missing Authorization Attribute
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

#pragma warning disable SEC0120 // Missing Authorization Attribute
        public IActionResult Contact()
#pragma warning restore SEC0120 // Missing Authorization Attribute
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

#pragma warning disable SEC0120 // Missing Authorization Attribute
        public IActionResult Error() => View();
#pragma warning restore SEC0120 // Missing Authorization Attribute
    }
}
