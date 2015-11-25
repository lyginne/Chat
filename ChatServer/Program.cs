﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ChatClient;
using ChatServer.DataBase;

namespace ChatServer {
    class Program {
        static void Main(string[] args) {
            try {
                XMLSettings xmlSettings = new XMLSettings("settings.xml");
                DataBaseManager.InitializeDatabaseManager();
                Server server = new Server(xmlSettings.IpAddress, xmlSettings.Port);
            }
            catch (Exception e) {
                Console.WriteLine("Невозможно запустить сервер" + e.Message);
                Console.ReadLine();
            }
        }
    }
}
