using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;

namespace RM.QuickLogOn.OAuth.RU.Models
{
    [OrchardFeature("RM.QuickLogOn.OAuth.RU.VKontakte")]
    public class VKontakteSettingsPartRecord : ContentPartRecord
    {
        public virtual string ClientId { get; set; }
        public virtual string EncryptedClientSecret { get; set; }
    }
}
