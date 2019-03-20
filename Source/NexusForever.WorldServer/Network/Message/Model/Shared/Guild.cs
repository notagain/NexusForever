using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model.Shared
{
    public class Guild : IWritable
    {
        public GuildData GuildData { get; set; } = new GuildData();
        public GuildMember GuildLeader { get; set; } = new GuildMember();
        public GuildUnknown GuildUnknown { get; set; } = new GuildUnknown();

        public void Write(GamePacketWriter writer)
        {
            GuildData.Write(writer);
            GuildLeader.Write(writer);
            GuildUnknown.Write(writer);
        }
    }
}
