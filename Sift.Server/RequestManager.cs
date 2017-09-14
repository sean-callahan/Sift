using System.Collections.Generic;
using System.Data;

using Lidgren.Network;

using Sift.Common;
using Sift.Common.Network;
using Sift.Server.Asterisk;
using Sift.Server.Util;

namespace Sift.Server
{
    internal class RequestManager
    {
        public Program Program { get; }

        public RequestManager(Program program)
        {
            Program = program;
            
            Program.Server.LoginRequest += Server_LoginRequest;
            Program.Server.RequestDump += Server_RequestDump;
            Program.Server.RequestScreen += Server_RequestScreen;
            Program.Server.RequestHold += Server_RequestHold;
            Program.Server.RequestLine += Server_RequestLine;
            Program.Server.RequestAir += Server_RequestAir;
            Program.Server.RequestUserLogin += Server_RequestUserLogin;
            Program.Server.SettingsChanged += Server_SettingsChanged;
        }

        private void Server_SettingsChanged(object sender, SettingsChanged e)
        {
            if (string.IsNullOrWhiteSpace(e.Category))
                return;
            using (IDbConnection db = Program.Database.NewConnection())
            {
                db.Open();
                IDbCommand cmd = DbUtil.CreateCommand(db,
                    "SELECT key, value FROM settings WHERE category=@category;",
                    new Dictionary<string, object> { { "@category", e.Category } });
                using (IDataReader reader = cmd.ExecuteReader())
                {
                    Dictionary<string, object> items = new Dictionary<string, object>();
                    while (reader.Read())
                    {
                        IDataRecord rec = reader;
                        items.Add((string)rec["key"], rec["value"]);
                    }
                    Program.Server.Send((NetConnection)sender, new SettingsChanged(e.Category, new SettingsCollection(items)));
                }
            }
        }

        private void Server_RequestUserLogin(object sender, User user)
        {
            NetConnection conn = (NetConnection)sender;
            if (LoginManager.Login(Program.Database, user))
            {
                conn.Approve();
                Logger.Log("Approved connection with username '" + user.Username + "'");
            }
            else
            {
                conn.Deny("Invalid username or password");
                Logger.Log("Denied connection with username '" + user.Username + "'");
            }
        }

        private void Server_RequestAir(object sender, RequestAir e)
        {
            Line line = Program.Lines[e.Index];

            if (line == null || line.Caller == null || Program.LinkedCallers.ContainsKey(line.Caller))
                return;

            if (Program.HoldGroup.Contains(line.Caller))
            {
                Program.HoldGroup.Remove(line.Caller);
                Logger.Log(line.Caller, "Removed from group " + Program.HoldGroup.Id);
            }

            if (Program.LinkedCallers.ContainsKey(line.Caller))
            {
                ILink existing = Program.LinkedCallers[line.Caller];
                Logger.Log(line.Caller, "Already linked with linker " + existing.Id, Logger.Level.Warning);
                return;
            }

            ILink link = new AsteriskLink(Program, (AsteriskProvider)Program.Provider, Program.Lines[e.Index].Caller, "2001");
            link.Start();

            Program.LinkedCallers[line.Caller] = link;

            line.State = LineState.OnAir;
            Program.Server.Broadcast(new UpdateLineState(line));
        }

        private void Server_RequestLine(object sender, RequestLine e)
        {
            if (e.Index >= 0)
            {
                Program.Server.Send((NetConnection)sender, new UpdateLineState(Program.Lines[e.Index]));
                return;
            }
            foreach (Line line in Program.Lines)
                Program.Server.Send((NetConnection)sender, new UpdateLineState(line));
        }

        private void Server_RequestHold(object sender, RequestHold e)
        {
            ILink link;
            if (!Program.LinkedCallers.TryGetValue(Program.Lines[e.Index].Caller, out link))
                return;
            link.Dispose();

            Caller c = Program.Lines[e.Index].Caller;
            if (c == null)
                return;

            if (Program.HoldGroup.Contains(c))
            {
                Logger.Log(c, "Already contained in group " + Program.HoldGroup.Id, Logger.Level.Warning);
            }
            else
            {
                Program.HoldGroup.Add(c);
                Logger.Log(c, "Added to group " + Program.HoldGroup.Id);
            }

            Program.Lines[e.Index].State = LineState.Hold;
            Program.Server.Broadcast(new UpdateLineState(Program.Lines[e.Index]));
        }

        private void Server_RequestScreen(object sender, RequestScreen e)
        {
            Line line = Program.Lines[e.Index];

            if (line.Caller == null || Program.LinkedCallers.ContainsKey(line.Caller))
                return;
            
            line.State = LineState.Screening;
            Program.Server.Broadcast(new UpdateLineState(line));

            if (Program.LinkedCallers.ContainsKey(line.Caller))
            {
                ILink existing = Program.LinkedCallers[line.Caller];
                Logger.Log(line.Caller, "Already linked with linker " + existing.Id, Logger.Level.Warning);
                return;
            }

            ILink link = new AsteriskLink(Program, (AsteriskProvider)Program.Provider, Program.Lines[e.Index].Caller, "2002");
            link.Start();
            
            Program.LinkedCallers[line.Caller] = link;
        }

        private void Server_RequestDump(object sender, RequestDump e)
        {
            Program.Provider.Hangup(Program.Lines[e.Index].Caller.Id);
        }

        private void Server_LoginRequest(object sender, LoginRequest e)
        {
            Program.Server.Send((NetConnection)sender, new UpdateAppState(Program.Lines.Count, Program.Provider.Type));
        }
    }
}
