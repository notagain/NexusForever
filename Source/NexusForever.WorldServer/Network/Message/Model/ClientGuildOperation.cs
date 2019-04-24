using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Guild.Static;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientGuildOperation)]
    public class ClientGuildOperation : IReadable
    {
        public TargetPlayerIdentity PlayerIdentity { get; private set; } = new TargetPlayerIdentity();
        public uint Guid { get; private set; }
        public ulong Id { get; private set; }
        public string Text { get; private set; }
        public GuildOperation Operation { get; private set; }

        public void Read(GamePacketReader reader)
        {
            PlayerIdentity.Read(reader);
            Guid = reader.ReadUInt();
            Id = reader.ReadULong();
            Text = reader.ReadWideString();
            Operation = reader.ReadEnum<GuildOperation>(6u);
        }
    }
}
