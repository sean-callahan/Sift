using System.Data;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

using Lidgren.Network;

using Sift.Common;
using Sift.Common.Network;
using Sift.Server.Asterisk;
using Sift.Server.Db;

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
            Program.Server.RequestSettings += Server_RequestSettings;
        }

        private void Server_RequestSettings(object sender, RequestSettings e)
        {
            Logger.DebugLog("Requested settings " + (string.IsNullOrWhiteSpace(e.Key) ? "*" : e.Key) + " from " + (string.IsNullOrWhiteSpace(e.Category) ? "*" : e.Category));
            BinaryFormatter formatter = new BinaryFormatter();
            using (var ctx = new SettingContext())
            {
                Setting[] settings;
                bool hasCategory = !string.IsNullOrWhiteSpace(e.Category);
                bool hasKey = !string.IsNullOrWhiteSpace(e.Key);
                if (hasCategory && !hasKey)
                {
                    settings = ctx.Settings.Where(s => s.Category == e.Category).ToArray();
                }
                else if (hasCategory && hasKey)
                {
                    settings = ctx.Settings.Where(s => s.Category == e.Category && s.Key == e.Key).ToArray();
                }
                else
                {
                    return;
                }
                Logger.DebugLog("Requested settings " + (string.IsNullOrWhiteSpace(e.Key) ? "*" : e.Key) + " from " + (string.IsNullOrWhiteSpace(e.Category) ? "*" : e.Category) + " retrieved " + settings.Length + " results");
                if (settings.Length == 0)
                    return;
                NetworkSetting[] net = new NetworkSetting[settings.Length];
                for (int i = 0; i < settings.Length; i++)
                {
                    net[i] = new NetworkSetting(settings[i].Category, settings[i].Key, settings[i].Value);
                }
                Program.Server.Send((NetConnection)sender, new UpdateSettings(net));
            }
        }

        private void Server_RequestUserLogin(object sender, NetworkUser user)
        {
            NetConnection conn = (NetConnection)sender;
            if (LoginManager.Login(user))
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

            string exten;
            using (var ctx = new SettingContext())
            {
                var item = ctx.Settings.Where(x => x.Key == "asterisk_screener_extension").FirstOrDefault();
                if (item == null)
                    return;
                NetworkSetting setting = item.ToNetworkSetting();
                exten = ((int)setting.Value).ToString();
            }

            ILink link = new AsteriskLink(Program, (AsteriskProvider)Program.Provider, Program.Lines[e.Index].Caller, exten);
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
