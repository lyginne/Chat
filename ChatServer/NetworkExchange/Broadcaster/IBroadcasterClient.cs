using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.NetworkExchange.Broadcaster {
    interface IBroadcasterClient : IDisposable {
        string GetUsername(); 
        void OnMessageRecieved(string message);
        void OnUserCame(string newUsername);
        void OnUserQuit(string username);
    }
}
