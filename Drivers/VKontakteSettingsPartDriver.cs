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
    [OrchardFeature("RM.QuickLogOn.OAuth.RU.VKontakte")]
    public class VKontakteSettingsPartDriver : ContentPartDriver<VKontakteSettingsPart>
    {
        private readonly IEncryptionService _service;

        public Localizer T { get; set; }

        public VKontakteSettingsPartDriver(IEncryptionService service)
        {
            _service = service;
            T = NullLocalizer.Instance;
        }

        protected override string Prefix { get { return "VKontakteSettings"; } }

        protected override DriverResult Editor(VKontakteSettingsPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_VKontakte_SiteSettings",
                               () => shapeHelper.EditorTemplate(TemplateName: "Parts.VKontakte.SiteSettings", Model: part, Prefix: Prefix)).OnGroup("QuickLogOn.RU");
        }

        protected override DriverResult Editor(VKontakteSettingsPart part, IUpdateModel updater, dynamic shapeHelper)
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
