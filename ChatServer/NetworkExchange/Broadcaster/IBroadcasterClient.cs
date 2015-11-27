using System;

namespace ChatServer.NetworkExchange.Broadcaster {
    interface IBroadcasterClient : IDisposable {
        string GetUsername(); 
        void OnMessageRecieved(string message);
        void OnUserCame(string newUsername);
        void OnUserQuit(string username);
    }
}
