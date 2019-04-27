using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Game.Guild.Static;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NexusForever.WorldServer.Game.Guild
{
    public abstract class GuildBase
    {
        public ulong Id { get; }
        public GuildType Type { get; }
        public string Name { get; protected set; }
        public ulong LeaderId { get; protected set; }
        public GuildMember Leader { get; protected set; }
        public DateTime CreateTime { get; }

        public Dictionary</*index*/byte, Rank> Ranks { get; private set; } = new Dictionary<byte, Rank>();
        public Dictionary</*id*/ulong, GuildMember> Members { get; private set; } = new Dictionary<ulong, GuildMember>();
        public Dictionary</*id*/ulong, WorldSession> OnlineMembers { get; private set; } = new Dictionary<ulong, WorldSession>();

        protected GuildBase(GuildType guildType)
        {
            Id = 1;
            Type = guildType;
            CreateTime = DateTime.Now;
        }

        protected IEnumerable<GuildData.Rank> GetRanks()
        {
            foreach (Rank rank in Ranks.Values)
            {
                yield return rank.GetGuildDataRank();
            }
        }

        public GuildMember GetGuildMember(ulong characterId)
        {
            return Members.Values.FirstOrDefault(i => i.CharacterId == characterId);
        }

        public IEnumerable<GuildMember> GetMembers()
        {
            return Members.Values;
        }
    }
}
