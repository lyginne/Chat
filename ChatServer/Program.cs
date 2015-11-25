using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatClient;

namespace ChatServer {
    class Program {
        static void Main(string[] args) {
            XMLSettings xmlSettings = new XMLSettings("settings.xml");
            Server server = new Server(xmlSettings.IpAddress,xmlSettings.Port);

        }
    }
}
