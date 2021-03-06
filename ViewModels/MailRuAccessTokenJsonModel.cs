﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace RM.QuickLogOn.OAuth.RU.ViewModels
{
    [DataContract]
    public class MailRuAccessTokenJsonModel
    {
        [DataMember]
        public string access_token { get; set; }
        
        [DataMember]
        public string expires_in { get; set; }

        [DataMember]
        public string refresh_token { get; set; }

        [DataMember]
        public string x_mailru_vid { get; set; }
    }
}
