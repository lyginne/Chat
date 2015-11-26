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
    class Connector :  IConnectorObservable, IDisposable {

        private static Connector _connector;
        private Socket _socket;
        private readonly XMLSettings _xmlSettings;
        private bool _connected;
        private const string settingPath="settings.xml";
        private NetworkStream networkStream;
        private StreamReader reader;
        private StreamWriter writer;

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
            _xmlSettings = new XMLSettings(settingsPath);
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
            _socket.Connect(_xmlSettings.IpAddress, _xmlSettings.Port);
            networkStream = new NetworkStream(_socket);
            reader = new StreamReader(networkStream, Encoding.UTF8);
            writer = new StreamWriter(networkStream, Encoding.UTF8);
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

            writer.WriteLine(String.Format("0|{0}, {1}", Convert.ToBase64String(EncodyngAndCryptoInformation.hashingAlgorytm.ComputeHash(Encoding.UTF8.GetBytes(password))), username));
            writer.Flush();
            string response = reader.ReadLine();
            AnalyzeUserPasswordRequestResult(response[0]);

        }

        public void Register(string username, string password) {
            try {
                Connect();
            }
            catch {
                NotifyObserversErrorOcured("Невозможно подключиться к серверу");
                return;
            }
            writer.WriteLine(String.Format("1|{0}, {1}", Convert.ToBase64String(EncodyngAndCryptoInformation.hashingAlgorytm.ComputeHash(Encoding.UTF8.GetBytes(password))), username));
            writer.Flush();
            string response = reader.ReadLine();
            AnalyzeUserPasswordRequestResult(response[0]);

        }

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

        #endregion

        #region Session

        private void StartSession() {
            _connected = true;
           new Thread(Session).Start();
        }

        private void Session() {
            while (true) {
                string requestString = reader.ReadLine();
                if (requestString.Substring(0,2).Equals("3|")) {
                    NotifyObserversMessageRecieved(requestString.Substring(2)+"\r\n");
                }
                else if (requestString.Substring(0, 2).Equals("4|")) {
                    NotifyObserversNewUserJoined(requestString.Substring(2));
                }
                else if (requestString.Substring(0, 2).Equals("5|")) {
                    NotifyObserversUserQuit(requestString.Substring(2));
                }
            }
        }

        public void Send(string message) {
            try {
                writer.WriteLine("3|" + message);
                writer.Flush();
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

        public void NotifyObserversNewUserJoined(string usersList) {
            foreach (var observer in Observers) {
                observer.OnUserJoined(usersList);
            }
            
        }

        public void NotifyObserversErrorOcured(string s) {
            foreach (var observer in Observers) {
                observer.OnErrorOcurs(s);
            }
        }

        public void NotifyObserversUserQuit(string user) {
            foreach (var observer in Observers) {
                observer.OnUserQuit(user);
            }
        }

        #endregion

        
    }
}
