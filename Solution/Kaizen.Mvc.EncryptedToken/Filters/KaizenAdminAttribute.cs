using EncryptedToken.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Routing;

namespace Kaizen.Mvc.EncryptedToken.Filters
{
    [AttributeUsage(AttributeTargets.Method)]
    public class KaizenAdminAttribute : FilterAttribute, IAuthorizationFilter
    {   
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var token = ServiceFactory.GetTokenState();
            if (token == null)
            {
                handleInValidRequest(filterContext);
                return; // no need for following execution 
            }
            if (token.IsValid)// doing double check to assure token is valid
            {
                // This will be executed when request is authenticated.
                if (token.Roles.Split(',').Contains(ServiceFactory.KaizenAdminRoleName))
                {
                    // request is authorized to access Kaizen Secure Module
                    return; // no need for following execution
                }
                handleAccessDeniedRequest(filterContext);
            }
            else
            {
                // In case of token is Invalid
                handleInValidRequest(filterContext);
            }
        }

        private void handleInValidRequest(AuthorizationContext filterContext)
        {
            var reqBase = filterContext.HttpContext.Request;
            // This will be executed when token is not captured and request is not authenticated.
            if (reqBase.IsAjaxRequest())
            {
                filterContext.Result = new HttpUnauthorizedResult(ServiceFactory.InvalidRequestMsg);
            }
            else
            {
                // Redirection to login page of Kaizen Secure Module
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Secure", action = "Administrator" }));
            }
        }
        private void handleAccessDeniedRequest(AuthorizationContext filterContext)
        {
            var reqBase = filterContext.HttpContext.Request;
            if (reqBase.IsAjaxRequest())
            {
                filterContext.Result = new HttpUnauthorizedResult(ServiceFactory.AccessDeniedMsg);
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Secure", action = "AccessDenied" }));
            }
        }


    }
}
