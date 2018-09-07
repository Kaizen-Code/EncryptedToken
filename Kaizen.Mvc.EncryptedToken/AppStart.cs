using EncryptedToken.Service;
using Kaizen.Mvc.EncryptedToken;
using Kaizen.Mvc.EncryptedToken.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

[assembly: PreApplicationStartMethod(typeof(AppStart), "Start")]
namespace Kaizen.Mvc.EncryptedToken
{
    public class AppStart
    {
        public static void Start()
        {
            GlobalFilters.Filters.Add(new TokenCapture()); //Register ActionFilter
            ServiceFactory.Startup();
        }
    }
}
