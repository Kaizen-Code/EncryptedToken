using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace Kaizen.WebAPI.EncryptedToken.Filters
{
    public class CookieAuthorization :BaseAttribute
    {

        public CookieAuthorization(EnumPermisssion permission) : base(permission){  }
        public CookieAuthorization() : base() { }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var tokenValue = actionContext.Request.GetCookieValue();
            if (actionContext.Authorize(tokenValue, _permissionId))
                base.OnAuthorization(actionContext);
        }
    }
}
