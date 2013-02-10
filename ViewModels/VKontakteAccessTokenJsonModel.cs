using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace RM.QuickLogOn.OAuth.RU.ViewModels
{
    [DataContract]
    public class VKontakteAccessTokenJsonModel
    {
        [DataMember]
        public string access_token { get; set; }

        [DataMember]
        public string user_id { get; set; }

        [DataMember]
        public string expires_in { get; set; }
    }
}
