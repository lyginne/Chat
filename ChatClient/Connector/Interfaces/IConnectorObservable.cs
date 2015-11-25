using Chat;

namespace ChatClient.Connector.Interfaces {
    interface IConnectorObservable {
        void AddObserver(IConnectorObserver observer);
        void RemoveObserver(IConnectorObserver observer);
        void NotifyObserversRegistered();
        void NotifyObserversAuthorized();
        void NotifyObserversMessageRecieved(string s);
        void NotifyObserversUsersListRecieved(string usersList);
        void NotifyObserversErrorOcured(string s);
    }
}
