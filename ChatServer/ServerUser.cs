using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer {
    class ServerUser : ChatModel.User {
        public byte[] HashedPassword;

        ServerUser(string username, byte[] hashedPassword) {
            HashedPassword = hashedPassword;
            Username = username;
        }
    }
}
