using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{

    [Message(GameMessageOpcode.ServerGuildJoin)]
    public class ServerGuildJoin : IWritable
    {
        public Guild Guild { get; set; } = new Guild();
        public bool Unknown0 { get; set; } = false;

        public void Write(GamePacketWriter writer)
        {
            Guild.Write(writer);
            writer.Write(Unknown0);
        }
    }
}
