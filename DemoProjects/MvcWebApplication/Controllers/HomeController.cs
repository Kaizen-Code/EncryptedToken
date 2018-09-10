using EncryptedToken.Service;
using EncryptedToken.Service.DataStore;
using Kaizen.Mvc.EncryptedToken.Filters;
using MvcWebApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcWebApplication.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if(ModelState.IsValid)
            {
                //1st  Validate userid and password.
                //2nd  Get user's rolename
                var RoleName = "Developer";
                var tokenService = ServiceFactory.TokenService;// using EncryptedToken.Service
                //3rd  Create token as following
                var tokenValue = tokenService.GetUniqueTokenValue(model.LoginId, RoleName, 20);
                //4th  Write token in cookies
                this.Response.SetTokenCookie(tokenValue);// using EncryptedToken.Service;
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public ActionResult LogOff()
        {
            var tokenService = ServiceFactory.TokenService;// using EncryptedToken.Service;
            this.Response.RemoveTokenCookie();// using EncryptedToken.Service;
            return RedirectToAction("Index", "Home");
        }

        [AuthorizeToken] // To autheticate request
        public ActionResult About()
        {
            return View();
        }

        [AuthorizeToken] // To autheticate request 
        public ActionResult Contact()
        {
            return View();
        }


    }
}
