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
        protected bool Disposing;

        protected void Connect(Socket socket) {
            Socket = socket;
            NetworkStream = new NetworkStream(socket);
            Reader = new StreamReader(NetworkStream, EncodyngAndCrypto.Encoding);
            Writer = new StreamWriter(NetworkStream, Encoding.UTF8);
        }

        public void Dispose() {
            if (Disposing)
                return;
            Disconnect();
        }

        private void Disconnect() {
            Disposing = true;

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
