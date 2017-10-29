using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;

using Sift.Common.Net;

namespace Sift.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal const string Version = "0.3.1";

        private SdpClient Client { get; }

        private BackgroundWorker worker;

        public App()
        {
            Client = new SdpClient();
            Client.Manager.Error += (string id, Error e) => DisplayError(e.Message, e.Detail, e.Fatal);

            worker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true
            };
            worker.DoWork += Worker_DoWork;
        }

        public static void DisplayError(string msg, string detail, bool fatal = false)
        {
            new ErrorWindow(msg, detail).Show();
            if (fatal)
            {
                foreach (Window w in Current.Windows)
                {
                    if (!(w is ErrorWindow))
                    {
                        w.Close();
                    }
                }
            }
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            worker.RunWorkerAsync();
            new LoginWindow(Client).Show();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            while (!worker.CancellationPending)
            {
                Thread.Sleep(1);

                if (Client.Manager.IncomingPackets.IsEmpty)
                    continue;

                Client.Manager.IncomingPackets.TryDequeue(out Tuple<string, ISdpPacket> p);
                if (p == null)
                    continue;

                Dispatcher?.BeginInvoke(new Action(() =>
                {
                    Client.Manager.HandlePacket(p.Item1, p.Item2);
                }));
            }
        }
    }
}
