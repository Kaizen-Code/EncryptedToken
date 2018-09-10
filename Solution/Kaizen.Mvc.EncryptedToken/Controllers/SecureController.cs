using EncryptedToken.Service;
using EncryptedToken.Service.DataStore;
using Kaizen.Mvc.EncryptedToken.Filters;
using Kaizen.Mvc.EncryptedToken.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Kaizen.Mvc.EncryptedToken.Controllers
{
    public class SecureController : Controller
    {
        #region anonymous access to Secure Pages
        public ActionResult Administrator()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Administrator(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var loginIdObj = ServiceFactory.ConfigRepository.FirstOrDefault(c => c.ItemKey == ServiceFactory.KaizenUserId);
                var passwordObj = ServiceFactory.ConfigRepository.FirstOrDefault(c => c.ItemKey == ServiceFactory.KaizenPassword);
                if (loginIdObj != null && passwordObj != null)
                    if (loginIdObj.ItemValue == model.LoginId && passwordObj.ItemValue == model.Password)
                    {
                        var tokenService = ServiceFactory.TokenService;
                        var tokenValue = tokenService.GetUniqueTokenValue(model.LoginId, ServiceFactory.KaizenAdminRoleName, 20);
                        this.Response.SetTokenCookie(tokenValue);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Msg = "Error: Invalid UserId or Password!";
                    }
            }
            return View();
        }

        public ActionResult logout()
        {
            var tokenService = ServiceFactory.TokenService;
            this.Response.RemoveTokenCookie();
            return RedirectToAction("Administrator");
        }

        public ActionResult AccessDenied()
        {
            return View();
        }

        #endregion anonymous access to Secure Module



        #region Internal GET actionresult 

        [KaizenAdmin]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Navigation()
        {
            return PartialView("_Navigation");
        }

        [KaizenAdmin]
        public ActionResult PermissionList()
        {
            var list = ServiceFactory.PermissionRepository.GetAll();
            return PartialView("_PermissionList", list);
        }

        [KaizenAdmin]
        public ActionResult PermissionDetail(int id)
        {
            var service = ServiceFactory.UserRoleRepository;
            var allowedList = service.GetAllowedUserRoles(id);
            var deniedList = service.GetDeniedUserRoles(id);
            ViewData["secureTuple"] = new Tuple<List<UserRole>, List<UserRole>>(deniedList, allowedList);
            var model = ServiceFactory.PermissionRepository.FirstOrDefault(id);
            return PartialView("_PermissionDetail", model);
        }
        [KaizenAdmin]
        public ActionResult EditPermission(int id)
        {
            var service = ServiceFactory.UserRoleRepository;
            var model = ServiceFactory.PermissionRepository.FirstOrDefault(id);
            return PartialView("_PermissionEdit", model);
        }

        [KaizenAdmin]
        public ActionResult UserList()
        {
            var bal = ServiceFactory.UserRoleRepository;
            return PartialView("_UserList", bal.GetAll());
        }

        [KaizenAdmin]
        public ActionResult UserDetail(int id)
        {
            var userBal = ServiceFactory.UserRoleRepository;
            ViewData["UserRole"] = userBal.FirstOrDefault(id);
            var bal = ServiceFactory.PermissionRepository;
            var model = new Tuple<List<Permission>, List<Permission>>(bal.GetDeniedPermissions(id), bal.GetAllowedPermissions(id));
            return PartialView("_UserDetail", model);
        }
        #endregion Internal GET actionresult



        #region internal POST Json 

        [HttpPost]
        [KaizenAdmin]
        public JsonResult AssignUserRoles(int id, int[] items)
        {
            var bal = ServiceFactory.PermissionRepository;
            return Json(bal.AssignUserRoles(id, items), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [KaizenAdmin]
        public JsonResult DismissUserRoles(int id, int[] items)
        {
            var bal = ServiceFactory.PermissionRepository;
            return Json(bal.DismissUserRoles(id, items), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [KaizenAdmin]
        public JsonResult AssignPermissions(int id, int[] items)
        {
            var bal = ServiceFactory.UserRoleRepository;
            return Json(bal.AssignPermissions(id, items), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [KaizenAdmin]
        public JsonResult DeniedPermissions(int id, int[] items)
        {
            var bal = ServiceFactory.UserRoleRepository;
            return Json(bal.DeniedPermissions(id, items), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [KaizenAdmin]
        public JsonResult SavePermission(Permission model)
        {
            var bal = ServiceFactory.PermissionRepository;
            return Json(bal.Update(model), JsonRequestBehavior.AllowGet);
        }

        #endregion internal POST Json  


        #region External 

        public ActionResult RefreshToken()
        {
            var token = ServiceFactory.GetTokenState();
            if (token == null)
            {
                return new HttpStatusCodeResult(401, "Unauthorized");
            }
            var tokenValue = token.Refresh();
            if(string.IsNullOrEmpty(tokenValue))
            {
                return new HttpStatusCodeResult(401, "Invalid or Expired token!");
            }
            Response.SetTokenCookie(tokenValue);
            return new EmptyResult();
        }

        #endregion External 

    }

}
