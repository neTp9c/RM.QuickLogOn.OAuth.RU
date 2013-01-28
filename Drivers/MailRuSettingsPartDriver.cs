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
    [OrchardFeature("RM.QuickLogOn.OAuth.RU")]
    public class MailRuSettingsPartDriver : ContentPartDriver<MailRuSettingsPart>
    {
        private readonly IEncryptionService _service;

        public Localizer T { get; set; }

        public MailRuSettingsPartDriver(IEncryptionService service)
        {
            _service = service;
            T = NullLocalizer.Instance;
        }

        protected override string Prefix { get { return "MailRuSettings"; } }

        protected override DriverResult Editor(MailRuSettingsPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_MailRu_SiteSettings",
                               () => shapeHelper.EditorTemplate(TemplateName: "Parts.MailRu.SiteSettings", Model: part, Prefix: Prefix)).OnGroup("QuickLogOn.RU");
        }

        protected override DriverResult Editor(MailRuSettingsPart part, IUpdateModel updater, dynamic shapeHelper)
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
