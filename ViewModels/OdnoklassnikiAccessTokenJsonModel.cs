using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RM.QuickLogOn.OAuth.RU.ViewModels
{
    public class OdnoklassnikiAccessTokenJsonModel
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string refresh_token { get; set; }
    }
}
