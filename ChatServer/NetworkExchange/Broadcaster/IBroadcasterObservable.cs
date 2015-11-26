namespace ChatServer.NetworkExchange.Broadcaster {
    interface IBroadcasterObservable {
        void AddBroadcasterClient(IBroadcasterClient client);
        void RemoveBroadcasterClient(IBroadcasterClient client);
        void NotifyClientsNewUserCame(string user);
        void NotifyClientsMessageRecieved(string message);
        void NotifyClientsUserQuit(string message);
    }
}
