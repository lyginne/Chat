namespace ChatModel.Interraction.Message.Headers {
    public class ServerMesageHeader {
        public const string Ok = "0|";
        public const string UsernameOrPasswordIncorrect = "1|";
        public const string UserExist = "2|";
        public const string Message = "3|";
        public const string NewUser = "4|";
        public const string UserQuit = "5|";
    }
}
