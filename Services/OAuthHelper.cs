﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Configuration;
using Orchard;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Security;
using System.Security.Cryptography;

namespace RM.QuickLogOn.OAuth.RU.Services
{
    public static class OAuthHelper
    {
        private static MD5 _md5 = MD5.Create();

        public static string Encrypt(this IEncryptionService service, string value)
        {
            return Convert.ToBase64String(service.Encode(Encoding.UTF8.GetBytes(value)));
        }

        public static string Decrypt(this IEncryptionService service, string value)
        {
            return Encoding.UTF8.GetString(service.Decode(Convert.FromBase64String(value)));
        }

        public static T FromJson<T>(Stream stream) where T : class
        {
            var js = new DataContractJsonSerializer(typeof(T));
            return js.ReadObject(stream) as T;
        }

        public static string HexMD5(string text)
        {
            return string.Join(string.Empty, _md5.ComputeHash(Encoding.UTF8.GetBytes(text)).Select(x => string.Format("{0:x2}", x)));
        }

        public static IWebProxy GetProxy()
        {
            var httpProxy = WebConfigurationManager.AppSettings["HttpProxy"];
            if(string.IsNullOrEmpty(httpProxy)) return null;
            var parts = httpProxy.Split(";".ToCharArray(), StringSplitOptions.None);
            var url = parts[0];
            var user = parts.Length > 2 ? parts[1] : null;
            var password = parts.Length > 3 ? parts[2] : null;
            var bypassList = parts.Length > 4 ? parts[3].Split(',') : null;

            var p = new WebProxy(url);
            if(bypassList != null && bypassList.Length > 0)
            {
                p.BypassProxyOnLocal = true;
                p.BypassList = bypassList;
            }
            if(string.IsNullOrEmpty(user))
            {
                p.Credentials = new NetworkCredential(user, password ?? string.Empty);
            }
            return p;
        }
    }
}
