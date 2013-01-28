using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RM.QuickLogOn.OAuth.RU.ViewModels
{
    public class VKontakteOAuthAuthViewModel
    {
        public string Code { get; set; }
        public string Error { get; set; }
        public string State { get; set; }
    }
}
