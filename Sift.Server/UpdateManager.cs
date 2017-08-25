using System;

using Sift.Common;
using Sift.Common.Network;

namespace Sift.Server
{
    internal class UpdateManager
    {
        public Program Program { get; }

        public UpdateManager(Program program)
        {
            Program = program;

            Program.Server.UpdateLineState += Server_UpdateLineState;
        }

        private void Server_UpdateLineState(object sender, UpdateLineState e)
        {
            Line line = Program.Lines[e.Index];
            if (line.Caller == null)
                return;

            line.State = e.State;
            line.Caller.Name = e.Name;
            line.Caller.Location = e.Location;
            line.Caller.Comment = e.Comment;
            line.Caller.Created = new DateTime().AddSeconds(e.Created);

            Program.Server.Broadcast(new UpdateLineState(line));
        }
    }
}
