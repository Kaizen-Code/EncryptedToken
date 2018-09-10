using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication
{
    public enum PermissionTable
    {
        /******* followings are examples about how to create permissions *****/
        User_View = 10,
        User_Create = 20,
        User_Print = 30,
        User_Edit = 40,

        Account_Create = 110,
        Acccount_Display =120 ,
        Account_Print = 130,
        Account_Delete = 140,
        Account_Edit = 150,

        TrialBalance_View = 210,
        Trial_Balance_Download = 220,

        Product_Listing = 310,
        Product_View = 320,
        Product_Edit = 330,
        Product_Delete= 340,
        Product_Create = 350,

        Dashboard_View = 410,
        /*********** remove enum values given above  **********/

    }
}