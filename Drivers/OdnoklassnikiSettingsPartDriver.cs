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
    [OrchardFeature("RM.QuickLogOn.OAuth.RU.Odnoklassniki")]
    public class OdnoklassnikiSettingsPartDriver : ContentPartDriver<OdnoklassnikiSettingsPart>
    {
        private readonly IEncryptionService _service;

        public Localizer T { get; set; }

        public OdnoklassnikiSettingsPartDriver(IEncryptionService service)
        {
            _service = service;
            T = NullLocalizer.Instance;
        }

        protected override string Prefix { get { return "OdnoklassnikiSettings"; } }

        protected override DriverResult Editor(OdnoklassnikiSettingsPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_Odnoklassniki_SiteSettings",
                               () => shapeHelper.EditorTemplate(TemplateName: "Parts.Odnoklassniki.SiteSettings", Model: part, Prefix: Prefix)).OnGroup("QuickLogOn.RU");
        }

        protected override DriverResult Editor(OdnoklassnikiSettingsPart part, IUpdateModel updater, dynamic shapeHelper)
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
