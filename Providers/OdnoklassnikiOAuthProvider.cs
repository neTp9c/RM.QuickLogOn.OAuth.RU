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
        public const string Url = "http://www.odnoklassniki.ru/oauth/authorize?client_id={0}&response_type=code&redirect_uri={1}";

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
#warning Add returnUrl
            var redirectUrl = new Uri(returnUrl, urlHelper.Action("Auth", "OdnoklassnikiOAuth", new { Area = "RM.QuickLogOn.OAuth.RU" })).ToString(); // , urlHelper.Encode(returnUrl.ToString())
            return string.Format(Url, clientId, urlHelper.Encode(redirectUrl));
        }
    }
}
