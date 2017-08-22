using System;
using System.Windows;

using Sift.Common.Network;

namespace Sift.Client
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private SdpClient Client { get; set; } 

        public LoginWindow(SdpClient client)
        {
            Client = client;
            Client.UpdateAppState += Client_UpdateAppState;
            Client.Error += Client_Error;

            InitializeComponent();
        }

        private void Client_Error(object sender, Exception e) => App.ShowError(e);

        private void Client_UpdateAppState(object sender, UpdateAppState e)
        {
            if (Client == null)
                return;
            if (e.LineCount < 1 || e.LineCount > 20)
                throw new Exception("Received an invalid line count");
            new MainWindow(Client, e.LineCount).Show();
            Close();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Connected: " + Client.Connected + "\nPort:" + Client.Client.Port);
            Client.Send(new LoginRequest());
        }
    }
}
