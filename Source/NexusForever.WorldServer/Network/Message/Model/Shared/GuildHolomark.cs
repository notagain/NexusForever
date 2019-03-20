using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model.Shared
{
    public class GuildHolomark : IWritable
    {
        public class HolomarkPart: IWritable
        {
            public ushort GuildStandardPartId { get; set; }
            public ushort DyeColorRampId1 { get; set; }
            public ushort DyeColorRampId2 { get; set; }
            public ushort DyeColorRampId3 { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(GuildStandardPartId, 14u);
                writer.Write(DyeColorRampId1, 14u);
                writer.Write(DyeColorRampId2, 14u);
                writer.Write(DyeColorRampId3, 14u);
            }
        }

        public HolomarkPart HolomarkPart1 { get; set; } = new HolomarkPart();
        public HolomarkPart HolomarkPart2 { get; set; } = new HolomarkPart();
        public HolomarkPart HolomarkPart3 { get; set; } = new HolomarkPart();

        public void Write(GamePacketWriter writer)
        {
            HolomarkPart1.Write(writer);
            HolomarkPart2.Write(writer);
            HolomarkPart3.Write(writer);
        }
    }
}
