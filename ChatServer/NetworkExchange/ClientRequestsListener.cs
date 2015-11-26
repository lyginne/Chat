using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using  ChatModel;
using ChatServer.NetworkExchange;

namespace ChatServer {
    class ClientRequestsListener {
        private Socket socket;
        private NetworkStream networkStream;
        private StreamWriter writer;
        private StreamReader reader;
        string userExist="1";
        string IncorrectLoginOrPassword= "2";

        public ClientRequestsListener(Socket socket) {
            networkStream = new NetworkStream(socket);
            reader = new StreamReader(networkStream,Encoding.UTF8);
            writer = new StreamWriter(networkStream, Encoding.UTF8);
            string requestString = reader.ReadLine();
            ChatModel.Rrules request;
            if (requestString[0] == '0') {
                OnAutorizeRequest(requestString);
            }
            else if (requestString[0] == '1') {
                OnRegisterRequest(requestString);
            }
            else {
                Console.WriteLine("Неавторизованный запрос");
                Disconnect(socket);
            }
        }

        private void OnAutorizeRequest(string message) {

            ServerUser serverUser = MessageAnalyserHelper.GetServerUserFromString(message);
            if (serverUser.Username.Length < 3) {
                SendServerResponse("1");
#warning disconnect
                //Disconnect(socket);
                return;
            }
            if (DataBase.DataBaseManager.GetInstance().VerifyUser(serverUser)) {
                Authorize(new ChatClient(serverUser, socket));
            }
            else {
                SendServerResponse("1");
#warning disconnect
                //Disconnect(socket);
            }

        }

        private void OnRegisterRequest(string message) {
            ServerUser serverUser = MessageAnalyserHelper.GetServerUserFromString(message);
            if (serverUser.Username.Length < 3) {
                SendServerResponse("1");
#warning disconnect
                //Disconnect(socket);
                return;
            }
            if (!DataBase.DataBaseManager.GetInstance().CheckUserExistance(serverUser)) {
                DataBase.DataBaseManager.GetInstance().AddUserToDataBase(serverUser);
                Authorize(new ChatClient(serverUser, socket));
            }
            else {
                SendServerResponse("2");
#warning disconnect
                //Disconnect(socket);
            }
        }

        private void Authorize(ChatClient chatClient) {
            SendServerResponse("0");
            SendHundreedOfMessages(chatClient);
            //Broadcaster.GetInstance().AddOnlineUser(chatClient);
            waitForMessages(chatClient);
        }

        private void waitForMessages(ChatClient chatClient) {
            Socket socket = chatClient.Socket;
            byte[] socketbuffer = new byte[1024];
//            while (true) {
//            string requestString = reader.ReadLine();
//
//                //Broadcaster.GetInstance().BroadcastMessageFrom(socket, MessageAnalyserHelper.GetMessagesFromBytes(socketbuffer,bytesRecieved));
//            }

        }

        private void SendServerResponse(string response) {
            writer.WriteLine(response);
            writer.Flush();
        }

        private void SendHundreedOfMessages(ChatClient chatClient) {
            //Broadcaster.GetInstance().SendHeundreedMessagesToUser(chatClient);
        }

        private void Disconnect(Socket socket) {
            if (socket == null) {
                return;
            }
            if (socket.Connected) {
                socket.Shutdown(SocketShutdown.Both);
            }
            socket.Close();
        }
    }
}
