using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Network.Message.Model
{

    [Message(GameMessageOpcode.ServerGuildInit)]
    public class ServerGuildInit : IWritable
    {
        public uint Unknown0 { get; set; }
        public List<Guild> Guilds { get; set; } = new List<Guild>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Guilds.Count);
            writer.Write(Unknown0);
            Guilds.ForEach(w => w.Write(writer));
        }
    }
}
