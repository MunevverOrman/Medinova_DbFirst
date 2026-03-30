using Medinova.Dtos;
using Medinova.Models;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace Medinova.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        MedinovaDbEntities1 context = new MedinovaDbEntities1();

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginDto model)
        {
            var user = context.Users
                              .FirstOrDefault(x => x.UserName == model.UserName && x.Password == model.Password);
            if (user == null)
            {
                ModelState.AddModelError("", "Kullanıcı veya şifre hatalı.");
                return View(model);
            }

            FormsAuthentication.SetAuthCookie(user.UserName, false);
            Session["userName"] = user.UserName;
            Session["fullName"] = user.FirstName + " " + user.LastName;

            var userRole = context.Users
                         .Where(x => x.UserId == user.UserId)
                         .SelectMany(x => x.Roles)
                         .Select(x => x.RoleName)
                         .FirstOrDefault();

            Session["role"] = userRole;

            switch (userRole)
            {
                case "Admin":
                    return RedirectToAction("Index", "AdminAbout");
                case "Doktor":
                    return RedirectToAction("Index", "Doktor");
                case "Hasta":
                    return RedirectToAction("Index", "Hasta");
                default:
                    ModelState.AddModelError("", "Rolünüz tanımlı değil.");
                    return View(model);
            }
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Login");
        }
    }
}