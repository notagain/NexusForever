using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{

    [Message(GameMessageOpcode.ServerGuildResult, MessageDirection.Server)]
    public class ServerGuildResult : IWritable
    {
        public ushort Realm { get; set; }
        public ulong CharacterId { get; set; }
        public uint Unknown0 { get; set; }
        public string GuildName { get; set; }
        public byte Result { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Realm, 14u);
            writer.Write(CharacterId);
            writer.Write(Unknown0);
            writer.WriteStringWide(GuildName);
            writer.Write(Result);
        }
    }
}
