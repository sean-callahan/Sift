using System.Data;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

using Sift.Common;
using Sift.Common.Net;
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

            // Line actions
            Program.Server.Manager.Air += Server_AirLine;
            Program.Server.Manager.Dump += Server_DumpLine;
            Program.Server.Manager.Screen += Server_ScreenLine;
            Program.Server.Manager.Hold += Server_HoldLine;

            Program.Server.Manager.Connection += Manager_Connection;
            Program.Server.Manager.SettingsQuery += Server_SettingsQuery;


            //Program.Server.LoginRequest += Server_LoginRequest;
            //Program.Server.LineRequest += Server_LineRequest;
            //Program.Server.RequestUserLogin += Server_RequestUserLogin;
            //Program.Server.RequestSettings += Server_RequestSettings;
        }

        private void Manager_Connection(string id)
        {
            Program.Server.SendTo(id, new InitializeClient()
            {
                Lines = (byte)Program.Lines.Count,
                Provider = Program.Provider.Type,
            });
        }

        private void Server_SettingsQuery(string id, SettingsQuery e)
        {
            Logger.DebugLog("Requested settings " + (string.IsNullOrWhiteSpace(e.Key) ? "*" : e.Key) + " from " + (string.IsNullOrWhiteSpace(e.Category) ? "*" : e.Category));
            BinaryFormatter formatter = new BinaryFormatter();
            /*using (var ctx = new SettingContext())
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
                //Program.Server.SendTo(id, new SettingsQuery());
            }*/
        }

        /*private void Server_RequestUserLogin(NetworkUser user)
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
        }*/

        private void Server_AirLine(string id, byte index)
        {
            Line line = Program.Lines[index];

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

            ILink link = new AsteriskLink(Program, (AsteriskProvider)Program.Provider, line.Caller, "2001");
            link.Start();

            Program.LinkedCallers[line.Caller] = link;

            line.State = LineState.OnAir;
            Program.Server.Broadcast(new LineStateChanged(line));
        }

        /*private void Server_LineRequest(InitializeLine packet)
        {
            if (packet.Index >= 0)
            {
                Program.Server.Send((NetConnection)sender, new InitializeLine(Program.Lines[e.Index]));
                return;
            }
            foreach (Line line in Program.Lines)
                Program.Server.Send((NetConnection)sender, new InitializeLine(line));
        }*/

        private void Server_HoldLine(string id, byte index)
        {
            if (!Program.LinkedCallers.TryGetValue(Program.Lines[index].Caller, out ILink link))
                return;
            link.Dispose();

            Line line = Program.Lines[index];
            
            if (line.Caller == null)
                return;

            if (Program.HoldGroup.Contains(line.Caller))
            {
                Logger.Log(line.Caller, "Already contained in group " + Program.HoldGroup.Id, Logger.Level.Warning);
            }
            else
            {
                Program.HoldGroup.Add(line.Caller);
                Logger.Log(line.Caller, "Added to group " + Program.HoldGroup.Id);
            }

            Program.Lines[index].State = LineState.Hold;
            Program.Server.Broadcast(new LineStateChanged(line));
        }

        private void Server_ScreenLine(string id, byte index)
        {
            Line line = Program.Lines[index];
            if (line.Caller == null || Program.LinkedCallers.ContainsKey(line.Caller))
                return;
            
            if (Program.LinkedCallers.ContainsKey(line.Caller))
            {
                ILink existing = Program.LinkedCallers[line.Caller];
                Logger.Log(line.Caller, "Already linked with linker " + existing.Id, Logger.Level.Warning);
                return;
            }

            if (!Program.Settings.TryGetValue("asterisk:screener_extension", out byte[] extenBytes))
                return;

            string exten = System.Text.Encoding.UTF8.GetString(extenBytes);

            ILink link = new AsteriskLink(Program, (AsteriskProvider)Program.Provider, Program.Lines[index].Caller, exten);
            link.Start();


            line.State = LineState.Screening;
            Program.Server.Broadcast(new LineStateChanged(line));


            Program.LinkedCallers[line.Caller] = link;
        }

        private void Server_DumpLine(string id, byte index)
        {
            Program.Provider.Hangup(Program.Lines[index].Caller.Id);
        }

        /*private void Server_LoginRequest(object sender, LoginRequest e)
        {
            Program.Server.Send((NetConnection)sender, new UpdateAppState((byte)Program.Lines.Count, Program.Provider.Type));
        }*/
    }
}
