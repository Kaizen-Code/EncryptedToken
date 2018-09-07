using EncryptedToken.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Kaizen.Mvc.EncryptedToken.Filters
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AuthorizeToken : FilterAttribute, IAuthorizationFilter
    {
       
        private int _permissionId;

        public AuthorizeToken()
        {

        }

        public AuthorizeToken(int permissionId)
        {
            if (permissionId < 1)
                throw new ArgumentException("Invalid permission Id! Permission Id should be greater than zero");
            _permissionId = permissionId;
        }
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var token = ServiceFactory.GetTokenState();
            if(token == null)
            {
                // This will be executed when token is not captured and request is not authenticated.
                filterContext.Result = new HttpUnauthorizedResult(ServiceFactory.InvalidRequestMsg);
                return; // no need for following execution 
            }
            if(token.IsValid) // doing double check to assure token is valid
            {
                // This will be executed when request is authenticated.
                if (_permissionId < 1)
                {
                    // Attribute is set to authenticate request only so no need to execute following flow.
                    return; // no need for following execution
                }
                var acceptHeader = filterContext.HttpContext.Request.Headers["Accept"];
                if(acceptHeader != null && acceptHeader.Contains("html"))
                    token.LoadAllowedPermissions();
                if (token.IsAllowed(_permissionId))
                {
                    return; // permission is granted to user's role 
                }
                // Following will be executed in case of permission is not granted to user's role
                filterContext.Result = new HttpUnauthorizedResult(ServiceFactory.AccessDeniedMsg);
            }
            else
            {
                // In case of token is Invalid
                filterContext.Result = new HttpUnauthorizedResult(ServiceFactory.InvalidRequestMsg);
            }
            
        }
    }
    
}
