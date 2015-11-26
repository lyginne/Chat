using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Chat {
    interface IConnectorObserver {
        void OnUserPasswordOperationSuceed();
        void OnMessageRecieved(string messsage);
        void OnErrorOcurs(string description);
        void OnUsersListRecieved(string usersList);
    }
}
