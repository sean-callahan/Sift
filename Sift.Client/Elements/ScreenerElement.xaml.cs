﻿using System.Windows.Controls;

using Sift.Common;
using Sift.Common.Net;

namespace Sift.Client.Elements
{
    /// <summary>
    /// Interaction logic for ScreenerElement.xaml
    /// </summary>
    public partial class ScreenerElement : UserControl
    {
        private Line line;
        public Line Line
        {
            get { return line; }
            set
            {
                if (line == value)
                    return;
                if (value == null || value.Caller == null)
                {
                    Clear();
                    SetEnabled(false);
                }
                else
                {
                    Show(value);
                    SetEnabled(true);
                }
                line = value;
            }
        }

        private SdpClient client;

        public ScreenerElement(SdpClient client)
        {
            InitializeComponent();

            this.client = client;
        }

        private void Show(Line line)
        {
            if (line.Caller == null)
                Clear();
            Number.Text = line.Caller.Number;
            CallerName.Text = line.Caller.Name;
            Location.Text = line.Caller.Location;
            Comment.Text = line.Caller.Comment;
        }

        public void Clear()
        {
            Number.Text = string.Empty;
            CallerName.Text = string.Empty;
            Location.Text = string.Empty;
            Comment.Text = string.Empty;
        }

        internal void SetEnabled(bool enabled)
        {
            CallerName.IsEnabled = enabled;
            Location.IsEnabled = enabled;
            Comment.IsEnabled = enabled;
        }

        private void Save_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (line == null || line.Caller == null)
                return;
            
            line.Caller.Name = CallerName.Text;
            line.Caller.Location = Location.Text;
            line.Caller.Comment = Comment.Text;

            client.Send(new LineMetadata(line));
        }
    }
}
