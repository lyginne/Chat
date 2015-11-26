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

        public static void Initialize() {
            _broadcaster = new Broadcaster();
        }

        public static Broadcaster GetInstance() {
            return _broadcaster;
        }

        private Broadcaster() {
            usersOnline = new List<ChatClient>();
        }

        public void AddOnlineUser(ChatClient chatClient) {
            lock (this) {
                usersOnline.Add(chatClient);
                BroadcastNewUsersList();
            }
            
        }
        public void RemoveOnlineUser(ChatClient chatClient) {
            lock (this) {
                usersOnline.Add(chatClient);
                BroadcastNewUsersList();
            }

        }

        ~Broadcaster() {
            ;
            ;
            ;
        }
        private void BroadcastNewUsersList() {
            
        }

        public void BroadcastMessageFrom(User sender, string message) {
            
        }
        public void BroadcastMessageFrom(Socket Sender, string message) {
            lock (this) {
                var firstOrDefault = usersOnline.FirstOrDefault(x => x.Socket.Equals(Sender));
                if (firstOrDefault != null) {
                    String SendingString = firstOrDefault.User.Username + message;
                }
                else {
                    throw new Exception("Юзер удален, почему он вообще что-то шлет?");
                }
            }
        }
        public void BroadcastMessageFrom(ChatClient Sender, string message) {

        }

    }
}
