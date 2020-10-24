using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace netbu.Controllers
{

    [Authorize]
    public class UsmartController : Controller
    {
        public ActionResult Index(string id)
        {
            return View();
        }
    }
}