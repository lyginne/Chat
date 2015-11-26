namespace ChatModel {
    public class ServerUser : User {
        public string HashedPasswordBase64;

        public ServerUser(string username, string hashedPasswordBase64) {
            HashedPasswordBase64 = hashedPasswordBase64;
            Username = username;
        }
    }
}
