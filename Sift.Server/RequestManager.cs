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
        }

        private void Server_RequestScreen(object sender, RequestScreen e)
        {
            if (((Asterisk)Program.Provider).ActiveLink != null)
                return;

            Line line = Program.Lines[e.Index];
            line.State = LineState.Screening;
            Program.Server.Broadcast(new UpdateLineState(line));

            AsteriskLink link = new AsteriskLink(Program, (Asterisk)Program.Provider, Program.Lines[e.Index].Caller);
            link.Dial("2002");

            ((Asterisk)Program.Provider).ActiveLink = link;
        }

        private void Server_RequestDump(object sender, RequestDump e)
        {
            Program.Provider.Hangup(Program.Lines[e.Index].Caller);
        }

        private void Server_LoginRequest(object sender, LoginRequest e)
        {
            Program.Server.Send((NetConnection)sender, new UpdateAppState(8));
        }
    }
}
