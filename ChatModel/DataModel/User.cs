namespace ChatModel.DataModel {
    public class User {
        public string Username;
        public string HashedPasswordBase64;

        public User(string username, string hashedPasswordBase64) {
            HashedPasswordBase64 = hashedPasswordBase64;
            Username = username;
        }
    }
}
