using System;
using System.Windows;

namespace Sift.Client
{
    /// <summary>
    /// Interaction logic for ErrorWindow.xaml
    /// </summary>
    public partial class ErrorWindow : Window
    {
        public ErrorWindow(Exception e)
        {
            InitializeComponent();

            Heading.Content = e.Message;
            Detail.Text = e.Source + "\n" + e.StackTrace;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
