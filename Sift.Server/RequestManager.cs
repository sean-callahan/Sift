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
            Program.Server.RequestLine += Server_RequestLine; ;
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
            AsteriskLink link;
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

            AsteriskLink link = new AsteriskLink(Program, (Asterisk)Program.Provider, Program.Lines[e.Index].Caller);
            link.Dial("2002");
        }

        private void Server_RequestDump(object sender, RequestDump e)
        {
            Program.Provider.Hangup(Program.Lines[e.Index].Caller.Id);
        }

        private void Server_LoginRequest(object sender, LoginRequest e)
        {
            Program.Server.Send((NetConnection)sender, new UpdateAppState(8));
        }
    }
}
