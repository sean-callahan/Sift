using System;
using System.Windows;
using System.Windows.Controls;

using Sift.Client.Elements;
using Sift.Common;

namespace Sift.Client
{
    /// <summary>
    /// Interaction logic for VoipProviderSettingsWindow.xaml
    /// </summary>
    public partial class VoipProviderSettingsWindow : Window
    {
        private VoipProviders provider;

        public VoipProviderSettingsWindow(VoipProviders provider)
        {
            this.provider = provider;

            InitializeComponent();

            UserControl element;
            switch (provider)
            {
                case VoipProviders.Asterisk:
                    element = new AsteriskSettingsElement();
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
