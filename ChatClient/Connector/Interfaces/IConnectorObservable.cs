namespace ChatClient.Connector.Interfaces {
    interface IConnectorObservable {
        void AddObserver(IConnectorObserver observer);
        void RemoveObserver(IConnectorObserver observer);
        void NotifyObserversUserPasswordOperationSucced();
        void NotifyObserversMessageRecieved(string s);
        void NotifyObserversNewUserJoined(string newUser);
        void NotifyObserversErrorOcured(string s);
        void NotifyObserversUserQuit(string user);
    }
}
