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

namespace RM.QuickLogOn.OAuth.RU.Providers
{
    [OrchardFeature("RM.QuickLogOn.OAuth.RU.Odnoklassniki")]
    public class OdnoklassnikiOAuthProvider : IQuickLogOnProvider
    {
        public const string Url = "https://accounts.google.com/o/oauth2/auth?response_type=code&client_id={0}&redirect_uri={1}&scope={2}&state={3}";
        public const string Scope = "https://www.googleapis.com/auth/userinfo.email";

        public string Name
        {
            get { return "Odnoklassniki"; }
        }

        public string Description
        {
            get { return "LogOn with Your Odnoklassniki account"; }
        }

        public string GetLogOnUrl(WorkContext context)
        {
            var urlHelper = new UrlHelper(context.HttpContext.Request.RequestContext);
            var part = context.CurrentSite.As<OdnoklassnikiSettingsPart>();
            var clientId = part.ClientId;
            var returnUrl = context.HttpContext.Request.Url;
            var redirectUrl = new Uri(returnUrl, urlHelper.Action("Auth", "OdnoklassnikiOAuth", new { Area = "RM.QuickLogOn.OAuth.RU" })).ToString();
            return string.Format(Url, clientId, urlHelper.Encode(redirectUrl), urlHelper.Encode(Scope), urlHelper.Encode(returnUrl.ToString()));
        }
    }
}
