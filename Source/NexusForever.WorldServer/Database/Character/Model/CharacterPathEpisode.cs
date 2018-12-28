using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.Character.Model
{
    public partial class CharacterPathEpisode
    {
        public ulong Id { get; set; }
        public uint EpisodeId { get; set; }
        public bool RewardReceived { get; set; }

        public Character Character { get; set; }
    }
}
