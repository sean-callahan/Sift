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
            

            ComponentDispatcher.ThreadIdle += (_, __) => Client.TryReadMessage();
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            new LoginWindow(Client).Show();
        }

        public static void ShowError(string msg, string stackTrace)
        {
            new ErrorWindow(msg, stackTrace).Show();
        }
    }
}
