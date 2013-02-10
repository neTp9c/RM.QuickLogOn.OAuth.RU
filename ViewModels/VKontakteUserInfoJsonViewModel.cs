using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RM.QuickLogOn.OAuth.RU.ViewModels
{
    [DataContract]
    public class VKontakteUserInfoJsonViewModel
    {
        public class UserInfo
        {
            [DataMember]
            public string uid { get; set; }

            [DataMember]
            public string first_name { get; set; }
            
            [DataMember]
            public string last_name { get; set; }
        }

        [DataMember]
        public UserInfo[] response { get; set; }
    }
}
