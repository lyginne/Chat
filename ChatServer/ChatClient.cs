using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ChatModel;

namespace ChatServer {
    class ChatClient {
        public readonly User User;
        public readonly Socket Socket;
        public ChatClient(User user, Socket socket) {
            User = user;
            Socket = socket;
        }
    }
}
