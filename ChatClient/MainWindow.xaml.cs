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
            InitializeComponent();
        }


        #region Events

        private void OnSend(object sender, RoutedEventArgs e) {
            Connector.Instance.Send(TbCurrentMessage.Text);
            TbCurrentMessage.Text = "";
        }

        private void OnAuthorize(object sender, RoutedEventArgs e) {
            try {
                Connector.InitializeConnector();
            }
            catch (Exception exception) {
                MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (Connector.Instance != null) {
                    Connector.Instance.Dispose();
                }
                return;
            }
            Connector.Instance.AddObserver(this);
            Connector.Instance.Authorize(TbLogin.Text,PbPassword.Password);
        }
        private void OnRegister(object sender, RoutedEventArgs e) {
            try {
                Connector.InitializeConnector();
            }
            catch (Exception exception) {
                MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (Connector.Instance != null) {
                    Connector.Instance.Dispose();
                }
                return;
            }
            Connector.Instance.AddObserver(this);
            //Если пароль хешровать, то узнать его длину уже не получится. Посылать в открытом виде и проверять на сервере?
            if (PbPassword.Password.Length < 3) {
                MessageBox.Show("Пароль должен быть не менее 3-х символов", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Connector.Instance.Register(TbLogin.Text, PbPassword.Password);
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e) {
            if (Connector.Instance != null) {
                Connector.Instance.Dispose();
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
            if (Connector.Instance != null) {
                Connector.Instance.Dispose();
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
            int index = TbUsersOnline.Text.IndexOf(username);
            if (index < 0) {
                return;
            }
            else {
                TbUsersOnline.Text = TbUsersOnline.Text.Remove(index, username.Length);
            }
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
