using System.Threading.Tasks;
using System.Windows;

namespace Sift.Client
{
    /// <summary>
    /// Interaction logic for NetworkSettingsWindow.xaml
    /// </summary>
    public partial class NetworkSettingsWindow : Window
    {
        private bool Saved = false;

        public NetworkSettingsWindow()
        {
            InitializeComponent();

            Address.Text = Properties.Settings.Default.Address;
            Port.Text = Properties.Settings.Default.Port.ToString();
        }

        public async Task Wait()
        {
            await Task.Run(() =>
            {
                while (!Saved) ;
            });
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Address = Address.Text;
            Properties.Settings.Default.Port = int.Parse(Port.Text);
            Properties.Settings.Default.Save();
            Saved = true;
        }
    }
}
