using EncryptedToken.Service.DataStore;
using EncryptedToken.Service.Repositories;
using EncryptedToken.Service.StateService;
using EncryptedToken.Service.Static;
using EncryptedToken.Service.TokenService;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EncryptedToken.Service
{
    public static class ServiceFactory
    {
        #region replacable 
        public static string uniqueNAME = "kaizen-TOKEN";
        public static string SCHEME = "bearer";
        public static string InvalidRequestMsg = "Invalid request!";
        public static string AccessDeniedMsg = "Access is denied!";
        #endregion

        public const string KaizenAdminRoleName = "Kaizen-Administrator";
        public const string KaizenUserId = "UserId";
        public const string KaizenPassword = "Password";
      

        #region private static 
        private static IStateService iStateService;
        static ServiceFactory()
        {
            iStateService = new CurrentItemService();
        }
        #endregion

        #region public
        public static void Initialize<T>() where T: struct
        {
            ServiceFactory.PermissionRepository.RegisterPermissioins<T>();
            ServiceFactory.UserRoleRepository.AddIfNotAdded(new DataStore.UserRole() { RoleTitle = ServiceFactory.KaizenAdminRoleName });
        }
        public static ITokenService TokenService => new EncryptedTokenService();
        public static PermissionRepository PermissionRepository => new PermissionRepository();
        public static UserRoleRepository UserRoleRepository => new UserRoleRepository();
        public static ConfigRepository ConfigRepository => new ConfigRepository();
        public static void SaveTokenState(ITokenService model)
        {
            iStateService.Save<ITokenService>(uniqueNAME, model);
        }
        public static ITokenService GetTokenState()
        {
            return iStateService.Get<ITokenService>(uniqueNAME);
        }

        #region cookie
        public static string GetTokenCookie(this HttpRequestBase reqBase)
        {
            var cookie = reqBase.Cookies[uniqueNAME];
            if (cookie != null)
            {
                return cookie.Value;
            }
            return null;
        }
        public static void SetTokenCookie(this HttpResponseBase response, string cookieValue,bool IsPersistent = false)
        {
            if (string.IsNullOrEmpty(cookieValue))
                return;
            var ck = new HttpCookie(uniqueNAME) { Value = cookieValue, Path = @"/", HttpOnly = true };
            if(IsPersistent)
            {
                var values = cookieValue.Split('.');
                int lifeSeconds;
                if(values.Length == 3 && int.TryParse(values[2],out lifeSeconds))
                {
                    ck.Expires = DateTime.UtcNow.AddSeconds(lifeSeconds);
                }
            }
            response.Cookies.Add(ck);
        }
        public static void RemoveTokenCookie(this HttpResponseBase response)
        {
            var ck = new HttpCookie(uniqueNAME) { Value = null, Expires = DateTime.UtcNow.AddDays(-365), Path = @"/", HttpOnly = true };
            response.Cookies.Add(ck);
        }

        //public static string GetCookieValue(this HttpRequestMessage httpRequestMessage)
        //{
        //    CookieHeaderValue cookie = httpRequestMessage.Headers.GetCookies(cookieNAME).FirstOrDefault();
        //    if (cookie != null)
        //    {
        //        return cookie[cookieNAME].Value;
        //    }
        //    return null;
        //}

        #endregion cookie
        #region header

        public static string GetHeaderValue(this HttpRequestBase reqBase)
        {
            var header = reqBase.Headers["Authorization"];
            if (header != null)
            {
                var values = header.Split(' ');
                if (values.Count() > 1 && values[0].Equals(SCHEME, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(values[1]))
                {
                    return values[1];
                }
            }
            return null;
        }
        #endregion header 


        public static void Startup()
        {
            Database.SetInitializer<SecureContext>(new CreateDatabaseIfNotExists<SecureContext>());
            UserRoleRepository.AddIfNotAdded(KaizenAdminRoleName);

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789~!@#$%^&*()_+{}:<>?,.;'[]-=";
            var random = new Random();

            Config UserIdModel = new Config();
            UserIdModel.ItemKey = KaizenUserId;
            UserIdModel.ItemValue = "Admin" + random.Next(1000, 9999);

            Config PasswordModel = new Config();
            PasswordModel.ItemKey = KaizenPassword;
            PasswordModel.ItemValue = new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray());

            Config SecreteModel = new Config();
            SecreteModel.ItemKey = SignatureHelper.SECRETEKEYNAME;
            SecreteModel.ItemValue = new string(Enumerable.Repeat(chars, 40).Select(s => s[random.Next(s.Length)]).ToArray());

            ConfigRepository.AddIfNotAdded(UserIdModel, PasswordModel, SecreteModel);
        }

        #endregion
    }
}
