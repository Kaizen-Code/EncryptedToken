using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EncryptedToken.Service.Static
{
    public class SignatureHelper
    {
        #region public
        public static Encoding TextEncoding = Encoding.UTF8;

        #endregion

        #region internal
        internal const string SECRETEKEYNAME = "kaizen-SECRETEKEY";

        internal static string GetSignature(IDictionary<string, string> claims, string secretKey)
        {
            string data = buildData(claims);
            if (string.IsNullOrEmpty(data))
                return null;
            byte[] keyByte = TextEncoding.GetBytes(secretKey);
            HMACSHA256 hmacsha256 = new HMACSHA256(keyByte);
            byte[] messageBytes = TextEncoding.GetBytes(data);
            return Convert.ToBase64String(hmacsha256.ComputeHash(messageBytes));
        }
        internal static string ToBase64String(IDictionary<string, string> claims)
        {
            string data = UrlEncodedSignatureHelper.buildData(claims);
            if (string.IsNullOrEmpty(data))
                return null;
            return Convert.ToBase64String(TextEncoding.GetBytes(data));
        }
        internal static IDictionary<string, string> FromBase64String(string encodedBase64)
        {
            if (string.IsNullOrEmpty(encodedBase64))
                return new Dictionary<string, string>();
            var fields = TextEncoding.GetString(DecodeBytes(encodedBase64));
            return UrlEncodedSignatureHelper.unbuildData(fields);
        }
        
        #endregion

        #region private 
        private const char FieldSeparator = '|';
        private const char ValueSeparator = ':';

        private static string buildData(IDictionary<string, string> claims)
        {
            var InvalidPair = claims.Where(c => c.Key.Contains(FieldSeparator) || c.Key.Contains(ValueSeparator)
                || (!string.IsNullOrEmpty(c.Value) && (c.Value.Contains(FieldSeparator) || c.Value.Contains(ValueSeparator)))).Count();
            if (InvalidPair > 0)
            {
                throw new ArgumentException(string.Format("Invalid Key/Values : '{0}' and '{1}' is not allowed!",FieldSeparator,ValueSeparator));
            }
            IList<string> dataToSign = new List<string>();
            foreach (var item in claims)
            {
                dataToSign.Add(item.Key + UrlEncodedSignatureHelper.ValueSeparator.ToString() + item.Value);
            }
            return string.Join(UrlEncodedSignatureHelper.FieldSeparator.ToString(), dataToSign);
        }
        private static IDictionary<string, string> unbuildData(string arg)
        {
            var claims = new Dictionary<string, string>();
            foreach (var item in arg.Split(UrlEncodedSignatureHelper.FieldSeparator))
            {
                var pair = item.Split(UrlEncodedSignatureHelper.ValueSeparator);
                claims.Add(pair[0], pair[1]);
            }
            return claims;
        }

        private const char Base64PadCharacter = '=';
        private const char Base64Character62 = '+';
        private const char Base64Character63 = '/';
        private const char Base64UrlCharacter62 = '-';
        private const char Base64UrlCharacter63 = '_';

        private static byte[] DecodeBytes(string arg)
        {
            if (String.IsNullOrEmpty(arg))
            {
                throw new ApplicationException("String to decode cannot be null or empty.");
            }

            StringBuilder s = new StringBuilder(arg);
            s.Replace(Base64UrlCharacter62, Base64Character62);
            s.Replace(Base64UrlCharacter63, Base64Character63);

            int pad = s.Length % 4;
            s.Append(Base64PadCharacter, (pad == 0) ? 0 : 4 - pad);

            return Convert.FromBase64String(s.ToString());
        }

        #endregion 

    }
}
