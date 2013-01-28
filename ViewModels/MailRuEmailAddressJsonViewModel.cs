using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RM.QuickLogOn.OAuth.RU.ViewModels
{
    [DataContract]
    public class MailRuEmailAddressJsonViewModel
    {
        [DataMember]
        public string email { get; set; }

        [DataMember]
        public bool verified_email { get; set; }
    }
}
