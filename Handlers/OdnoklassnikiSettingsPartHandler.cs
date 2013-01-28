using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using RM.QuickLogOn.OAuth.RU.Models;

namespace RM.QuickLogOn.OAuth.RU.Handlers
{
    [OrchardFeature("RM.QuickLogOn.OAuth.RU.Odnoklassniki")]
    public class OdnoklassnikiSettingsPartHandler : ContentHandler
    {
        public Localizer T { get; set; }

        public OdnoklassnikiSettingsPartHandler(IRepository<OdnoklassnikiSettingsPartRecord> repository)
        {
            Filters.Add(new ActivatingFilter<OdnoklassnikiSettingsPart>("Site"));
            Filters.Add(StorageFilter.For(repository));
            T = NullLocalizer.Instance;
        }

        protected override void GetItemMetadata(GetContentItemMetadataContext context)
        {
            if (context.ContentItem.ContentType != "Site")
                return;
            base.GetItemMetadata(context);
            context.Metadata.EditorGroupInfo.Add(new GroupInfo(T("QuickLogOn.RU")) { Id = "QuickLogOn.RU" });
        }
    }
}
