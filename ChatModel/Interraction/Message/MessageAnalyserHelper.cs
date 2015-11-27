using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatModel.DataModel;

namespace ChatModel {
    public static class MessageAnalyserHelper {
        public static User GetServerUserFromString(string serverUserString) {
            string hashedPasswordBase64 = serverUserString.Substring(2, 44);
            string username = serverUserString.Substring(46, serverUserString.Length - 46);
            return new User(username,hashedPasswordBase64);
            
        }
    }
}
