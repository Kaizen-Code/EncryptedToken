using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcWebApplication
{
    public enum PermissionTable
    {

        Home_About = 1001,
        Home_Contact = 1002,

        Client_List = 1101,  // started from  1101 because we can add more permissions in home if it require in future.
        Client_Create = 1102,
        Client_Edit = 1103,
        Client_Delete = 1104,
        Client_DetailView = 1105
       
    }
}