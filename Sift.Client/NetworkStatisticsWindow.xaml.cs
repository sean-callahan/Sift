using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace Sift.Client
{
    /// <summary>
    /// Interaction logic for NetworkStatistics.xaml
    /// </summary>
    public partial class NetworkStatisticsWindow : Window
    {
        private const int ItemCount = 4;

        private readonly DispatcherTimer timer = new DispatcherTimer();
        private readonly string[] items = new string[ItemCount];
        //private readonly NetPeer peer;

        public NetworkStatisticsWindow(/*NetPeer peer*/)
        {
            //this.peer = peer ?? throw new ArgumentNullException("peer");

            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 1);

            InitializeComponent();

            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            /*if (peer == null)
                return;

            NetPeerStatistics stats = peer.Statistics;

            items[0] = "Received Packets: " + stats.ReceivedPackets;
            items[1] = "Received Bytes: " + SizeSuffix(stats.ReceivedBytes);
            items[2] = "Sent Packets: " + stats.SentPackets;
            items[3] = "Sent Bytes: " + SizeSuffix(stats.SentBytes);

            Stats.ItemsSource = items;*/
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            timer.Stop();
        }

        // Credit: JLRishe | https://stackoverflow.com/questions/14488796/does-net-provide-an-easy-way-convert-bytes-to-kb-mb-gb-etc
        private static readonly string[] SizeSuffixes =
                   { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        private static string SizeSuffix(long value, int decimalPlaces = 1)
        {
            if (value < 0) { return "-" + SizeSuffix(-value); }
            if (value == 0) { return "0.0 bytes"; }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                SizeSuffixes[mag]);
        }
    }
}
