using EncryptedToken.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Kaizen.WebAPI.EncryptedToken.Filters
{
    public class BaseAttribute : AuthorizationFilterAttribute
    {
        protected int? _permissionId;
        protected BaseAttribute(EnumPermisssion permission)
        {
            _permissionId = (int)permission;
        }
        protected BaseAttribute() { }

    }
}
