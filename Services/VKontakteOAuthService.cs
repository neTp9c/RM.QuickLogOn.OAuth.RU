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
using System.Web;

namespace RM.QuickLogOn.OAuth.RU.Services
{
    public interface IVKontakteOAuthService : IDependency
    {
        QuickLogOnResponse Auth(WorkContext wc, string code, string error, string returnUrl);
    }

    [OrchardFeature("RM.QuickLogOn.OAuth.RU.VKontakte")]
    public class VKontakteOAuthService : IVKontakteOAuthService
    {
        public const string TokenRequestUrl = "https://oauth.vk.com/access_token";
        public const string UserInfoRequestUrl = "https://api.vk.com/method/users.get?uids={0}&fields=uid,first_name,last_name&access_token={1}";

        private readonly IQuickLogOnService _quickLogOnService;
        private readonly IEncryptionService _oauthHelper;

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public VKontakteOAuthService(IEncryptionService oauthHelper, IQuickLogOnService quickLogOnService)
        {
            _quickLogOnService = quickLogOnService;
            _oauthHelper = oauthHelper;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        private VKontakteAccessTokenJsonModel GetAccessToken(WorkContext wc, string code, string returnUrl)
        {
            try
            {
                var part = wc.CurrentSite.As<VKontakteSettingsPart>();
                var clientId = part.ClientId;
                var clientSecret = _oauthHelper.Decrypt(part.Record.EncryptedClientSecret);

                var urlHelper = new UrlHelper(wc.HttpContext.Request.RequestContext);
                var redirectUrl =
                    new Uri(wc.HttpContext.Request.Url,
                            urlHelper.Action("Auth", "VKontakteOAuth", new { Area = "RM.QuickLogOn.OAuth.RU", returnUrl = urlHelper.Encode(returnUrl) })).ToString();

                var wr = WebRequest.Create(TokenRequestUrl);
                wr.Proxy = OAuthHelper.GetProxy();
                wr.ContentType = "application/x-www-form-urlencoded";
                wr.Method = "POST";
                using (var stream = wr.GetRequestStream())
                using (var ws = new StreamWriter(stream))
                {
                    ws.Write("client_id={0}&", clientId);
                    ws.Write("client_secret={0}&", clientSecret);
                    ws.Write("code={0}&", code);
                    ws.Write("redirect_uri={0}&", redirectUrl);
                }
                var wres = wr.GetResponse();
                using (var stream = wres.GetResponseStream())
                {
                    var result = OAuthHelper.FromJson<VKontakteAccessTokenJsonModel>(stream);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
            }
            
            return null;
        }

        private string GetEmailAddress(string token, string userId)
        {
            try
            {
                var wr = WebRequest.Create(string.Format(UserInfoRequestUrl, userId, token));
                wr.Method = "GET";
                wr.Proxy = OAuthHelper.GetProxy();
                var wres = wr.GetResponse();
                using (var stream = wres.GetResponseStream())
                {
                    var result = OAuthHelper.FromJson<VKontakteUserInfoJsonViewModel>(stream);
                    var ui = result != null ? result.response.FirstOrDefault() : null;
                    return ui != null ? string.Format("{0}.{1}@{2}.vk.com", ui.first_name, ui.last_name, ui.uid) : null;
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
                error = T("invalid code").ToString();
            }
            else
            {
                var token = GetAccessToken(wc, code, returnUrl);
                if (token != null)
                {
                    var email = GetEmailAddress(token.access_token, token.user_id);
                    if (!string.IsNullOrEmpty(email))
                    {
                        returnUrl = HttpUtility.UrlDecode(returnUrl);
                        return _quickLogOnService.LogOn(new QuickLogOnRequest
                        {
                            Email = email,
                            Login = email,
                            RememberMe = false,
                            ReturnUrl = returnUrl
                        });
                    }
                    error = T("invalid email").ToString();
                }
                else
                {
                    error = T("invalid token").ToString();
                }
            }
            return new QuickLogOnResponse { Error = T("LogOn through VKontakte failed: {0}", error), ReturnUrl = returnUrl };
        }
    }
}
