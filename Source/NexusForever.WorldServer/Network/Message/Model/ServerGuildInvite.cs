using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerGuildInvite)]
    public class ServerGuildInvite : IWritable
    {
        public string PlayerName { get; set; }
        public string GuildName { get; set; }
        public uint Unknown2 { get; set; }
        public byte Unknown3 { get; set; }
        public ulong Unknown4 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.WriteStringWide(PlayerName);
            writer.WriteStringWide(GuildName);
            writer.Write(Unknown2);
            writer.Write(Unknown3, 4u);
            writer.Write(Unknown4);
        }
    }
}
