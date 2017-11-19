using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using Sift.Common;
using Sift.Common.Net;
using Sift.Common.Network;
using Sift.Server.Db;

namespace Sift.Server
{
    internal class UpdateManager
    {
        public Program Program { get; }

        public UpdateManager(Program program)
        {
            Program = program;

            Program.Server.Manager.LineMetadata += Server_LineMetadata;
            //Program.Server.UpdateSettings += Server_UpdateSettings;
        }

        private static IFormatter formatter = new BinaryFormatter();

        private void Server_UpdateSettings(object sender, Settings e)
        {
            /*using (var ctx = new SettingContext())
            {
                foreach (NetworkSetting net in e.Items)
                {
                    Setting setting = ctx.Settings.Where(s => s.Key == net.Key).FirstOrDefault();
                    if (setting == null)
                        setting = new Setting
                        {
                            Key = net.Key,
                            Category = net.Category,
                        };
                    setting.Value = net.Body;
                    
                }
                ctx.SaveChanges();
            }*/
        }

        private void Server_LineMetadata(string id, LineMetadata e)
        {
            Line line = Program.Lines[e.Index];
            if (line.Caller == null)
                return;
            
            line.Caller.Name = e.Name;
            line.Caller.Location = e.Location;
            line.Caller.Comment = e.Comment;

            Program.Server.Broadcast(new LineMetadata(line));
        }
    }
}
