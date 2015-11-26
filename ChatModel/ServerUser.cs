namespace ChatModel {
    public class ServerUser : User {
        public byte[] HashedPassword;

        public ServerUser(string username, byte[] hashedPassword) {
            HashedPassword = hashedPassword;
            Username = username;
        }
    }
}
