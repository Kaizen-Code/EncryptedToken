using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MvcWebApplication
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
                   

            //Register all permissions into Module
            EncryptedToken.Service.ServiceFactory.Initialize<PermissionTable>();
            // get all roles from database 
            string[] roles = { "Developer", "Admin" , "Supervisor", "Clerk" };
            // all roles must be inserted into Module 
            EncryptedToken.Service.ServiceFactory.UserRoleRepository.AddIfNotAdded(roles); 


        }
    }
}
