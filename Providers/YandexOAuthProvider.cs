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
    [OrchardFeature("RM.QuickLogOn.OAuth.RU.Yandex")]
    public class YandexOAuthProvider : IQuickLogOnProvider
    {
        public const string Url = "https://oauth.yandex.ru/authorize?response_type=code&client_id={0}&state={1}";

        public string Name
        {
            get { return "Yandex"; }
        }

        public string Description
        {
            get { return "LogOn with Your Yandex account"; }
        }

        public string GetLogOnUrl(WorkContext context)
        {
            var urlHelper = new UrlHelper(context.HttpContext.Request.RequestContext);
            var part = context.CurrentSite.As<YandexSettingsPart>();
            var clientId = part.ClientId;
            var returnUrl = context.HttpContext.Request.Url;
            return string.Format(Url, clientId, urlHelper.Encode(returnUrl.ToString()));
        }
    }
}
