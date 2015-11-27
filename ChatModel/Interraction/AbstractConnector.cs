using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace ChatModel.Interraction {
    public abstract class AbstractConnector : IDisposable {
        protected Socket Socket;
        protected NetworkStream NetworkStream;
        protected StreamWriter Writer;
        protected StreamReader Reader;
        private bool _disposing;

        protected void Connect(Socket socket) {
            Socket = socket;
            NetworkStream = new NetworkStream(socket);
            Reader = new StreamReader(NetworkStream, Encoding.UTF8);
            Writer = new StreamWriter(NetworkStream, Encoding.UTF8);
        }

        public void Dispose() {
            if (_disposing)
                return;
            Disconnect();
        }

        private void Disconnect() {
            _disposing = true;

            Reader?.Close();
            Writer?.Close();
            NetworkStream?.Close();
            if (Socket == null) {
                return;
            }
            if (Socket.Connected) {
                Socket.Shutdown(SocketShutdown.Both);
            }
            Socket.Close();
        }
    }
}
