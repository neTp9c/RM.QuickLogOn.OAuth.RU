using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace RM.QuickLogOn.OAuth.RU.ViewModels
{
    [DataContract]
    public class YandexAccessTokenJsonModel
    {
        [DataMember]
        public string access_token { get; set; }
        
        [DataMember]
        public string expires_in { get; set; }
    }
}
