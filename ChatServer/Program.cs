using System;
using ChatModel;
using ChatServer.DataBase;
using ChatServer.NetworkExchange.Broadcaster;

namespace ChatServer {
    class Program {
        static void Main(string[] args) {
            try {
                XMLSettings xmlSettings = new XMLSettings("settings.xml");
                DataBaseManager.InitializeDatabaseManager();
                Broadcaster.Initialize();
                new Server(xmlSettings.IpAddress, xmlSettings.Port);

            }
            catch (Exception e) {
                Console.WriteLine("Невозможно запустить сервер: " + e.Message);
                Console.ReadLine();
            }
        }
    }
}
