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
using RM.QuickLogOn.OAuth.RU.Models;
using RM.QuickLogOn.OAuth.RU.Services;
using RM.QuickLogOn.OAuth.RU.ViewModels;
using RM.QuickLogOn.Providers;
using RM.QuickLogOn.Services;

namespace RM.QuickLogOn.OAuth.RU.Services
{
    public interface IYandexOAuthService : IDependency
    {
        QuickLogOnResponse Auth(WorkContext wc, string code, string error, string returnUrl);
    }

    [OrchardFeature("RM.QuickLogOn.OAuth.RU.Yandex")]
    public class YandexOAuthService : IYandexOAuthService
    {
        public const string TokenRequestUrl = "https://oauth.yandex.ru/token";
        public const string EmailRequestUrl = "https://login.yandex.ru/info?format=json&oauth_token={0}";

        private readonly IQuickLogOnService _quickLogOnService;
        private readonly IEncryptionService _oauthHelper;

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public YandexOAuthService(IEncryptionService oauthHelper, IQuickLogOnService quickLogOnService)
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
                var part = wc.CurrentSite.As<YandexSettingsPart>();
                var clientId = part.ClientId;
                var clientSecret = _oauthHelper.Decrypt(part.Record.EncryptedClientSecret);

                var wr = WebRequest.Create(TokenRequestUrl);
                wr.Proxy = OAuthHelper.GetProxy();
                wr.ContentType = "application/x-www-form-urlencoded";
                wr.Method = "POST";
                using (var stream = wr.GetRequestStream())
                using (var ws = new StreamWriter(stream))
                {
                    ws.Write("grant_type=authorization_code&");
                    ws.Write("code={0}&", code);
                    ws.Write("client_id={0}&", clientId);
                    ws.Write("client_secret={0}", clientSecret);
                }
                var wres = wr.GetResponse();
                using (var stream = wres.GetResponseStream())
                {
                    var result = OAuthHelper.FromJson<YandexAccessTokenJsonModel>(stream);
                    return result.access_token;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
            }
            
            return null;
        }

        private string GetEmailAddress(string token)
        {
            try
            {
                var wr = WebRequest.Create(string.Format(EmailRequestUrl, token));
                wr.Method = "GET";
                wr.Proxy = OAuthHelper.GetProxy();
                var wres = wr.GetResponse();
                using (var stream = wres.GetResponseStream())
                {
                    var result = OAuthHelper.FromJson<YandexEmailAddressJsonViewModel>(stream);
                    return result != null ? result.default_email : (result.emails != null ? result.emails.FirstOrDefault() : null);
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
                    var email = GetEmailAddress(token);
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
            return new QuickLogOnResponse { Error = T("LogOn through Yandex failed: {0}", error), ReturnUrl = returnUrl };
        }
    }
}
