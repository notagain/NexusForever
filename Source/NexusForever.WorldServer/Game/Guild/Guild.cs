using NexusForever.WorldServer.Database.Character;
using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Guild.Static;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NexusForever.WorldServer.Game.Guild
{
    public class Guild : GuildBase
    {
        public uint Taxes { get; private set; }
        public GuildStandard GuildStandard { get; private set; }

        /// <summary>
        /// Create a new <see cref="Guild"/>
        /// </summary>
        public Guild(WorldSession leaderSession, string guildName, string leaderRankName, string councilRankName, string memberRankName, GuildStandard guildStandard)
            : base(GuildType.Guild)
        {
            Name = guildName;
            LeaderId = leaderSession.Player.CharacterId;
            Taxes = 0;

            Ranks.Add(0, new Rank(leaderRankName, 0, GuildRankPermission.Leader, ulong.MaxValue, long.MaxValue, long.MaxValue));
            Ranks.Add(1, new Rank(councilRankName, 1, (GuildRankPermission.CouncilChat | GuildRankPermission.MemberChat | GuildRankPermission.Kick | GuildRankPermission.Invite | GuildRankPermission.ChangeMemberRank | GuildRankPermission.Vote), ulong.MaxValue, long.MaxValue, long.MaxValue));
            Ranks.Add(2, new Rank(memberRankName, 2, GuildRankPermission.MemberChat, 0, 0, 0));

            GuildStandard = guildStandard;

            Player player = leaderSession.Player;
            Leader = new GuildMember
            {
                Realm = WorldServer.RealmId,
                CharacterId = player.CharacterId,
                Rank = 0,
                Name = player.Name,
                Sex = player.Sex,
                Class = player.Class,
                Path = player.Path,
                Level = player.Level
            };
            Members.Add(Leader.CharacterId, Leader);
            OnlineMembers.Add(Leader.CharacterId, leaderSession);
        }

        public GuildData BuildServerGuildData()
        {
            return new GuildData
            {
                GuildId = Id,
                GuildName = Name,
                Taxes = Taxes,
                Type = Type,
                Ranks = GetRanks().ToList(),
                GuildStandard = GuildStandard,
                TotalMembers = (uint)Members.Count,
                UsersOnline = (uint)OnlineMembers.Count,
                Unknown6 = 1,
                Unknown7 = 1,
                GuildInfo =
                {
                    AgeInDays = (float)DateTime.Now.Subtract(CreateTime).TotalHours * -1
                }
            };
        }
    }
}
