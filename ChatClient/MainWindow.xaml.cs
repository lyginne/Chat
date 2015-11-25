using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Chat.Annotations;
using ChatClient;
using ChatClient.Connector;

namespace Chat
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IConnectorObserver{

        delegate void StringMethodInboker(string arg);

        delegate void NoArgumentsMethodInvoker();

        public MainWindow() {
            try {
                Connector.InitializeConnector();
            }
            catch (Exception e) {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
                return;
            }
            Connector.Instance.AddObserver(this); 
            InitializeComponent();
        }


        #region Events

        private void OnSend(object sender, RoutedEventArgs e) {
            Connector.Instance.Send(TbCurrentMessage.Text);
            TbCurrentMessage.Text = "";
        }
        private void OnAuthorize(object sender, RoutedEventArgs e) {
            Connector.Instance.Authorize(TbLogin.Text,PbPassword.Password);
        }
        private void OnRegister(object sender, RoutedEventArgs e) {
            //Если пароль хешровать, то узнать его длину уже не получится. Посылать в открытом виде и проверять на сервере?
            if (PbPassword.Password.Length < 3) {
                MessageBox.Show("Пароль должен быть не менее 3-х символов", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Connector.Instance.Register(TbLogin.Text, PbPassword.Password);
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e) {
           Connector.Instance.Disconnect();
        }

        #endregion

        #region IConnectorObserver

        public void OnAutorizationSucceed() {
            if (!Dispatcher.CheckAccess()) {
                Dispatcher.Invoke(new NoArgumentsMethodInvoker(OnAutorizationSucceed));
                return;
            }
            SetAuthorizedState();
        }

        public void OnRegistrationSucced() {
            OnAutorizationSucceed();
        }

        public void OnMessageRecieved(string messsage) {
            if (!Dispatcher.CheckAccess()) {
                Dispatcher.Invoke(new StringMethodInboker(OnMessageRecieved), messsage);
                return;
            }
            TbChatMessages.AppendText(messsage+"\r\n");
            TbChatMessages.ScrollToEnd();
        }

        public void OnErrorOcurs(string description) {
            if (!Dispatcher.CheckAccess()) {
                Dispatcher.Invoke(new StringMethodInboker(OnErrorOcurs), description);
                return;
            }
            MessageBox.Show(description, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            SetUnauthorizedState();

        }

        public void OnUsersListRecieved(string usersList) {
            if (!Dispatcher.CheckAccess()) {
                Dispatcher.Invoke(new StringMethodInboker(OnUsersListRecieved), usersList);
                return;
            }
            TbUsersOnline.Text = "";
            TbUsersOnline.AppendText(usersList);
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
