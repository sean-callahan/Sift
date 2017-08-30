﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sift.Common;
using Sift.Common.Network;

namespace Sift.Server
{
    internal class Program
    {
        public const string Version = "0.1.0";

        public static void Main(string[] args)
        {
            string motd = "    __________________\n" +
                "   / __/  _/ __/_  __/\n" +
                "  _\\ \\_/ // _/  / /    Call Screener\n" +
                $" /___/___/_/   /_/     Server Version {Version}\n";
            Console.WriteLine(motd);

            try
            {
                Program p = new Program(new Asterisk("10.199.1.172", 8088, "asterisk", "asterisk", "hello-world"), 12);
                p.Connect();
                CancellationTokenSource cts = new CancellationTokenSource();
                p.ProcessLoop(cts.Token);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadLine();
            }
        }

        public bool Running { get; set; }

        public IVoipProvider Provider { get; }
        public IReadOnlyList<Line> Lines { get; }

        public IGroup HoldGroup { get; private set; }
        public Dictionary<Caller, ILink> LinkedCallers { get; } = new Dictionary<Caller, ILink>(); 

        public SdpServer Server { get; }

        public Program(IVoipProvider provider, int numLines)
        {
            using (LiteDB.LiteDatabase db = new LiteDB.LiteDatabase(@".\Data.db"))
            {
                CreateAndVerifyDatabase(db);
            }

            List<Line> lines = new List<Line>(numLines);

            for (int i = 0; i < numLines; i++)
            {
                lines.Add(new Line(i));
            }

            Lines = lines;
            Provider = provider;

            provider.CallerStart += Provider_CallerStart;
            provider.CallerEnd += Provider_CallerEnd;
            provider.ConnectionStateChanged += Provider_ConnectionStateChanged;
            
            Server = new SdpServer(7777);
            Server.Start();

            new RequestManager(this);
            new UpdateManager(this);
        }

        private void CreateAndVerifyDatabase(LiteDB.LiteDatabase db)
        {
            var col = db.GetCollection<User>("users");
            if (col.Count() < 1)
            {
                LoginManager.Create("admin", "changeme");
            }
        }

        private void Provider_ConnectionStateChanged(object sender, VoipProviderConnectionState state)
        {
            switch (state)
            {
                case VoipProviderConnectionState.Connecting:
                    Console.WriteLine($"Trying to connect to {Provider.Name}...");
                    break;
                case VoipProviderConnectionState.Open:
                    Console.WriteLine($"Connected to {Provider.Name}");
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
                await Task.Delay(250);
            }

            try
            {
                HoldGroup = new AsteriskGroup(Provider, GroupType.Holding);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void ProcessLoop(CancellationToken cancel)
        {
            while (!cancel.IsCancellationRequested)
            {
                Server.TryReadMessage();
                Thread.Sleep(1);
            }
        }

        private void Provider_CallerEnd(object sender, Caller c)
        {
            for (int i = 0; i < Lines.Count; i++)
            {
                if (Lines[i].Caller == c)
                {
                    Lines[i].Caller = null;
                    Lines[i].State = LineState.Empty;
                    Console.WriteLine("caller: removed from index " + i);
                    Server.Broadcast(new UpdateLineState(Lines[i]));
                    break;
                }
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
                Console.WriteLine("denying caller: lines are full");
                Provider.Hangup(c.Id);
                return;
            }

            Line line = Lines[next];
            line.Caller = c;
            line.State = LineState.Ringing;

            Console.WriteLine("caller: assigning index " + next);

            Provider.Ring(c.Id);

            Server.Broadcast(new UpdateLineState(line));
        }
    }
}
