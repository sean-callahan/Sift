using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using IniParser;
using IniParser.Model;

using Sift.Common;
using Sift.Common.Net;
using Sift.Server.Asterisk;

namespace Sift.Server
{
    internal class Program
    {
        public const string Version = "0.3.1";

        public static void Main(string[] args)
        {
            ManualResetEvent exit = new ManualResetEvent(false);
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                exit.Set();
            };

            string motd = "    __________________\n" +
                "   / __/  _/ __/_  __/\n" +
                "  _\\ \\_/ // _/  / /    Call Screener\n" +
                $" /___/___/_/   /_/     Server Version {Version}\n";
            Console.WriteLine(motd);

            try
            {
                Program p = new Program();
                p.Connect();
                p.ProcessNetwork(exit);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public bool Running { get; set; }

        public IVoipProvider Provider { get; }
        public IReadOnlyList<Line> Lines { get; }

        public IGroup HoldGroup { get; private set; }

        public Dictionary<Caller, ILink> LinkedCallers { get; } = new Dictionary<Caller, ILink>();

        public SdpServer Server { get; }

        public static DatabaseEngine Database { get; private set; }

        public Program()
        {
            IniData data = ReadConfigFile("sift.ini");

            int numLines = int.Parse(data["General"]["lines"]);

            Provider = VoipProviderFactory.Create(data["Provider"]);

            Logger.Log("Loading database...");
            try
            {
                Database = new DatabaseEngine(data["Database"]);
                Database.Initialize();
            }
            catch (Exception e)
            {
                Logger.Log("Could not connect to database.", Logger.Level.Error);
                Logger.Log(e.InnerException);
                return;
            }
            
            if (!LoginManager.UserExists("admin"))
            {
                LoginManager.Create("admin", "changeme");
                Logger.Log("Created 'admin' user account");
            }

            List<Line> lines = new List<Line>(numLines);
            for (byte i = 0; i < numLines; i++)
            {
                lines.Add(new Line(i));
            }

            Lines = lines;
            Logger.Log($"Initialized {numLines} lines");

            Provider.CallerStart += Provider_CallerStart;
            Provider.CallerEnd += Provider_CallerEnd;
            Provider.ConnectionStateChanged += Provider_ConnectionStateChanged;

            Server = new SdpServer(7777);
            Server.Manager.Connection += ServerConnection;
            Server.Manager.Disconnected += ServerDisconnection;
            Server.Start();

            new RequestManager(this);
            new UpdateManager(this);
        }

        private void ServerConnection(string id)
        {
            Logger.Log("Client connected with ID = " + id);
        }

        private void ServerDisconnection(string id)
        {
            Logger.Log("Client with ID = " + id + " disconnected.");
        }

        private void ProcessNetwork(ManualResetEvent exit)
        {
            Tuple<string, ISdpPacket> p;
            while (!exit.WaitOne(1))
            {
                if (!Server.Manager.IncomingPackets.TryDequeue(out p))
                    continue;

                Logger.Log("Got packet " + p.Item2.Type, Logger.Level.Debug);

                Server.Manager.HandlePacket(p.Item1, p.Item2);
            }
        }

        private void Provider_ConnectionStateChanged(object sender, VoipProviderConnectionState state)
        {
            switch (state)
            {
                case VoipProviderConnectionState.Connecting:
                    Logger.Log($"Trying to connect to {Provider.Name}...");
                    break;
                case VoipProviderConnectionState.Open:
                    Logger.Log($"Connected to {Provider.Name}");
                    break;
                case VoipProviderConnectionState.Closed:
                    Server.Broadcast(new Error
                    {
                        Message = "Provider connection lost.",
                        Detail = $"The connection to the provider {Provider.Name} has been lost. " +
                            "To continue using Sift, please restart the server appplication and wait for a successful provider connection. " +
                            "Please restart all clients once the server application has been restarted.",
                        Fatal = true,
                    });
                    break;
            }
        }

        public async void Connect()
        {
            Provider.Connect();

            int tries = 0;
            while (!Provider.Connected)
            {
                tries++;
                if (tries > 40)
                    throw new Exception($"Could not connect to {Provider.Name}");
                await Task.Delay(1000);
            }

            try
            {
                HoldGroup = new AsteriskGroup(Provider, GroupType.Holding);
                Logger.Log("Created hold group with ID " + HoldGroup.Id.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void Provider_CallerEnd(object sender, Caller c)
        {
            foreach (Line line in Lines)
            {
                if (line.Caller != c)
                    continue;

                if (HoldGroup.Contains(c))
                    HoldGroup.Remove(c);
                if (LinkedCallers.ContainsKey(c))
                    LinkedCallers[c].Dispose();

                line.Caller = null;
                line.State = LineState.Empty;
                Server.Broadcast(new LineStateChanged(line));

                break;
            }
        }

        private int NextLine
        {
            get
            {
                for (int i = 0; i < Lines.Count; i++)
                {
                    if (Lines[i].Caller == null)
                        return i;
                }
                return -1;
            }
        }

        private void Provider_CallerStart(object sender, Caller c)
        {
            int next = NextLine;
            if (next < 0)
            {
                Logger.Log(c, "Caller denied because lines are full.");
                Provider.Hangup(c.Id);
                return;
            }

            Line line = Lines[next];
            line.Caller = c;
            line.State = LineState.Ringing;

            Logger.Log(c, "Assigning line " + (line.Index + 1) + " to caller");

            Provider.Ring(c.Id);

            Server.Broadcast(new InitializeLine(line));
        }

        private static FileIniDataParser parser = new FileIniDataParser();

        private static IniData ReadConfigFile(string name)
        { 
            return parser.ReadFile(name);
        }
    }
}
