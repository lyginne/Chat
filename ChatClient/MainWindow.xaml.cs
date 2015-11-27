using System;
using System.ComponentModel;
using System.Windows;
using ChatClient.Connector.Interfaces;

namespace ChatClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IConnectorObserver{

        delegate void StringMethodInboker(string arg);

        delegate void NoArgumentsMethodInvoker();

        public MainWindow() {
            InitializeComponent();
        }


        #region Events

        private void OnSend(object sender, RoutedEventArgs e) {
            Connector.Connector.GetInstance().Send(TbCurrentMessage.Text);
            TbCurrentMessage.Text = "";
        }

        private void OnAuthorize(object sender, RoutedEventArgs e) {
            try {
                Connector.Connector.InitializeConnector();
            }
            catch (Exception exception) {
                MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (Connector.Connector.GetInstance() != null) {
                    Connector.Connector.GetInstance().Dispose();
                }
                return;
            }
            Connector.Connector.GetInstance().AddObserver(this);
            Connector.Connector.GetInstance().Authorize(TbLogin.Text,PbPassword.Password);
        }
        private void OnRegister(object sender, RoutedEventArgs e) {
            try {
                Connector.Connector.InitializeConnector();
            }
            catch (Exception exception) {
                MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (Connector.Connector.GetInstance() != null) {
                    Connector.Connector.GetInstance().Dispose();
                }
                return;
            }
            Connector.Connector.GetInstance().AddObserver(this);
            //Если пароль хешровать, то узнать его длину уже не получится. Посылать в открытом виде и проверять на сервере?
            if (PbPassword.Password.Length < 3) {
                MessageBox.Show("Пароль должен быть не менее 3-х символов", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Connector.Connector.GetInstance().Register(TbLogin.Text, PbPassword.Password);
        }

        private void OnWindowClosing(object sender, CancelEventArgs e) {
            if (Connector.Connector.GetInstance() != null) {
                Connector.Connector.GetInstance().Dispose();
            }
        }

        #endregion

        #region IConnectorObserver

        public void OnUserPasswordOperationSuceed() {
            if (!Dispatcher.CheckAccess()) {
                Dispatcher.Invoke(new NoArgumentsMethodInvoker(OnUserPasswordOperationSuceed));
                return;
            }
            SetAuthorizedState();
        }

        public void OnMessageRecieved(string messsage) {
            if (!Dispatcher.CheckAccess()) {
                Dispatcher.Invoke(new StringMethodInboker(OnMessageRecieved), messsage);
                return;
            }
            TbChatMessages.AppendText(messsage);
            TbChatMessages.ScrollToEnd();
        }

        public void OnErrorOcurs(string description) {
            if (!Dispatcher.CheckAccess()) {
                Dispatcher.Invoke(new StringMethodInboker(OnErrorOcurs), description);
                return;
            }
            MessageBox.Show(description, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            SetUnauthorizedState();
            if (Connector.Connector.GetInstance() != null) {
                Connector.Connector.GetInstance().Dispose();
            }


        }

        public void OnUserJoined(string username) {
            if (!Dispatcher.CheckAccess()) {
                Dispatcher.Invoke(new StringMethodInboker(OnUserJoined), username);
                return;
            }
            TbUsersOnline.AppendText($"{username}\r\n");
        }

        public void OnUserQuit(string username) {
            if (!Dispatcher.CheckAccess()) {
                Dispatcher.Invoke(new StringMethodInboker(OnUserQuit), username);
                return;
            }

            username += "\r\n";
            int index = TbUsersOnline.Text.IndexOf(username, StringComparison.Ordinal);
            if (index < 0) {
                OnErrorOcurs("Сервер просит удалить юзера, которого и так нет в списке");
                return;
            }
            TbUsersOnline.Text = TbUsersOnline.Text.Remove(index, username.Length);
        }

        private void SetUnauthorizedState() {
            TbLogin.IsEnabled = true;
            PbPassword.IsEnabled = true;
            ButtonAuthorization.IsEnabled = true;
            ButtonRegistration.IsEnabled = true;
            TbCurrentMessage.IsEnabled = false;
            ButtonSend.IsEnabled = false;
            TbChatMessages.Text = "";
            TbUsersOnline.Text = "";
        }

        private void SetAuthorizedState() {
            TbLogin.IsEnabled = false;
            PbPassword.IsEnabled = false;
            ButtonAuthorization.IsEnabled = false;
            ButtonRegistration.IsEnabled = false;
            TbCurrentMessage.IsEnabled = true;
            ButtonSend.IsEnabled = true;
        }
    #endregion
    }
}
