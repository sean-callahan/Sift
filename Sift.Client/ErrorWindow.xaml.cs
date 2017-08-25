using System;
using System.Windows;

namespace Sift.Client
{
    /// <summary>
    /// Interaction logic for ErrorWindow.xaml
    /// </summary>
    public partial class ErrorWindow : Window
    {
        public ErrorWindow(string message, string stackTrace)
        {
            InitializeComponent();

            Heading.Content = message;
            Detail.Text = stackTrace;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
