using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptedToken.Service.TokenService
{
    public interface ITokenService
    {
        string UserName { get; }
        string Roles { get; }

        string OtherInfo { get; }

        string GetUniqueTokenValue(string userName, string commaSeparatedRoles, int lifeMinutes, string otherInfo = null);

        bool Validate(string uniqueTokenValue);

        bool IsValid { get; }

        string Refresh();

        void LoadAllowedPermissions();
        bool IsAllowed(int permissionId);

        

    }
}
