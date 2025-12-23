using System.Web.Mvc;

namespace Medinova.Controllers
{
    [Authorize]
    public class AdminAboutController : Controller
    {
        // GET: AdminAbout
        public ActionResult Index()
        {
            return View();
        }
    }
}