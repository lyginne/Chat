using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatModel {
    public abstract class AbstractConnector : IDisposable {
        private Socket socket;
        private NetworkStream networkStream;
        private StreamWriter writer;
        private StreamReader reader;
        private bool connected;

        private void Connect(Socket socket) {
            networkStream = new NetworkStream(socket);
            reader = new StreamReader(networkStream, Encoding.UTF8);
            writer = new StreamWriter(networkStream, Encoding.UTF8);
        }

        private void Disconnect() {
            if (reader != null) {
                reader.Close();
            }
            if (writer != null) {
                writer.Close();
            }
            if (networkStream != null) {
                networkStream.Close();
            }
            if (socket == null) {
                return;
            }
            if (socket.Connected) {
                socket.Shutdown(SocketShutdown.Both);
            }
            socket.Close();
        }

        public void Dispose() {
            Disconnect();
        }
    }
}
