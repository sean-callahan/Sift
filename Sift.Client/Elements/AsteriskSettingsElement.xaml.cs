using System.Collections.Generic;
using System.Windows.Controls;

using Sift.Common;

namespace Sift.Client.Elements
{
    /// <summary>
    /// Interaction logic for AsteriskSettingsElement.xaml
    /// </summary>
    public partial class AsteriskSettingsElement : UserControl
    {
        private SettingsCollection settings;

        public AsteriskSettingsElement(SettingsCollection settings)
        {
            this.settings = settings;

            InitializeComponent();
        }
    }
}
