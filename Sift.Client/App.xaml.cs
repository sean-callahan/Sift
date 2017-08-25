using System;
using System.Windows;
using System.Windows.Interop;

using Sift.Client.Properties;
using Sift.Common.Network;

namespace Sift.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private SdpClient Client { get; }

        public App()
        {
            Client = new SdpClient();
            

            ComponentDispatcher.ThreadIdle += new EventHandler(Client.ReadMessages);
        }

        private async void App_Startup(object sender, StartupEventArgs e)
        {
            string address = Settings.Default.Address;
            if (string.IsNullOrWhiteSpace(address))
            {
                NetworkSettingsWindow settings = new NetworkSettingsWindow();
                settings.Show();
                await settings.Wait();
                settings.Hide();
            }
            address = Settings.Default.Address;
            int port = Settings.Default.Port;

            Client.Connect(address, port);

            new LoginWindow(Client).Show();
        }

        public static void ShowError(string msg, string stackTrace)
        {
            new ErrorWindow(msg, stackTrace).Show();
        }
    }
}
