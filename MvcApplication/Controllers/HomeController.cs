using EncryptedToken.Service;
using Kaizen.Mvc.EncryptedToken.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApplication.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            //var tokenService = ServiceFactory.TokenService;
            //var tokenValue = tokenService.GetUniqueTokenValue("Arpan", "Administrator", 10);
            ////HttpContext.Response.SetCookieValue(tokenValue);
            //this.Response.SetCookieValue(tokenValue);

            return View();
        }


        [AuthorizeToken((int)PermissionTable.User_Create)]
        public ActionResult CreateUser()
        {
            return View();
        }

        
        [AuthorizeToken((int)PermissionTable.User_View)]
        public ActionResult DisplayUser()
        {
            var t = 0;
            return View();
        }

        
    }
}