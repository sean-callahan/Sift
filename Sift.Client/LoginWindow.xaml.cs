using System;
using System.Windows;

using Sift.Client.Properties;
using Sift.Common.Net;
using Sift.Common.Network;

namespace Sift.Client
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private SdpClient Client { get; set; }

        private bool connectionSent;

        private readonly Role[] possibleRoles = new Role[]
        {
            Role.Host,
            Role.Screener
        };

        public LoginWindow(SdpClient client)
        {
            Client = client;
            Client.Manager.InitializeClient += InitializeClient;
            Client.Manager.Error += Client_Error;

            InitializeComponent();

            Title = "Sift " + App.Version + " - Login";

            LoginAs.ItemsSource = possibleRoles;
            LoginAs.SelectedIndex = 0;
        }

        private void Client_Disconnected(object sender, string reason)
        {
            connectionSent = false;

            App.DisplayError("Could not connect to server.", reason);
        }

        private void Client_ConnectionSuccess(object sender, EventArgs e)
        {
            //Client.Send(new LoginRequest());
        }

        private void Client_Error(string id, Error e) => App.DisplayError(e.Message, e.Detail);

        private void InitializeClient(string id, InitializeClient e)
        {
            if (Client == null)
                return;
            if (e.Lines < 1 || e.Lines > 20)
                throw new Exception("Received an invalid line count");
            new MainWindow(Client, e.Provider, e.Lines, Role.Screener).Show();
            Close();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (connectionSent)
                return;

            NetworkUser u = new NetworkUser()
            {
                Username = UsernameBox.Text,
                Password = PasswordBox.Password,
            };

            Client.Connect(Properties.Settings.Default.Address, Properties.Settings.Default.Port);
            connectionSent = true;
        }

        private async void SettingsButton_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            NetworkSettingsWindow settings = new NetworkSettingsWindow();
            settings.Show();
            await settings.Wait();
            settings.Hide();
        }
    }
}
