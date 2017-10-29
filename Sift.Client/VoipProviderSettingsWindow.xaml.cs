using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using Sift.Client.Elements;
using Sift.Common;
using Sift.Common.Net;

namespace Sift.Client
{
    /// <summary>
    /// Interaction logic for VoipProviderSettingsWindow.xaml
    /// </summary>
    public partial class VoipProviderSettingsWindow : Window
    {
        private static readonly IDictionary<VoipProviders, string> providerNames = new Dictionary<VoipProviders, string>()
        {
            { VoipProviders.Asterisk, "asterisk" },
        };

        private SdpClient client;
        private VoipProviders provider;
        //private IDictionary<string, NetworkSetting> cache;
        //private ISettingsElement element;

        public VoipProviderSettingsWindow(SdpClient client, VoipProviders provider)
        {
            this.client = client;
            this.provider = provider;
            
            //client.UpdateSettings += Client_UpdateSettings;

            InitializeComponent();

            //client.Send(new RequestSettings() { Category = providerNames[provider] });
        }

        /*private void Client_UpdateSettings(object sender, UpdateSettings e)
        {
            if (cache != null || e.Category != providerNames[provider])
                return;

            cache = new Dictionary<string, NetworkSetting>();
            foreach (NetworkSetting item in e.Items)
                cache.Add(item.Key, item);

            //client.UpdateSettings -= Client_UpdateSettings;

            openElement(cache);
        }

        private void openElement(IDictionary<string, NetworkSetting> cache)
        {
            switch (provider)
            {
                case VoipProviders.Asterisk:
                    element = new AsteriskSettingsElement(client);
                    break;
                default:
                    throw new Exception("Unknown VOIP provider");
            }
            element.Load(cache);
            Element.Children.Add((UserControl)element);
        }
        */
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            //element.Save();
            Close();
        }
    }
}
