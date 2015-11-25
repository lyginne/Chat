using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatModel {
    public class MessageAnalyserHelper {
        public static ServerUser GetServerUserFromBytes(byte[] buffer,int length) {
            int hasedPasswordSize = EncodyngAndCryptoInformation.hashingAlgorytm.HashSize;
            string username=EncodyngAndCryptoInformation.encoding.GetString(buffer, hasedPasswordSize+1, length - hasedPasswordSize+1);
            if (username.Length < 3) {
                throw new Exception("Моловато будет");
            }
            byte[] hashedPassword = new byte[hasedPasswordSize];
            buffer.CopyTo(hashedPassword,1);

            return new ServerUser(username, hashedPassword);
        }
    }
}
