using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer {
    //Синглтон для общения с базой
    class DataBaseManager {
        private static DataBaseManager _dataBaseManager;
        private const string DataBasePath = "my.bd";
        private SQLiteConnection dataBaseConnection;

        private DataBaseManager() {
            CreateDataBaseFileIfNotExist(DataBasePath);
            dataBaseConnection = new SQLiteConnection(String.Format("Data Source={0}", DataBasePath));
            CreateUsesrsTableIfNotExist();
        }

        private void CreateUsesrsTableIfNotExist() {
            dataBaseConnection.Open();
            SQLiteCommand command = new SQLiteCommand("CREATE TABLE IF NOT EXISTS USERS( USERNAME VARCHAR(10),PASSWORDHASHBASE64 VARCHAR(44) );", dataBaseConnection);
            command.ExecuteNonQuery();
            dataBaseConnection.Close();
        }


        public static void InitializeDatabaseManager() {
            _dataBaseManager=new DataBaseManager();
        }

        public static DataBaseManager GetInstance() {
            return _dataBaseManager;
        }

        public void AddUserToDataBase(ServerUser user) {
            
        }

        public bool VerifyUser(ServerUser user) {
            
            return true;
        }

        public bool CheckUserExostance(ServerUser user) {
            return false;
        }

        private void CreateDataBaseFileIfNotExist(string dataBasePath) {
            if (!File.Exists(dataBasePath)) {
                SQLiteConnection.CreateFile(dataBasePath);
            }
        }
    }
}
