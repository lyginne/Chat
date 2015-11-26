﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatModel {
    public class MessageAnalyserHelper {
        public static ServerUser GetServerUserFromBytes(byte[] buffer,int length) {
            int hasedPasswordSize = EncodyngAndCryptoInformation.hashingAlgorytm.HashSize/8;
            string username=EncodyngAndCryptoInformation.encoding.GetString(buffer, hasedPasswordSize, length - (hasedPasswordSize+1));
            byte[] hashedPassword = new byte[hasedPasswordSize];
            Array.Copy(buffer,1,hashedPassword,0,hasedPasswordSize);
            //buffer.CopyTo(hashedPassword,1);

            return new ServerUser(username, hashedPassword);
        }
        public static string GetMessagesFromBytes(byte[] buffer, int length) {
            return EncodyngAndCryptoInformation.encoding.GetString(buffer, 1, length - 1 );
        }
    }
}
