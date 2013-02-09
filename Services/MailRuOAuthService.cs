using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Security;
using System.Net;
using RM.QuickLogOn.OAuth.RU.Services;
using RM.QuickLogOn.Providers;
using RM.QuickLogOn.Services;
using RM.QuickLogOn.OAuth.RU.Models;
using RM.QuickLogOn.OAuth.RU.ViewModels;

namespace RM.QuickLogOn.OAuth.RU.Services
{
    public interface IMailRuOAuthService : IDependency
    {
        QuickLogOnResponse Auth(WorkContext wc, string code, string error, string returnUrl);
    }

    [OrchardFeature("RM.QuickLogOn.OAuth.RU")]
    public class MailRuOAuthService : IMailRuOAuthService
    {
        public const string TokenRequestUrl = "https://connect.mail.ru/oauth/token";
        public const string EmailRequestUrl = "http://www.appsmail.ru/platform/api?method=users.getInfo&secure=1&app_id={0}&session_key={1}&sig={2}";
        public const string SigParams = "method=users.getInfosecure=1app_id={0}session_key={1}{2}";


        private readonly IQuickLogOnService _quickLogOnService;
        private readonly IEncryptionService _oauthHelper;

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public MailRuOAuthService(IEncryptionService oauthHelper, IQuickLogOnService quickLogOnService)
        {
            _quickLogOnService = quickLogOnService;
            _oauthHelper = oauthHelper;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        private string GetAccessToken(WorkContext wc, string code, string returnUrl)
        {
            try
            {
                var part = wc.CurrentSite.As<MailRuSettingsPart>();
                var clientId = part.ClientId;
                var clientSecret = _oauthHelper.Decrypt(part.Record.EncryptedClientSecret);

                var urlHelper = new UrlHelper(wc.HttpContext.Request.RequestContext);
                var redirectUrl = new Uri(wc.HttpContext.Request.Url, urlHelper.Action("Auth", "MailRuOAuth", new { Area = "RM.QuickLogOn.OAuth.RU" })).ToString(); //, ReturnUrl = returnUrl.ToString()

                var wr = WebRequest.Create(TokenRequestUrl);
                wr.Proxy = OAuthHelper.GetProxy();
                wr.ContentType = "application/x-www-form-urlencoded";
                wr.Method = "POST";
                using (var stream = wr.GetRequestStream())
                using (var ws = new StreamWriter(stream, Encoding.UTF8))
                {
                    ws.Write("client_id={0}&", clientId);
                    ws.Write("client_secret={0}&", clientSecret);
                    ws.Write("grant_type=authorization_code&");
                    ws.Write("code={0}&", code);
                    ws.Write("redirect_uri={0}", redirectUrl);
                }
                var wres = wr.GetResponse();
                using (var stream = wres.GetResponseStream())
                {
                    var result = OAuthHelper.FromJson<MailRuAccessTokenJsonModel>(stream);
                    return result.access_token;
                }
            }
            catch (Exception ex)
            {
                var body = new StreamReader((ex as WebException).Response.GetResponseStream()).ReadToEnd();
                Logger.Error(ex, body);
            }
            
            return null;
        }

        private string GetEmailAddress(WorkContext wc, string token)
        {
            try
            {
                var part = wc.CurrentSite.As<MailRuSettingsPart>();
                var clientId = part.ClientId;
                var clientSecret = _oauthHelper.Decrypt(part.Record.EncryptedClientSecret);

                var sigParams = string.Format(SigParams, clientId, token, clientSecret);

                var md5 = System.Security.Cryptography.MD5.Create();

                var sig = string.Join(string.Empty, md5.ComputeHash(Encoding.UTF8.GetBytes(sigParams)).Select(x=>string.Format("{0:x2}", x)));

                var wr = WebRequest.Create(string.Format(EmailRequestUrl, clientId, token, sig));
                wr.Method = "GET";
                wr.Proxy = OAuthHelper.GetProxy();
                var wres = wr.GetResponse();
                using (var stream = wres.GetResponseStream())
                {
                    var result = OAuthHelper.FromJson<MailRuEmailAddressJsonViewModel[]>(stream);
                    return result != null && result.Any() ? result[0].email : null;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
            }
            return null;
        }

        public QuickLogOnResponse Auth(WorkContext wc, string code, string error, string returnUrl)
        {
            if (string.IsNullOrEmpty(code) && string.IsNullOrEmpty(error))
            {
                error = "invalid code";
            }
            else
            {
                var token = GetAccessToken(wc, code, returnUrl);
                if (!string.IsNullOrEmpty(token))
                {
                    var email = GetEmailAddress(wc, token);
                    if (!string.IsNullOrEmpty(email))
                    {
                        return _quickLogOnService.LogOn(new QuickLogOnRequest
                        {
                            Email = email,
                            Login = email,
                            RememberMe = false,
                            ReturnUrl = returnUrl
                        });
                    }
                    error = "invalid email";
                }
                else
                {
                    error = "invalid token";
                }
            }
            return new QuickLogOnResponse { Error = T("LogOn through Mail.ru failed: {0}", error), ReturnUrl = returnUrl };
        }
    }
}
