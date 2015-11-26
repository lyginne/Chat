using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using ChatModel;
using ChatServer.DataBase;
using ChatServer.NetworkExchange.Broadcaster;

namespace ChatServer  {
    class ClientRequestsListener : IBroadcasterClient  {
        private Socket _socket;
        private readonly NetworkStream _networkStream;
        private readonly StreamWriter _writer;
        private readonly StreamReader _reader;
        private ServerUser _serverUser;
        string userExist="1";
        string IncorrectLoginOrPassword= "2";

        public ClientRequestsListener(Socket socket) {
            _socket = socket;
            _networkStream = new NetworkStream(socket);
            _reader = new StreamReader(_networkStream,Encoding.UTF8);
            _writer = new StreamWriter(_networkStream, Encoding.UTF8);
            string requestString = _reader.ReadLine();
            if (requestString[0] == '0') {
                OnAutorizeRequest(requestString);
            }
            else if (requestString[0] == '1') {
                OnRegisterRequest(requestString);
            }
            else {
                Console.WriteLine("Неавторизованный запрос");
                Dispose();
            }
        }

        private void OnAutorizeRequest(string message) {
            _serverUser = MessageAnalyserHelper.GetServerUserFromString(message);
            if (_serverUser.Username.Length < 3) {
                SendServerResponse("1");
                Dispose();
                return;
            }
            if (DataBaseManager.GetInstance().VerifyUser(_serverUser)) {
                Authorize();
            }
            else {
                SendServerResponse("1");
                Dispose();
            }

        }

        private void OnRegisterRequest(string message) {
           _serverUser = MessageAnalyserHelper.GetServerUserFromString(message);
            if (_serverUser.Username.Length < 3) {
                SendServerResponse("1");
                Dispose();
                return;
            }
            if (!DataBaseManager.GetInstance().CheckUserExistance(_serverUser)) {
                DataBaseManager.GetInstance().AddUserToDataBase(_serverUser);
                Authorize();
            }
            else {
                SendServerResponse("2");
                Dispose();
            }
        }

        private void Authorize() {
            try {
                SendServerResponse("0");
            }
            catch (Exception) {
                throw;
            }
            Broadcaster.GetInstance().AddBroadcasterClient(this);
            WaitForMessages();
        }

        private void WaitForMessages() {
            while (true) {
                string requestString=null;
                try {
                    requestString = _reader.ReadLine();
                }
                catch (Exception) {
                    Broadcaster.GetInstance().RemoveBroadcasterClient(this);
                }
                
                if (requestString == null) {
                    Broadcaster.GetInstance().RemoveBroadcasterClient(this);
                    Console.WriteLine("Клиент ушел");
                    Dispose();
                    return;
                }
                Broadcaster.GetInstance().BroadcastMessage($"{_serverUser.Username}: {requestString.Substring(2)}");
            }
        }

        private void SendServerResponse(string response) {
            _writer.WriteLine(response);
            _writer.Flush();
        }

        #region IBroadcasterClient

        public string GetUsername() {
            return _serverUser.Username;
        }



        public void OnMessageRecieved(string message) {
            _writer.WriteLine("3|"+message);
            _writer.Flush();
        }

        public void OnUserCame(string newUserName) {
            _writer.WriteLine("4|"+newUserName);
            _writer.Flush();
        }

        public void OnUserQuit(string username) {
            _writer.WriteLine("5|"+username);
            _writer.Flush();
        }

        #endregion

        public void Dispose() {
            Disconnect();
        }

        private void Disconnect() {
            _reader?.Close();
            _writer?.Close();
            _networkStream?.Close();
            if (_socket == null) {
                return;
            }
            if (_socket.Connected) {
                _socket.Shutdown(SocketShutdown.Both);
            }
            _socket.Close();
        }
    }
}
