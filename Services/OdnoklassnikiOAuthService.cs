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
    public interface IOdnoklassnikiOAuthService : IDependency
    {
        QuickLogOnResponse Auth(WorkContext wc, string code, string error, string returnUrl);
    }

    [OrchardFeature("RM.QuickLogOn.OAuth.RU.Odnoklassniki")]
    public class OdnoklassnikiOAuthService : IOdnoklassnikiOAuthService
    {
        public const string TokenRequestUrl = "http://api.odnoklassniki.ru/oauth/token.do";
        public const string UserInfoRequestUrl = "http://api.odnoklassniki.ru/fb.do?method=users.getCurrentUser&access_token={0}&application_key={1}&sig={2}";

        private readonly IQuickLogOnService _quickLogOnService;
        private readonly IEncryptionService _oauthHelper;

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public OdnoklassnikiOAuthService(IEncryptionService oauthHelper, IQuickLogOnService quickLogOnService)
        {
            _quickLogOnService = quickLogOnService;
            _oauthHelper = oauthHelper;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        private string GetAccessToken(WorkContext wc, string code)
        {
            try
            {
                var part = wc.CurrentSite.As<OdnoklassnikiSettingsPart>();
                var clientId = part.ClientId;
                var clientSecret = _oauthHelper.Decrypt(part.Record.EncryptedClientSecret);

                var urlHelper = new UrlHelper(wc.HttpContext.Request.RequestContext);
                var redirectUrl =
                    new Uri(wc.HttpContext.Request.Url,
                            urlHelper.Action("Auth", "OdnoklassnikiOAuth", new { Area = "RM.QuickLogOn.OAuth.RU" })).ToString();

                var wr = WebRequest.Create(TokenRequestUrl);
                wr.Proxy = OAuthHelper.GetProxy();
                wr.ContentType = "application/x-www-form-urlencoded";
                wr.Method = "POST";
                using (var stream = wr.GetRequestStream())
                using (var ws = new StreamWriter(stream))
                {
                    ws.Write("code={0}&", code);
                    ws.Write("redirect_uri={0}&", redirectUrl);
                    ws.Write("grant_type=authorization_code&");
                    ws.Write("client_id={0}&", clientId);
                    ws.Write("client_secret={0}", clientSecret);
                }
                var wres = wr.GetResponse();
                using (var stream = wres.GetResponseStream())
                {
                    var result = OAuthHelper.FromJson<OdnoklassnikiAccessTokenJsonModel>(stream);
                    return result.access_token;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
            }
            
            return null;
        }

        private string GetEmailAddress(WorkContext wc, string token)
        {
            try
            {
                var part = wc.CurrentSite.As<OdnoklassnikiSettingsPart>();
                var clientPublicId = part.ClientPublicId;
                var clientSecret = _oauthHelper.Decrypt(part.Record.EncryptedClientSecret);

                var sig = OAuthHelper.HexMD5(string.Format("application_key={0}method=users.getCurrentUser{1}", clientPublicId, OAuthHelper.HexMD5(token+clientSecret)));

                var wr = WebRequest.Create(string.Format(UserInfoRequestUrl, token, clientPublicId, sig));
                wr.Method = "GET";
                wr.Proxy = OAuthHelper.GetProxy();
                var wres = wr.GetResponse();
                using (var stream = wres.GetResponseStream())
                {
                    var result = OAuthHelper.FromJson<OdnoklassnikiUserInfoJsonModel>(stream);
                    return result != null ? string.Format("{0}.{1}@{2}.odnoklassniki.ru", result.first_name, result.last_name, result.uid) : null;
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
                var token = GetAccessToken(wc, code);
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
            return new QuickLogOnResponse { Error = T("LogOn through Odnoklassniki failed: {0}", error), ReturnUrl = returnUrl };
        }
    }
}
