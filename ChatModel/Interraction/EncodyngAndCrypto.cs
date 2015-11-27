using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatModel {
    public static class EncodyngAndCrypto {
        public static Encoding encoding = Encoding.UTF8;
        public static System.Security.Cryptography.SHA256Managed hashingAlgorytm = new System.Security.Cryptography.SHA256Managed();
    }
}
