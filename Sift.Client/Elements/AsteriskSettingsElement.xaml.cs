using System;
using System.Collections.Generic;
using System.Windows.Controls;

using Sift.Common.Net;

namespace Sift.Client.Elements
{
    /// <summary>
    /// Interaction logic for AsteriskSettingsElement.xaml
    /// </summary>
    public partial class AsteriskSettingsElement : UserControl, ISettingsElement
    {
        private SdpClient client;
        private IDictionary<string, NetworkSetting> cache;

        public AsteriskSettingsElement(SdpClient client)
        {
            this.client = client;

            InitializeComponent();
        }

        private void HybridAdd_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string text = HybridAddBox.Text;
            if (string.IsNullOrWhiteSpace(text))
                return;
            text = text.Trim();
            if (HybridsBox.Items.Contains(text))
                return;
            int exten;
            if (!int.TryParse(text, out exten))
            {
                App.DisplayError("Could not parse extension.", "Extension must be a number.");
                return;
            }
            HybridsBox.Items.Add(exten);
        }

        private void HybridRemove_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (HybridsBox.SelectedIndex < 0)
                return;

            HybridsBox.Items.RemoveAt(HybridsBox.SelectedIndex);
        }

        public void Load(IDictionary<string, NetworkSetting> cache)
        {
            this.cache = cache;

            ScreenerExtensionBox.Text = ((int)cache["asterisk_screener_extension"].Value).ToString();
            int[] hybrids = (int[])cache["asterisk_hybrid_extensions"].Value;
            Array.Sort(hybrids);
            foreach (int hybrid in hybrids)
                HybridsBox.Items.Add(hybrid);
        }

        public void Save()
        {
            int[] hybrids = new int[HybridsBox.Items.Count];
            for (int i = 0; i < hybrids.Length; i++)
                hybrids[i] = (int)HybridsBox.Items[i];

            cache["asterisk_hybrid_extensions"].Value = hybrids;
            cache["asterisk_screener_extension"].Value = int.Parse(ScreenerExtensionBox.Text);

            //client.Send(new UpdateSettings(cache.Values));
        }
    }
}
