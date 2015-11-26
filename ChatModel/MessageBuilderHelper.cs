using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatModel {
    public  class MessageBuilderHelper {
        public static byte[] GetBytesToRegistrationRequest(string username, string password) {
            byte[] hashedPassword = EncodyngAndCryptoInformation.hashingAlgorytm.ComputeHash(Encoding.UTF8.GetBytes(password));
            byte[] inputbuffer = new byte[1];
            byte[] sendBuffer = new byte[hashedPassword.Length + username.Length + 1];
            sendBuffer[0] = (byte)ChatModel.Rrules.Registration;
            hashedPassword.CopyTo(sendBuffer, 1);
            Encoding.UTF8.GetBytes(username).CopyTo(sendBuffer, 32);
            return sendBuffer;
        }
        public static byte[] GetBytesToAutorisationRequest(string username, string password) {
            byte[] hashedPassword = EncodyngAndCryptoInformation.hashingAlgorytm.ComputeHash(Encoding.UTF8.GetBytes(password));
            byte[] inputbuffer = new byte[1];
            byte[] sendBuffer = new byte[hashedPassword.Length + username.Length + 1];
            sendBuffer[0] = (byte)ChatModel.Rrules.Autorization;
            hashedPassword.CopyTo(sendBuffer, 1);
            Encoding.UTF8.GetBytes(username).CopyTo(sendBuffer, 32);
            return sendBuffer;
        }
        public static byte[] GetBytesToMessageSendRequest(string message) {
            byte[] sendBuffer = new byte[message.Length+1];
            sendBuffer[0] = (byte) ChatModel.Rrules.Message;
            EncodyngAndCryptoInformation.encoding.GetBytes(message).CopyTo(sendBuffer,1);
            return sendBuffer;
        }
    }
}
