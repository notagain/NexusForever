using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.Character.Model
{
    public partial class CharacterPathMission
    {
        public ulong Id { get; set; }
        public uint MissionId { get; set; }
        public bool Completed { get; set; }
        public uint Progress { get; set; }
        public uint State { get; set; }

        public Character Character { get; set; }
    }
}
