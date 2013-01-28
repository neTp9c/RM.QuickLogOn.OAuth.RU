using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RM.QuickLogOn.OAuth.RU.ViewModels
{
    [DataContract]
    public class YandexEmailAddressJsonViewModel
    {
        [DataMember]
        public string default_email { get; set; }

        [DataMember]
        public IEnumerable<string> emails { get; set; }
    }
}
