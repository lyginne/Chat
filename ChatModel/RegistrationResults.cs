using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatModel
{
    enum RegistrationResults{
        UserAlreadyExists,
        IncorrectLoginOrPassword,
        Ok
    }
    public enum Rrules : byte {
        Message,
        UsersList,
        Login,
        Password,
        Registration,
        Autorization,
        Unauthorized,
        UserExists,
        IncorrectLoginOrPassword,
        ErrorOcured,
        Ok
    }
}
