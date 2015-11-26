using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Chat;
using ChatClient.Connector.Interfaces;
using ChatModel;

namespace ChatClient.Connector {
    //Синглтон для общения с сервером
    class Connector : IConnectorObservable, IDisposable {

        private static Connector _connector;
        private Socket _socket;
        private XMLSettings XMLsettings;
        private bool _connected;
        private const string settingPath="settings.xml";

        public static Connector Instance {
            get { return _connector; }
        }
        #region Connstructors and Initializers
        public static Connector InitializeConnector() {
            _connector?.Disconnect();
            _connector = new Connector(settingPath);
            
            return _connector;
        }

        private Connector(string settingsPath) {
            Observers = new List<IConnectorObserver>();
            XMLsettings = new XMLSettings(settingsPath);
            


        }
        #endregion

        #region Destructors
        public void Disconnect() {
            lock (this) {
                if (_socket != null) {
                    _connected = false;
                    if (_socket.Connected) {
                        _socket.Shutdown(SocketShutdown.Both);
                        _socket.Close();
                        _socket = null;
                    }
                }
            }
        }
        public void Dispose() {
            _connector = null;
            Disconnect();
        }
        #endregion

        public void Connect() {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Connect(XMLsettings.IpAddress, XMLsettings.Port);
        }

        #region ButtonRegistration and Autorisation
        public void Authorize(string username, string password) {
            try {
                Connect();
            }
            catch {
                NotifyObserversErrorOcured("Невозможно подключиться к серверу");
                return;
            }
            NetworkStream networkStream = new NetworkStream(_socket);
            StreamReader reader = new StreamReader(networkStream, Encoding.UTF8);
            StreamWriter writer = new StreamWriter(networkStream, Encoding.UTF8);
            writer.WriteLine(String.Format("0|{0}, {1}", Convert.ToBase64String(EncodyngAndCryptoInformation.hashingAlgorytm.ComputeHash(Encoding.UTF8.GetBytes(password))), username));
            writer.Flush();
            string response = reader.ReadLine();
            AnalyzeUserPasswordRequestResult(response[0]);
            //            string resut = reader.ReadLine();
            //            if (resut.Equals("0|")) {
            //                AnalyzeUserPasswordRequestResult((byte)ChatModel.Rrules.Ok);
            //            }


            //            byte[] sendBuffer = ChatModel.MessageBuilderHelper.GetBytesToAutorisationRequest(username, password);
            //            byte[] inputbuffer = new byte[1];
            //              
            //            try {
            //                //_socket.Send(sendBuffer, sendBuffer.Length, SocketFlags.None);
            //                //_socket.Receive(inputbuffer, 0, 1, SocketFlags.None);
            //            }
            //            catch {
            //                NotifyObserversErrorOcured("Ошибка соединения, авторизация невозможна");
            //                Disconnect();
            //                return;
            //            }


            //else {

            //AnalyzeUserPasswordRequestResult(inputbuffer[0]);
            //}

        }

        public void Register(string username, string password) {
            try {
                Connect();
            }
            catch {
                NotifyObserversErrorOcured("Невозможно подключиться к серверу");
                return;
            }
            NetworkStream networkStream = new NetworkStream(_socket);
            StreamReader reader = new StreamReader(networkStream, Encoding.UTF8);
            StreamWriter writer = new StreamWriter(networkStream, Encoding.UTF8);
            writer.WriteLine(String.Format("1|{0}, {1}", Convert.ToBase64String(EncodyngAndCryptoInformation.hashingAlgorytm.ComputeHash(Encoding.UTF8.GetBytes(password))), username));
            writer.Flush();
            string response = reader.ReadLine();
            AnalyzeUserPasswordRequestResult(response[0]);

        }
        #endregion

        private void AnalyzeUserPasswordRequestResult(char result) {
            if (result == '0') {
                NotifyObserversUserPasswordOperationSucced();
                StartSession();
            }
            else if (result == '2') {
                NotifyObserversErrorOcured("Юзер существует");
                Disconnect();
            }
            else if (result == '1') {
                NotifyObserversErrorOcured("Некорректное имя пользователя или пароль");
                Disconnect();
            }
            else {
                NotifyObserversErrorOcured("Неожиданная ошибка");
                Disconnect();
            }
        }

        private void AnalyzeUserPasswordRequestResult(byte result) {
            if (result == (byte)ChatModel.Rrules.Ok) {
                NotifyObserversUserPasswordOperationSucced();
                StartSession();
            }
            else if (result == (byte)ChatModel.Rrules.UserExists) {
                NotifyObserversErrorOcured("Юзер существует");
                Disconnect();
            }
            else if (result == (byte)ChatModel.Rrules.IncorrectLoginOrPassword) {
                NotifyObserversErrorOcured("Некорректное имя пользователя или пароль");
                Disconnect();
            }
            else {
                NotifyObserversErrorOcured("Неожиданная ошибка");
                Disconnect();
            }
        }

        #region Session
        private void StartSession() {
            _connected = true;
           new Thread(Session).Start();
        }

        private void Session() {
            byte[] inputbuffer = new byte[1024];
            int count = 0;
            while (_connected) {
                try {
                    count = _socket.Receive(inputbuffer, 0, inputbuffer.Length, SocketFlags.None);
                    
                }
                catch {
                    if (_connected) { //Нас прервали, ибо мы умираем или почему-то ещё?
                        //Если не умираем, сообщить, что соединение упало
                        NotifyObserversErrorOcured("Сервер больше не отвечает");
                        Disconnect();
                        return;
                    }
                }
                if (count == 0) {
                    NotifyObserversErrorOcured("Сервер закрыл подключение");
                    Disconnect();
                    return;
                }
                if (inputbuffer[0] == (byte) ChatModel.Rrules.Message) {
                    string message = Encoding.UTF8.GetString(inputbuffer, 1, count - 1);
                    NotifyObserversMessageRecieved(message);
                }
                else if(inputbuffer[0] == (byte)ChatModel.Rrules.UsersList) {
                    string message = Encoding.UTF8.GetString(inputbuffer, 1, count - 1);
                    NotifyObserversUsersListRecieved(message);
                }
                else if (inputbuffer[0] == (byte)ChatModel.Rrules.ErrorOcured) {
                    string message = Encoding.UTF8.GetString(inputbuffer, 1, count - 1);
                    NotifyObserversErrorOcured(message);
                    Disconnect();
                    return;

                }
            }
        }

        public void Send(string message) {
            byte[] byteMessage = Encoding.UTF8.GetBytes(message);
            byte[] sendBuffer = new byte[byteMessage.Length + 1];
            sendBuffer[0] = (byte) ChatModel.Rrules.Message;
            byteMessage.CopyTo(sendBuffer, 1);
            try {
                _socket.Send(sendBuffer, 0, sendBuffer.Length, SocketFlags.None);
            }
            catch {
                NotifyObserversErrorOcured("Сервер не отвечает");
                Disconnect();
            }
            
        }
        #endregion

        #region IConnectorObservable Implementation

        private List<IConnectorObserver> Observers { get; }

        public void AddObserver(IConnectorObserver observer) {
            Observers.Add(observer);
        }

        public void RemoveObserver(IConnectorObserver observer) {
            Observers.Remove(observer);
        }

        public void NotifyObserversUserPasswordOperationSucced() {
            foreach (var observer in Observers) {
                observer.OnUserPasswordOperationSuceed();
            }
        }

        public void NotifyObserversMessageRecieved(string s) {
            foreach (var observer in Observers) {
                observer.OnMessageRecieved(s);
            }
        }

        public void NotifyObserversUsersListRecieved(string usersList) {
            foreach (var observer in Observers) {
                observer.OnUsersListRecieved(usersList);
            }
            
        }

        public void NotifyObserversErrorOcured(string s) {
            foreach (var observer in Observers) {
                observer.OnErrorOcurs(s);
            }
        }
        #endregion

        
    }
}
