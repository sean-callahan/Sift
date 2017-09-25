using System.Windows.Controls;

namespace Sift.Client.Elements
{
    /// <summary>
    /// Interaction logic for HybridElement.xaml
    /// </summary>
    public partial class HybridElement : UserControl
    {
        public HybridElement(string number)
        {
            InitializeComponent();

            Number.Content = number;
        }
    }
}
