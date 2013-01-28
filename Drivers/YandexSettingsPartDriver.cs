using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Security;
using RM.QuickLogOn.OAuth.RU.Models;
using RM.QuickLogOn.OAuth.RU.Services;

namespace RM.QuickLogOn.OAuth.RU.Drivers
{
    [OrchardFeature("RM.QuickLogOn.OAuth.RU.Yandex")]
    public class YandexSettingsPartDriver : ContentPartDriver<YandexSettingsPart>
    {
        private readonly IEncryptionService _service;

        public Localizer T { get; set; }

        public YandexSettingsPartDriver(IEncryptionService service)
        {
            _service = service;
            T = NullLocalizer.Instance;
        }

        protected override string Prefix { get { return "YandexSettings"; } }

        protected override DriverResult Editor(YandexSettingsPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_Yandex_SiteSettings",
                               () => shapeHelper.EditorTemplate(TemplateName: "Parts.Yandex.SiteSettings", Model: part, Prefix: Prefix)).OnGroup("QuickLogOn.RU");
        }

        protected override DriverResult Editor(YandexSettingsPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            if(updater.TryUpdateModel(part, Prefix, null, null))
            {
                if(!string.IsNullOrWhiteSpace(part.ClientSecret))
                {
                    part.Record.EncryptedClientSecret = _service.Encrypt(part.ClientSecret);
                }
            }
            return Editor(part, shapeHelper);
        }
    }
}
