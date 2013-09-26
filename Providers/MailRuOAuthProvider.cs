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
    [OrchardFeature("RM.QuickLogOn.OAuth.RU")]
    public class MailRuOAuthProvider : IQuickLogOnProvider
    {
        public const string Url = "https://connect.mail.ru/oauth/authorize?client_id={0}&response_type=code&redirect_uri={1}";

        private Localizer T = NullLocalizer.Instance;

        public string Name
        {
            get { return T("MailRu").ToString(); }
        }

        public string Description
        {
            get { return T("LogOn with Your Mail.RU account").ToString(); }
        }

        public string GetLogOnUrl(WorkContext context)
        {
            var urlHelper = new UrlHelper(context.HttpContext.Request.RequestContext);
            var part = context.CurrentSite.As<MailRuSettingsPart>();
            var clientId = part.ClientId;
            var returnUrl = context.HttpContext.Request.Url;
            var redirectUrl = new Uri(returnUrl, urlHelper.Action("Auth", "MailRuOAuth", new { Area = "RM.QuickLogOn.OAuth.RU", ReturnUrl = urlHelper.Encode(returnUrl.ToString()) })).ToString(); //
            return string.Format(Url, clientId, urlHelper.Encode(redirectUrl));
        }
    }
}
