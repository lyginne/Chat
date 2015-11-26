﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using  ChatModel;
using ChatServer.NetworkExchange;

namespace ChatServer {
    class ClientRequestsListener {

    public ClientRequestsListener(Socket socket) {
            byte[] socketbuffer = new byte[1024];
            int bytesRecieved = socket.Receive(socketbuffer, 0,socketbuffer.Length, SocketFlags.None);
            if (socketbuffer[0] == (byte)ChatModel.Rrules.Autorization) {
                OnAutorizeRequest(socketbuffer,bytesRecieved,socket);
            }
            else if (socketbuffer[0] == (byte) ChatModel.Rrules.Registration) {
                OnRegisterRequest(socketbuffer, bytesRecieved,socket);
            }
            else {
                Console.WriteLine("Неавторизованный запрос");
                Disconnect(socket);
            }

        }

        private void OnAutorizeRequest(byte[] socketbuffer, int bytesRecieved, Socket socket) {
            ServerUser serverUser = MessageAnalyserHelper.GetServerUserFromBytes(socketbuffer, bytesRecieved);
            if (DataBase.DataBaseManager.GetInstance().VerifyUser(serverUser)) {
                Authorize(new ChatClient(serverUser, socket));
            }
            else {
                sendServerResponse(socket, ChatModel.Rrules.IncorrectLoginOrPassword);
                Disconnect(socket);
            }

        }

        private void OnRegisterRequest(byte[] socketbuffer, int bytesRecieved, Socket socket) {
            ServerUser serverUser = MessageAnalyserHelper.GetServerUserFromBytes(socketbuffer, bytesRecieved);
            if (!DataBase.DataBaseManager.GetInstance().CheckUserExistance(serverUser)) {
                Authorize(new ChatClient(serverUser, socket));
            }
            else {
                sendServerResponse(socket, ChatModel.Rrules.IncorrectLoginOrPassword);
                Disconnect(socket);
            }
        }

        private void Authorize(ChatClient chatClient) {
            sendServerResponse(chatClient.Socket, ChatModel.Rrules.Ok);
            SendHundreedOfMessages(chatClient);
            Broadcaster.GetInstance().AddOnlineUser(chatClient);
            waitForMessages(chatClient);
        }

        private void waitForMessages(ChatClient chatClient) {
            Socket socket = chatClient.Socket;
            byte[] socketbuffer = new byte[1024];
            while (true) {
                int bytesRecieved = socket.Receive(socketbuffer, 0, socketbuffer.Length, SocketFlags.None);
                if (bytesRecieved == 0) {
                    Console.WriteLine("Клиент ушёл и зашатдаунил порт");
                    return;
                }
                Broadcaster.GetInstance().BroadcastMessageFrom(socket, MessageAnalyserHelper.GetMessagesFromBytes(socketbuffer,bytesRecieved));
            }

        }

        private void sendServerResponse(Socket socket, ChatModel.Rrules response) {
            byte[] outputbuffer = new byte[1];
            outputbuffer[0] = (byte)ChatModel.Rrules.Ok;
            socket.Send(outputbuffer, 1, SocketFlags.None);
        }

        private void SendHundreedOfMessages(ChatClient chatClient) {


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
