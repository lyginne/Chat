using System;
using System.Data.SQLite;
using System.IO;
using ChatModel;
using ChatModel.DataModel;

namespace ChatServer.DataBase {
    //Синглтон для общения с базой. По-хорошему ему надо свой поток, очередь на запись и расписание, но ладно
    class DataBaseManager {
        private static DataBaseManager _dataBaseManager;
        private const string DataBasePath = "my.bd";
        private SQLiteConnection dataBaseConnection;
        
        public static DataBaseManager GetInstance() {
            return _dataBaseManager;
        }

        public void AddUserToDataBase(User user) {
            lock (dataBaseConnection) {
                dataBaseConnection.Open();
                string sql = $"INSERT INTO USERS ( USERNAME, PASSWORDHASHBASE64 ) " +
                                $"VALUES ( '{user.Username}' , '{user.HashedPasswordBase64}' ) ;";
                SQLiteCommand command = new SQLiteCommand(sql, dataBaseConnection);
                command.ExecuteNonQuery();
                dataBaseConnection.Close();
            }


        }

        public bool VerifyUser(User user) {
            int userMatch;
            lock (dataBaseConnection) {
                dataBaseConnection.Open();
                string sql = $"SELECT COUNT(*) " +
                             $"FROM USERS " +
                             $"WHERE USERNAME = '{user.Username}' " +
                             $"AND PASSWORDHASHBASE64 = '{user.HashedPasswordBase64}' ;";
                SQLiteCommand command = new SQLiteCommand(sql, dataBaseConnection);
                userMatch = Convert.ToInt32(command.ExecuteScalar());
                dataBaseConnection.Close();
            }
            if (userMatch == 1) {
                return true;
            }
            if (userMatch > 1) {
                //Строго говоря, больше одного совпадения - кривая база, но ладно
                return false;
            }
            return false;
        }

        public bool CheckUserExistance(User user) {
            int userMatch;
            lock (dataBaseConnection) {
                dataBaseConnection.Open();
                string sql = $"SELECT COUNT(*) " +
                             $"FROM USERS " +
                             $"WHERE USERNAME = '{user.Username}' ;";
                SQLiteCommand command = new SQLiteCommand(sql, dataBaseConnection);
                userMatch = Convert.ToInt32(command.ExecuteScalar());
                dataBaseConnection.Close();
            }
            if (userMatch > 0) {
                //Строго говоря, больше одного совпадения - кривая база, но ладно
                return true;
            }
            return false;
        }

        #region constructors, initialisers

        public static void InitializeDatabaseManager() {
            _dataBaseManager = new DataBaseManager();
        }

        private DataBaseManager() {
            CreateDataBaseFileIfNotExist(DataBasePath);
            dataBaseConnection = new SQLiteConnection(String.Format("Data Source={0}", DataBasePath));
            CreateUsesrsTableIfNotExist();
        }

        private void CreateDataBaseFileIfNotExist(string dataBasePath) {
            if (!File.Exists(dataBasePath)) {
                SQLiteConnection.CreateFile(dataBasePath);
            }
        }

        private void CreateUsesrsTableIfNotExist() {
            dataBaseConnection.Open();
            SQLiteCommand command = new SQLiteCommand("CREATE TABLE IF NOT EXISTS USERS( USERNAME VARCHAR(10),PASSWORDHASHBASE64 VARCHAR(44) ) ;", dataBaseConnection);
            command.ExecuteNonQuery();
            dataBaseConnection.Close();
        }

        #endregion
    }
}
