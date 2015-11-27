using ChatModel.DataModel;

namespace ChatModel.Interraction.Message {
    public static class MessageAnalyserHelper {
        public static User GetServerUserFromString(string serverUserString) {
            string hashedPasswordBase64 = serverUserString.Substring(2, 44);
            string username = serverUserString.Substring(46, serverUserString.Length - 46);
            return new User(username,hashedPasswordBase64);
            
        }
    }
}
