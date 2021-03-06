﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace RM.QuickLogOn.OAuth.RU.Models
{
    [OrchardFeature("RM.QuickLogOn.OAuth.RU.Odnoklassniki")]
    public class OdnoklassnikiSettingsPart : ContentPart<OdnoklassnikiSettingsPartRecord>
    {
        [Required(ErrorMessage = "Odnoklassniki ClientId is required")]
        public string ClientId { get { return Record.ClientId; } set { Record.ClientId = value; } }

        [Required(ErrorMessage = "Odnoklassniki ClientPublicId is required")]
        public string ClientPublicId { get { return Record.ClientPublicId; } set { Record.ClientPublicId = value; } }

        public string ClientSecret { get; set; }
    }
}
