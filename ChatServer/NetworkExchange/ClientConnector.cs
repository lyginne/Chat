using System;
using System.Net.Sockets;
using ChatModel;
using ChatModel.DataModel;
using ChatModel.Interraction;
using ChatServer.DataBase;
using ChatServer.NetworkExchange.Broadcaster;

namespace ChatServer.NetworkExchange  {
    class ClientConnector : AbstractConnector, IBroadcasterClient  {
        private User _user;

        public ClientConnector(Socket socket) {
            Socket = socket;
            try {
                Connect(Socket);
            }
            catch (Exception e) {
                Console.WriteLine("Не цепануться к сокету" + e.ToString());
            }
            string requestString=null;
            try {
                requestString = Reader.ReadLine();
            }
            catch (Exception e) {
                Console.WriteLine("Первое же чтение после ацепта упало" + e.ToString());
            }
            
            if (requestString==null) {
                Console.WriteLine("Клиент отвалился");
                Dispose();
                return;
            }
            if (requestString.Substring(0,2).Equals(ClientMesageHeader.Authorize)) {
                OnAutorizeRequest(requestString);
            }
            else if (requestString.Substring(0,2).Equals(ClientMesageHeader.Register)) {
                OnRegisterRequest(requestString);
            }
            else {
                Console.WriteLine("Неавторизованный запрос");
                Dispose();
            }
        }

        private void OnAutorizeRequest(string message) {
            _user = MessageAnalyserHelper.GetServerUserFromString(message);
            if (_user.Username.Length < 3) {
                SendServerResponse(ServerMesageHeader.UsernameOrPasswordIncorrect);
                Dispose();
                return;
            }
            if (DataBaseManager.GetInstance().VerifyUser(_user)) {
                Authorize();
            }
            else {
                SendServerResponse(ServerMesageHeader.UsernameOrPasswordIncorrect);
                Dispose();
            }

        }

        private void OnRegisterRequest(string message) {
           _user = MessageAnalyserHelper.GetServerUserFromString(message);
            if (_user.Username.Length < 3) {
                SendServerResponse(ServerMesageHeader.UsernameOrPasswordIncorrect);
                Dispose();
                return;
            }
            if (!DataBaseManager.GetInstance().CheckUserExistance(_user)) {
                DataBaseManager.GetInstance().AddUserToDataBase(_user);
                Authorize();
            }
            else {
                SendServerResponse(ServerMesageHeader.UserExist);
                Dispose();
            }
        }

        private void Authorize() {
            try {
                SendServerResponse(ServerMesageHeader.OK);
            }
            catch (Exception) {
                throw;
            }
            Broadcaster.Broadcaster.GetInstance().AddBroadcasterClient(this);
            WaitForMessages();
        }

        private void WaitForMessages() {
            while (true) {
                string requestString;
                try {
                    requestString = Reader.ReadLine();
                }
                catch (Exception) {
                    Broadcaster.Broadcaster.GetInstance().RemoveBroadcasterClient(this);
                    Console.WriteLine("Клиент отвалился");
                    Dispose();
                    return;
                }
                
                if (requestString == null) {
                    Broadcaster.Broadcaster.GetInstance().RemoveBroadcasterClient(this);
                    Console.WriteLine("Клиент ушел");
                    Dispose();
                    return;
                }
                if (!requestString.Substring(0, 2).Equals(ClientMesageHeader.Message)) {
                    Broadcaster.Broadcaster.GetInstance().RemoveBroadcasterClient(this);
                    Console.WriteLine("Клиент шлет не сообщения (хедер инвалид), отключаем");
                    Dispose();
                    return;
                }
                Broadcaster.Broadcaster.GetInstance().BroadcastMessage($"{_user.Username}: {requestString.Substring(2)}");
            }
        }

        private void SendServerResponse(string response) {
            Writer.WriteLine(response);
            Writer.Flush();
        }

        #region IBroadcasterClient

        public string GetUsername() {
            return _user.Username;
        }

        public void OnMessageRecieved(string message) {
            SendServerResponse($"{ServerMesageHeader.Message}{message}");
        }

        public void OnUserCame(string newUserName) {
            SendServerResponse($"{ServerMesageHeader.NewUser}{newUserName}");
        }

        public void OnUserQuit(string username) {
            SendServerResponse($"{ServerMesageHeader.UserQuit}{username}");
        }

        #endregion

    }
}
