﻿using System;
using System.Windows;

using Sift.Client.Properties;
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
            Client.Disconnected += Client_Disconnected;
            Client.ConnectionSuccess += Client_ConnectionSuccess;
            Client.UpdateAppState += Client_UpdateAppState;
            Client.Error += Client_Error;

            InitializeComponent();

            LoginAs.ItemsSource = possibleRoles;
            LoginAs.SelectedIndex = 0;
        }

        private void Client_Disconnected(object sender, string reason)
        {
            connectionSent = false;

            App.ShowError("Could not connect to server.", reason);
        }

        private void Client_ConnectionSuccess(object sender, EventArgs e)
        {
            Client.Send(new LoginRequest());
        }

        private void Client_Error(object sender, ErrorPacket e) => App.ShowError(e.Message, e.StackTrace);

        private void Client_UpdateAppState(object sender, UpdateAppState e)
        {
            if (Client == null)
                return;
            if (e.LineCount < 1 || e.LineCount > 20)
                throw new Exception("Received an invalid line count");
            new MainWindow(Client, e.ProviderType, e.LineCount, (Role)LoginAs.SelectedItem).Show();
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

            Client.Connect(Settings.Default.Address, Settings.Default.Port, u);
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
