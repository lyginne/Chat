using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ChatClient.Connector.Interfaces;
using ChatModel;
using ChatModel.Interraction;

namespace ChatClient.Connector {
    //Синглтон для общения с сервером
    class Connector : AbstractConnector,  IConnectorObservable, IDisposable {

        private static Connector _connector;
        private readonly XMLSettings _xmlSettings;
        private bool _disposing;
        private const string SettingPath="settings.xml";

        public static Connector GetInstance() {
            return _connector;
        }

        #region Connstructors and Initializers

        public static Connector InitializeConnector() {
            _connector?.Dispose();
            _connector = new Connector(SettingPath);
            
            return _connector;
        }

        private Connector(string settingsPath) {
            Observers = new List<IConnectorObserver>();
            _xmlSettings = new XMLSettings(settingsPath);
        }
        #endregion

        public new void Dispose() {
            base.Dispose();
            _connector = null;
        }

        public void Connect() {
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Socket.Connect(_xmlSettings.IpAddress, _xmlSettings.Port);
            base.Connect(Socket);
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

            Writer.WriteLine("{0}{1}{2}", ClientMesageHeader.Authorize, Convert.ToBase64String(EncodyngAndCrypto.hashingAlgorytm.ComputeHash(Encoding.UTF8.GetBytes(password))), username);
            Writer.Flush();
            string response = Reader.ReadLine();
            AnalyzeUserPasswordRequestResult(response);

        }

        public void Register(string username, string password) {
            try {
                Connect();
            }
            catch {
                NotifyObserversErrorOcured("Невозможно подключиться к серверу");
                return;
            }
            Writer.WriteLine("{0}{1}{2}", ClientMesageHeader.Register, Convert.ToBase64String(EncodyngAndCrypto.hashingAlgorytm.ComputeHash(Encoding.UTF8.GetBytes(password))), username);
            Writer.Flush();
            string response = Reader.ReadLine();
            AnalyzeUserPasswordRequestResult(response);

        }

        private void AnalyzeUserPasswordRequestResult(string result) {
            if (result.Equals(ServerMesageHeader.OK)) {
                NotifyObserversUserPasswordOperationSucced();
                StartSession();
            }
            else if (result.Equals(ServerMesageHeader.UserExist)) {
                NotifyObserversErrorOcured("Юзер существует");
                Dispose();
            }
            else if (result.Equals(ServerMesageHeader.UsernameOrPasswordIncorrect)) {
                NotifyObserversErrorOcured("Некорректное имя пользователя или пароль");
                Dispose();
            }
            else {
                NotifyObserversErrorOcured("Неожиданная ошибка");
                Dispose();
            }
        }

        #endregion

        #region Session

        private void StartSession() {
            _disposing = true;
           new Thread(Session).Start();
        }

        private void Session() {
            while (true) {
                string requestString=null;
                try {
                    requestString = Reader.ReadLine();
                }
                catch (Exception) {
                    if (_disposing) {
                        return;
                    }
                    NotifyObserversErrorOcured("Нет соединения с сервером");
                    
                }
                if (requestString == null) {
                    NotifyObserversErrorOcured("Сервер разорвал подключение");
                    return;
                }
                
                if (requestString.Substring(0,2).Equals(ServerMesageHeader.Message)) {
                    NotifyObserversMessageRecieved(requestString.Substring(2)+"\r\n");
                }
                else if (requestString.Substring(0, 2).Equals(ServerMesageHeader.NewUser)) {
                    NotifyObserversNewUserJoined(requestString.Substring(2));
                }
                else if (requestString.Substring(0, 2).Equals(ServerMesageHeader.UserQuit)) {
                    NotifyObserversUserQuit(requestString.Substring(2));
                }
            }
        }

        public void Send(string message) {
            try {
                Writer.WriteLine("{0}{1}", ClientMesageHeader.Message, message);
                Writer.Flush();
            }
            catch {
                NotifyObserversErrorOcured("Сервер не отвечает");
                Dispose();
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
