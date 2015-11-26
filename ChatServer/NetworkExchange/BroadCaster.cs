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
        private static LimitedQueue<String> hundreedMessages; 

        public static void Initialize() {
            _broadcaster = new Broadcaster();
        }

        public static Broadcaster GetInstance() {
            return _broadcaster;
        }

        private Broadcaster() {
            usersOnline = new List<ChatClient>();
            hundreedMessages = new LimitedQueue<string>(100);
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

        private void BroadcastNewUsersList() {
            
        }

        public void BroadcastMessageFrom(Socket Sender, string message) {
            lock (this) {
                String sendingString="";
                var firstOrDefault = usersOnline.FirstOrDefault(x => x.Socket.Equals(Sender));
                if (firstOrDefault != null) {
                    sendingString = $"{firstOrDefault.User.Username}: {message}";
                    hundreedMessages.Enqueue(sendingString);
                }
                else {
                    throw new Exception("Юзер удален, почему он вообще что-то шлет?");
                }
                foreach (var user in usersOnline) {
                    byte[] outputBuffer = MessageBuilderHelper.GetBytesToMessageSendRequest(sendingString);
                    user.Socket.Send(outputBuffer, outputBuffer.Length,SocketFlags.None);
                }
            }
        }
    }
}
