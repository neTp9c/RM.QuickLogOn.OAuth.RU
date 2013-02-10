using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Mvc.Extensions;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Themes;
using Orchard.UI.Notify;
using RM.QuickLogOn.OAuth.RU.Services;
using RM.QuickLogOn.OAuth.RU.ViewModels;

namespace RM.QuickLogOn.OAuth.RU.Controllers
{
    [HandleError, Themed]
    [OrchardFeature("RM.QuickLogOn.OAuth.RU.VKontakte")]
    public class VKontakteOAuthController : Controller
    {
        private readonly IOrchardServices _services;
        private readonly IVKontakteOAuthService _oauthService;

        public Localizer T { get; set; }

        public VKontakteOAuthController(IOrchardServices services, IVKontakteOAuthService oauthService)
        {
            T = NullLocalizer.Instance;
            _services = services;
            _oauthService = oauthService;
        }

        public ActionResult Auth(VKontakteOAuthAuthViewModel model)
        {
            var response = _oauthService.Auth(_services.WorkContext, model.Code, model.Error, model.ReturnUrl);
            if (response.Error != null)
            {
                _services.Notifier.Add(NotifyType.Error, response.Error);
            }
            
            return this.RedirectLocal(response.ReturnUrl);
        }
    }
}
