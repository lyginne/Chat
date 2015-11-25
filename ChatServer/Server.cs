using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatServer {
    class Server {
        public Server(IPAddress ipAddress, int port) {
            Socket socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(ipAddress,port));
            socket.Listen(100);
            while (true) {
                new Thread( new ParameterizedThreadStart(AcceptSocket)).Start(socket.Accept());
            }
        }

        private void AcceptSocket(Object socket) {
            new ClientRequestsListener((Socket) socket);

        }
    }
}
