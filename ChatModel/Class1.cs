﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatModel {
    static class EncodyngAndCryptoInformation {
        public static Encoding encoding = Encoding.UTF8;
        public static System.Security.Cryptography.SHA256Managed hashingAlgorytm = new System.Security.Cryptography.SHA256Managed();
        
    }
}
