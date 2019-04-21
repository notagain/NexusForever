using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model.Shared
{
    public class GuildHolomark : IWritable, IReadable
    {
        public class HolomarkPart: IWritable, IReadable
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

            public void Read(GamePacketReader reader)
            {
                GuildStandardPartId = reader.ReadUShort(14u);
                DyeColorRampId1 = reader.ReadUShort(14u);
                DyeColorRampId2 = reader.ReadUShort(14u);
                DyeColorRampId3 = reader.ReadUShort(14u);
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

        public void Read(GamePacketReader reader)
        {
            HolomarkPart1.Read(reader);
            HolomarkPart2.Read(reader);
            HolomarkPart3.Read(reader);
        }
    }
}
