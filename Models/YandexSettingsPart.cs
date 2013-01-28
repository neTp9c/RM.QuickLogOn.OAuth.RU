using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace RM.QuickLogOn.OAuth.RU.Models
{
    [OrchardFeature("RM.QuickLogOn.OAuth.RU.Yandex")]
    public class YandexSettingsPart : ContentPart<YandexSettingsPartRecord>
    {
        [Required(ErrorMessage = "Mail.ru ClientId is required")]
        public string ClientId { get { return Record.ClientId; } set { Record.ClientId = value; } }

        public string ClientSecret { get; set; }
    }
}
