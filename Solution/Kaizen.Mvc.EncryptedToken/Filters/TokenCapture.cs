using EncryptedToken.Service;
using EncryptedToken.Service.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

namespace Kaizen.Mvc.EncryptedToken.Filters
{
    public class TokenCapture : FilterAttribute, IAuthenticationFilter
    {
        
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            var reqBase = filterContext.HttpContext.Request;
            var tokenValue = reqBase.GetTokenCookie();
            if (string.IsNullOrEmpty(tokenValue))
                tokenValue = reqBase.GetHeaderValue();
            //filterContext.Principal = "";
            var service = ServiceFactory.TokenService;
            if (service.Validate(tokenValue))
            {
                //var nameClaim = new Claim(ClaimTypes.Name, service.UserName);
                //var claims = new List<Claim> { nameClaim };
                //var identity = new ClaimsIdentity(claims,"Basic");
                //filterContext.Principal = new ClaimsPrincipal(identity);
                ServiceFactory.SaveTokenState(service);
            }
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            
        }
    }
}
