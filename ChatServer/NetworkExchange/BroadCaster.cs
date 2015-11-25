using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ChatModel;

namespace ChatServer.NetworkExchange {
    class Broadcaster {
        private static Broadcaster _broadcaster;
        private static List<ChatClient> usersOnline;

        public void Initialize() {
            _broadcaster = new Broadcaster();
        }

        public static Broadcaster GetInstance() {
            return _broadcaster;
        }

        private Broadcaster() {
            usersOnline = new List<ChatClient>();
        }

        private void AddOnlineUser(ChatClient chatClient) {
            lock (this) {
                usersOnline.Add(chatClient);
                BroadcastNewUsersList();
            }
            
        }

        private void BroadcastNewUsersList() {
            
        }

        public void BroadcastMessageFrom(User Sender, string message) {
            
        }
        public void BroadcastMessageFrom(Socket Sender, string message) {

        }
        public void BroadcastMessageFrom(ChatClient Sender, string message) {

        }

    }
}
