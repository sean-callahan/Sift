using Lidgren.Network;

using Sift.Common;
using Sift.Common.Network;

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
        }

        private void Server_RequestUserLogin(object sender, User user)
        {
            NetConnection conn = (NetConnection)sender;
            if (LoginManager.Login(Program, user))
                conn.Approve();
            else
                conn.Deny("Invalid username or password");
        }

        private void Server_RequestAir(object sender, RequestAir e)
        {
            Line line = Program.Lines[e.Index];

            if (line == null || line.Caller == null || Program.LinkedCallers.ContainsKey(line.Caller))
                return;

            Program.HoldGroup.Remove(line.Caller);

            AsteriskLink link = new AsteriskLink(Program, (Asterisk)Program.Provider, Program.Lines[e.Index].Caller, "2001");
            link.Start();

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
                
            Program.HoldGroup.Add(Program.Lines[e.Index].Caller);

            Program.Lines[e.Index].State = LineState.Hold;
            Program.Server.Broadcast(new UpdateLineState(Program.Lines[e.Index]));
        }

        private void Server_RequestScreen(object sender, RequestScreen e)
        {
            Line line = Program.Lines[e.Index];

            if (Program.LinkedCallers.ContainsKey(line.Caller))
                return;
            
            line.State = LineState.Screening;
            Program.Server.Broadcast(new UpdateLineState(line));

            AsteriskLink link = new AsteriskLink(Program, (Asterisk)Program.Provider, Program.Lines[e.Index].Caller, "2002");
            link.Start();
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
