using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Kaizen.WebAPI.EncryptedToken.Filters
{
    public class HeaderAuthorization : BaseAttribute
    {
        public HeaderAuthorization(EnumPermisssion permission) : base(permission) { }
        public HeaderAuthorization() : base() { }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var tokenValue = actionContext.Request.GetHeaderValue();
            if(actionContext.Authorize(tokenValue, _permissionId))
                base.OnAuthorization(actionContext);
        }

    }
}
