using EncryptedToken.Service.DataStore;
using EncryptedToken.Service.Static;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EncryptedToken.Service.TokenService
{
    class EncryptedTokenService : ITokenService
    {

        private const char TokenSeparator = '.';
        private const char Comma = ',';
        

        private const string RANDOM = "00";
        private const string USERNAME = "10";
        private const string DATE = "20";
        private const string EXP = "30";
        private const string LIFESECONDS = "40";
        private const string ID = "50";
        private const string ROLEIDS = "60";
        private const string OTHERINFO = "70";

        
        private readonly static DateTime utc0;
        private readonly static Random _random;
        //internal static int LastAllowedTokenRefreshSeconds;
        private static string RandomNumber
        {
            get
            {
                return _random.Next(1000, 9999).ToString();
            }
        }
        private static string _secreteKeyValue;

        static EncryptedTokenService()
        {
            _random = new Random();
            //LastAllowedTokenRefreshSeconds = 60;
            utc0 = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        }// close static constructor 

        private string _encryptedValue;
        private IDictionary<string, string> payload;
        private bool _isValid;


        public string UserName
        {
            get
            {
                if (this._isValid && this.payload.ContainsKey(USERNAME))
                    return this.payload[USERNAME];
                else
                    return string.Empty;
            }
        }

        private string _roles;
        public string Roles
        {
            get
            {
                if (_isValid)
                {
                    if (string.IsNullOrEmpty(this._roles))
                    {
                        //load from database
                        using (var data = new SecureContext())
                        {
                            var id = int.Parse(payload[ID]);
                            _roles = data.Tokens.FirstOrDefault(c => c.Id == id).Roles;
                            //var roleIds = this.payload[ROLEIDS].Split(Comma).Select(int.Parse).ToArray();
                            //_roles = string.Join(",", data.UserRoles.Where(c => roleIds.Contains(c.Id)).Select(c => c.RoleTitle).ToArray());
                        }
                    }
                    return _roles;
                }
                else
                    return string.Empty;
                
            }
        }

        public string OtherInfo
        {
            get
            {
                if (payload.ContainsKey(OTHERINFO))
                    return payload[OTHERINFO];
                else
                    return string.Empty;
            }
        }

        public string GetUniqueTokenValue(string userName, string commaSeparatedRoles, int lifeMinutes, string otherInfo = null)
        {
            if(string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(commaSeparatedRoles))
                return string.Empty;
            lifeMinutes = (lifeMinutes < 2) ? 2 : lifeMinutes;
            var lifeSeconds = lifeMinutes * 60;
            var issueTime = DateTime.UtcNow;
            var issueSeconds = (int)issueTime.Subtract(utc0).TotalSeconds;

            var service = new EncryptedTokenService();
            service.payload.Add(USERNAME, userName);
            service.payload.Add(LIFESECONDS, lifeSeconds.ToString());
            service.payload.Add(OTHERINFO, otherInfo);
            service.payload.Add(RANDOM, EncryptedTokenService.RandomNumber);
            service.payload.Add(DATE, issueSeconds.ToString());
            service.payload.Add(EXP, (issueSeconds + lifeSeconds).ToString());
            using (var data = new SecureContext())
            {
                var rolesArray = commaSeparatedRoles.Split(Comma).ToArray();
                int[] RoleIds = data.UserRoles.Where(c => rolesArray.Contains(c.RoleTitle)).Select(c => c.Id).ToArray();
                service.payload.Add(ROLEIDS, string.Join(Comma.ToString(), RoleIds));
                var obj = new DataStore.Token
                {
                    UserName = userName,
                    Roles = commaSeparatedRoles,
                    CreatedDateTime = issueTime,
                    ExpiryDateTime = issueTime.AddSeconds(lifeSeconds),
                    EncryptedValue = string.Empty
                };
                data.Tokens.Add(obj);
                data.SaveChanges();
                service.payload.Add(ID, obj.Id.ToString()); //service.payload.Add(ID, obj.Id.ToString());
                service._encryptToken();
                obj.EncryptedValue = service._encryptedValue;
                data.SaveChanges();
            }

            return service._encryptedValue;
        }
        private int[] _allowedPermissions;
        public void LoadAllowedPermissions()
        {
            var roleIds = this.payload[ROLEIDS].Split(Comma).Select(int.Parse).ToArray();
            // load allowed permissions from database 
            using (var data = new SecureContext())
            {
                _allowedPermissions = data.RolePermissionMaps.Where(c => roleIds.Contains(c.RoleId)).Select(c => c.PermissionId).Distinct().ToArray();
            }
        }
        public bool IsAllowed(int permissionId)
        {
            
            if (_isValid)
            {
                if (_allowedPermissions != null)
                {
                    return _allowedPermissions.Contains(permissionId);
                }
                var roleIds = this.payload[ROLEIDS].Split(Comma).Select(int.Parse).ToArray();
                using (var data = new SecureContext())
                {
                    var obj = data.RolePermissionMaps.FirstOrDefault(c => c.PermissionId == permissionId && roleIds.Contains(c.RoleId));
                    if (obj != null)
                        return true;
                }
            }
            return false;

        }
        public bool IsValid => _isValid;
        
        public EncryptedTokenService()
        {
            this._isValid = false;
            this.payload = new Dictionary<string, string>();
            this._encryptedValue = string.Empty;
        }
        private void _decryptToken()
        {
            //this.payload = new Dictionary<string, string>();
            var values = this._encryptedValue.Split(TokenSeparator);
            if (values.Length > 1)
                this.payload = UrlEncodedSignatureHelper.FromBase64String(values[1]);
        }
        public bool Validate(string uniqueTokenValue)
        {
            if (string.IsNullOrEmpty(uniqueTokenValue))
                return false;
            _encryptedValue = uniqueTokenValue;
            this._decryptToken();
            if (this.payload == null)
                return false;
            foreach (var item in new string[]{
                RANDOM,
                USERNAME,
                DATE,
                EXP,
                LIFESECONDS,
                ID,
                ROLEIDS,
                OTHERINFO
            })
                if (!this.payload.ContainsKey(item) || string.IsNullOrEmpty(item))
                    return false;

            int iat;
            if (!int.TryParse(this.payload[DATE], out iat))
                return false;
            int exp;
            if (!int.TryParse(this.payload[EXP], out exp))
                return false;
            int seconds;
            if (!int.TryParse(this.payload[LIFESECONDS], out seconds))
                return false;
            int tokenId;
            if (!int.TryParse(this.payload[ID], out tokenId))
                return false;
            if (utc0.AddSeconds(exp) <= DateTime.UtcNow)
                return false;
            // do more code if needed
            if (string.IsNullOrEmpty(_secreteKeyValue))
                using (var context = new SecureContext())
                {
                    _secreteKeyValue = context.Configs.FirstOrDefault(c => c.ItemKey == SignatureHelper.SECRETEKEYNAME).ItemValue;
                }
            var tokenValues = this._encryptedValue.Split(TokenSeparator);
            this._isValid = tokenValues[0].Equals(UrlEncodedSignatureHelper.GetSignature(this.payload, string.Format("{0}{1}{2}", this.payload[RANDOM], _secreteKeyValue, this.payload[EXP])));
            return this._isValid;
        }

        

        private DateTime ExpTime
        {
            get
            {
                return utc0.AddSeconds(double.Parse(this.payload[EXP]));
            }
        }
        private int LifeTime
        {
            get
            {
                return (int)(this.ExpTime - DateTime.UtcNow).TotalSeconds;
            }
        }
        private void _encryptToken()
        {
            if (string.IsNullOrEmpty(_secreteKeyValue))
                using (var context = new SecureContext())
                {
                    _secreteKeyValue = context.Configs.FirstOrDefault(c => c.ItemKey == SignatureHelper.SECRETEKEYNAME).ItemValue;
                }
            var sign = UrlEncodedSignatureHelper.GetSignature(this.payload, string.Format("{0}{1}{2}", this.payload[RANDOM],_secreteKeyValue, this.payload[EXP]));
            var base64 = UrlEncodedSignatureHelper.ToBase64String(this.payload);
            _encryptedValue = string.Join(TokenSeparator.ToString(), sign, base64, LifeTime /*- EncryptedTokenService.LastAllowedTokenRefreshSeconds */);
        }
        public string Refresh()
        {
            if (_isValid)
            {
                //if (this.LifeTime > LastAllowedTokenRefreshSeconds)
                //    return string.Empty;
                this._encryptedValue = string.Empty;
                this.payload[RANDOM] = EncryptedTokenService.RandomNumber;
                var newTotalSeconds = ((int)DateTime.UtcNow.Subtract(utc0).TotalSeconds) + int.Parse(this.payload[LIFESECONDS]);
                this.payload[EXP] = newTotalSeconds.ToString();
                this._encryptToken();
                //Save refreshed token into database
                using (var data = new SecureContext())
                {
                    var id = int.Parse(this.payload[ID]);
                    var obj = data.Tokens.FirstOrDefault(c => c.Id == id);
                    if (obj == null)
                        return string.Empty;
                    obj.ExpiryDateTime = utc0.AddSeconds(newTotalSeconds);
                    obj.EncryptedValue = this._encryptedValue;
                    data.SaveChanges();
                }
                return this._encryptedValue;
            }// close if valid
            return string.Empty;
        }

    }

   


    


}
