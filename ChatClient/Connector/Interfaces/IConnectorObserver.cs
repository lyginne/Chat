namespace ChatClient.Connector.Interfaces {
    interface IConnectorObserver {
        void OnUserPasswordOperationSuceed();
        void OnMessageRecieved(string messsage);
        void OnErrorOcurs(string description);
        void OnUserJoined(string username);
        void OnUserQuit(string username);
    }
}
