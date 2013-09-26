using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using RM.QuickLogOn.OAuth.RU.Models;
using RM.QuickLogOn.Providers;
using System.Web.Mvc;
using Orchard.Localization;

namespace RM.QuickLogOn.OAuth.RU.Providers
{
    [OrchardFeature("RM.QuickLogOn.OAuth.RU.VKontakte")]
    public class VKontakteOAuthProvider : IQuickLogOnProvider
    {
        public const string Url = "https://oauth.vk.com/authorize?client_id={0}&redirect_uri={1}&response_type=code";
        public const string Scope = "";

        private Localizer T = NullLocalizer.Instance;

        public string Name
        {
            get { return T("VKontakte").ToString(); }
        }

        public string Description
        {
            get { return T("LogOn with Your VKontakte account").ToString(); }
        }

        public string GetLogOnUrl(WorkContext context)
        {
            var urlHelper = new UrlHelper(context.HttpContext.Request.RequestContext);
            var part = context.CurrentSite.As<VKontakteSettingsPart>();
            var clientId = part.ClientId;
            var returnUrl = context.HttpContext.Request.Url;
            var redirectUrl = new Uri(returnUrl, urlHelper.Action("Auth", "VKontakteOAuth", new { Area = "RM.QuickLogOn.OAuth.RU", returnUrl = urlHelper.Encode(returnUrl.ToString()) })).ToString();
            return string.Format(Url, clientId, urlHelper.Encode(redirectUrl)); // , urlHelper.Encode(returnUrl.ToString())
        }
    }
}
