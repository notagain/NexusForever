namespace NexusForever.WorldServer.Database.World.Model
{
    public partial class StoreOfferGroup
    {
        public uint Id { get; set; }
        public uint DisplayFlags { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ushort Field2 { get; set; }
        public bool Visible { get; set; }
    }
}
