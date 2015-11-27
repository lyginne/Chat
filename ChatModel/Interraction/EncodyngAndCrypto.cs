using System.Text;

namespace ChatModel.Interraction {
    public static class EncodyngAndCrypto {
        public static Encoding Encoding = Encoding.UTF8;
        public static System.Security.Cryptography.SHA256Managed HashingAlgorytm = new System.Security.Cryptography.SHA256Managed();
    }
}
