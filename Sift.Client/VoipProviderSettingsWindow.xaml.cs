using System;
using System.Windows;
using System.Windows.Controls;

using Sift.Client.Elements;
using Sift.Common;
using Sift.Common.Network;

namespace Sift.Client
{
    /// <summary>
    /// Interaction logic for VoipProviderSettingsWindow.xaml
    /// </summary>
    public partial class VoipProviderSettingsWindow : Window
    {
        private SdpClient client;
        private VoipProviders provider;

        public VoipProviderSettingsWindow(SdpClient client, VoipProviders provider)
        {
            this.client = client;
            this.provider = provider;

            client.SettingsChanged += Client_SettingsChanged;

            InitializeComponent();

            FetchSettings(SettingsCollection.Category[provider]);
        }

        private void Client_SettingsChanged(object sender, SettingsChanged e)
        {
            if (e.Category != SettingsCollection.Category[provider])
                return;

            openElement(e.Settings);

            client.SettingsChanged -= Client_SettingsChanged;
        }

        private void FetchSettings(string category)
        {
            client?.Send(new SettingsChanged(category));
        }

        private void openElement(SettingsCollection settings)
        {
            UserControl element;
            switch (provider)
            {
                case VoipProviders.Asterisk:
                    element = new AsteriskSettingsElement(settings);
                    break;
                default:
                    throw new Exception("Unknown VOIP provider");
            }
            Element.Children.Add(element);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Save
            Close();
        }
    }
}
