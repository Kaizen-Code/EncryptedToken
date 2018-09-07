using EncryptedToken.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace Kaizen.WebAPI.EncryptedToken
{
    internal static class WebAPIsExtension
    {
        #region cookie
        private const string cookieNAME = "kaizen-token";
        public static string GetCookieValue(this HttpRequestMessage httpRequestMessage)
        {
            CookieHeaderValue cookie = httpRequestMessage.Headers.GetCookies(cookieNAME).FirstOrDefault();
            if (cookie != null)
            {
                return cookie[cookieNAME].Value;
            }
            return null;
        }
        public static void SetCookieValue(this HttpResponseMessage resMessage, string cookieValue, DateTime? exp = null)
        {
            CookieHeaderValue ck = new CookieHeaderValue(cookieNAME, cookieValue);
            if (exp != null)
                ck.Expires = exp.Value;
            //ck.Domain = resMessage.RequestMessage.RequestUri.Host;
            ck.Path = @"/";
            ck.HttpOnly = true;
            resMessage.Headers.AddCookies(new CookieHeaderValue[] { ck });
        }
        public static void RemoveCookieValue(this HttpResponseMessage resMessage)
        {
            CookieHeaderValue ck = new CookieHeaderValue(cookieNAME,string.Empty)
            {
                Expires = DateTime.UtcNow.AddDays(-365),
                //ck.Domain = resMessage.RequestMessage.RequestUri.Host;
                Path = @"/",
                HttpOnly = true,
            };
            resMessage.Headers.AddCookies(new CookieHeaderValue[] { ck });
        }

        #endregion cookie

        public static bool Authorize(this HttpActionContext actionContext, string tokenValue,int? _permissionId)
        {
            var service = ServiceFactory.TokenService;
            if (!service.Validate(tokenValue))  // Negative checking 
            {
                actionContext.Response = new System.Net.Http.HttpResponseMessage(HttpStatusCode.BadRequest);
                return false;
            }
            // save user state so it can be used during rest of request life cycle 


            if (_permissionId == null)
            {
                return true;
            }
            if (!service.IsAllowed((_permissionId.Value))) // Negative checking 
            {
                actionContext.Response = new System.Net.Http.HttpResponseMessage(HttpStatusCode.Unauthorized);
                return false;
            }
            return true;
        }


        #region header
        private const string SCHEME = "bearer";
        public static string GetHeaderValue(this HttpRequestMessage httpRequestMessage)
        {
            AuthenticationHeaderValue authHeaderVal = httpRequestMessage.Headers.Authorization;
            if (authHeaderVal != null)
            {
                if (authHeaderVal.Scheme.Equals(SCHEME, StringComparison.OrdinalIgnoreCase)
                    && !string.IsNullOrEmpty(authHeaderVal.Parameter))
                {
                    return authHeaderVal.Parameter;
                }
            }
            return null;
        }

        #endregion header

    }
}
