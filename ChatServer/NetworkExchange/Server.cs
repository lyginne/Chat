using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ChatServer.NetworkExchange {
    class Server {
        public Server(IPAddress ipAddress, int port) {
            Socket socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(ipAddress,port));
            socket.Listen(100);
            while (true) {
                new Thread(AcceptSocket).Start(socket.Accept());
            }
        }

        private void AcceptSocket(Object socket) {
            new ClientConnector((Socket) socket);

        }
    }
}
