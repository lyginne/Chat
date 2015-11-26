using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Chat;
using ChatClient.Connector.Interfaces;

namespace ChatClient.Connector {
    //Синглтон для общения с сервером
    class Connector : IConnectorObservable {

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


        ~Connector() {
            Disconnect();
        }
        #endregion

        #region ButtonRegistration and Autorisation
        public void Authorize(string username, string password) {

            try {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.Connect(IPAddress.Parse("127.0.0.1"), 12345);
            }
            catch {
                NotifyObserversErrorOcured("Невозможно подключиться к серверу");
                return;
            }

            byte[] sendBuffer = ChatModel.MessageBuilderHelper.GetBytesToAutorisationRequest(username, password);
            byte[] inputbuffer = new byte[1];
              
            try {
                _socket.Send(sendBuffer, sendBuffer.Length, SocketFlags.None);
                _socket.Receive(inputbuffer, 0, 1, SocketFlags.None);
            }
            catch {
                NotifyObserversErrorOcured("Ошибка соединения, авторизация невозможна");
                Disconnect();
                return;
            }
            
            
            if (inputbuffer[0] == (byte) ChatModel.Rrules.Ok) {
                NotifyObserversAuthorized();
                StartSession();

            }
            else if (inputbuffer[0] == (byte) ChatModel.Rrules.IncorrectLoginOrPassword) {
                NotifyObserversErrorOcured("Неверный логин или пароль");
                Disconnect();
            }
            else {
                NotifyObserversErrorOcured("Сервер сообщил об ошибке, о которой не должен был сообщать");
                Disconnect();
            }

        }

        public void Register(string username, string password) {
            try {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.Connect(IPAddress.Parse("127.0.0.1"), 12345);
            }
            catch {
                NotifyObserversErrorOcured("Невозможно подключиться к серверу");
                return;
            }

            byte[] sendbuffer = ChatModel.MessageBuilderHelper.GetBytesToRegistrationRequest(username, password);
            byte[] inputbuffer = new byte[1];

            try {
                _socket.Send(sendbuffer, sendbuffer.Length, SocketFlags.None);
                _socket.Receive(inputbuffer, 0, 1, SocketFlags.None);
            }
            catch (Exception e) {
                NotifyObserversErrorOcured("Ошибка подключения, невозможно зарегистрироваться");
                Disconnect();
                return;
            }
            if (inputbuffer[0] == (byte)ChatModel.Rrules.UserExists) {
                NotifyObserversErrorOcured("Юзер существует");
                Disconnect();
            }
            else if (inputbuffer[0] == (byte)ChatModel.Rrules.Ok) {
                NotifyObserversRegistered();
                StartSession();
            }
            else if (inputbuffer[0] == (byte)ChatModel.Rrules.IncorrectLoginOrPassword) {
                NotifyObserversErrorOcured("Некорректное имя пользователя или пароль");
                Disconnect();
            }
            else {
                NotifyObserversErrorOcured("Неожиданная ошибка");
                Disconnect();
            }

        }
        #endregion

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
                if(inputbuffer[0] == (byte)ChatModel.Rrules.UsersList) {
                    string message = Encoding.UTF8.GetString(inputbuffer, 1, count - 1);
                    NotifyObserversUsersListRecieved(message);
                }
                if (inputbuffer[0] == (byte)ChatModel.Rrules.ErrorOcured) {
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

        public void NotifyObserversRegistered() {
            foreach (var observer in Observers) {
                observer.OnRegistrationSucced();
            }
        }

        public void NotifyObserversAuthorized() {
            foreach (var observer in Observers) {
                observer.OnAutorizationSucceed();
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
