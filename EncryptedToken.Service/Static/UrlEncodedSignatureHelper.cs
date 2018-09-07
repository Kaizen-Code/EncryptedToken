using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EncryptedToken.Service.Static
{
    public class UrlEncodedSignatureHelper : SignatureHelper
    {
        internal new static string GetSignature(IDictionary<string, string> claims, string secretKey)
        {
            var Base64 = SignatureHelper.GetSignature(claims, secretKey);
            return HttpUtility.UrlEncode(Base64);
        }
        //internal new static string ToBase64String(IDictionary<string, string> claims)
        //{
        //    var Base64 = SignatureHelper.ToBase64String(claims);
        //    return HttpUtility.UrlEncode(Base64);
        //}
        //internal new static IDictionary<string, string> FromBase64String(string encodedBase64UrlSafeValue)
        //{
        //    var decodedBase64 = HttpUtility.UrlDecode(encodedBase64UrlSafeValue);

        //    return SignatureHelper.FromBase64String(decodedBase64);
        //}

    }
}
