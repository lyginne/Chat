using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatServer.NetworkExchange.Broadcaster {
    class Broadcaster : IBroadcasterObservable {
        private static Broadcaster _broadcaster;
        private static LimitedQueue<String> _hundreedMessages; 
        private static List<IBroadcasterClient> _clientsOnline;

        public static void Initialize() {
            _broadcaster = new Broadcaster();
        }

        public static Broadcaster GetInstance() {
            return _broadcaster;
        }

        private Broadcaster() {
            _clientsOnline =new List<IBroadcasterClient>();
            _hundreedMessages = new LimitedQueue<string>(100);
        }

        private void SendHeundreedMessagesToUser(IBroadcasterClient chatClient) {
            foreach (var message in _hundreedMessages) {
                chatClient.OnMessageRecieved(message);
            }
        }

        private void SendUsersOnlineListToUser(IBroadcasterClient chatClient) {
            foreach (var client in _clientsOnline) {
                chatClient.OnUserCame(client.GetUsername());
            }
           
        }

        public void BroadcastMessage(string message) {
            lock (this) {
                _hundreedMessages.Enqueue(message);
                NotifyClientsMessageRecieved(message);
            }
        }

        #region IBroadcasterObservable

        public void AddBroadcasterClient(IBroadcasterClient chatClient) {
            lock (this) {
                try {
                    SendHeundreedMessagesToUser(chatClient);
                    SendUsersOnlineListToUser(chatClient);
                }
                catch (Exception) {
                    chatClient.Dispose();
                }

                _clientsOnline.Add(chatClient);
                NotifyClientsNewUserCame(chatClient.GetUsername());


            }
        }

        public void RemoveBroadcasterClient(IBroadcasterClient chatClient) {
            lock (this) {
                _clientsOnline.Remove(chatClient);
                chatClient.Dispose();
                NotifyClientsUserQuit(chatClient.GetUsername());
            }

        }

        public void NotifyClientsNewUserCame(string user) {
            for (int i = 0; i < _clientsOnline.Count; i++) {
                var observer = _clientsOnline.ElementAt(i);
                try {
                    observer.OnUserCame(user);
                }
                catch (Exception) {
                    RemoveBroadcasterClient(observer);
                    observer.Dispose();
                    i--;
                }
            }
        }

        public void NotifyClientsMessageRecieved(string message) {
            for (int i = 0; i < _clientsOnline.Count; i++) {
                var observer = _clientsOnline.ElementAt(i);
                try {
                    observer.OnMessageRecieved(message);
                }
                catch (Exception) {
                    RemoveBroadcasterClient(observer);
                    observer.Dispose();
                    i--;
                }
            }
        }

        public void NotifyClientsUserQuit(string user) {
            for (int i=0;i<_clientsOnline.Count;i++) {
                var observer = _clientsOnline.ElementAt(i);
                try {
                    observer.OnUserQuit(user);
                }
                catch (Exception) {
                    RemoveBroadcasterClient(observer);
                    observer.Dispose();
                    i--;
                }
            }
        }

        #endregion

    }
}
