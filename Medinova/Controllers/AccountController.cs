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
        MedinovaContext context = new MedinovaContext();

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
            Session["userName"]=user.UserName;
            Session["fullName"]=user.FirstName+" "+ user.LastName;
            return RedirectToAction("Index", "AdminAbout");


        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Login");
        }
    }
}