using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatModel {
    public class ServerMesageHeader {
        public const string OK = "0|";
        public const string UsernameOrPasswordIncorrect = "1|";
        public const string UserExist = "2|";
        public const string Message = "3|";
        public const string NewUser = "4|";
        public const string UserQuit = "5|";
    }
}
