using NexusForever.WorldServer.Game.Guild.Static;
using NexusForever.WorldServer.Network.Message.Model.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.WorldServer.Game.Guild
{
    public class Rank
    {
        public string Name { get; private set; }
        public GuildRankPermission GuildPermission { get; private set; }
        public ulong BankWithdrawalPermissions { get; set; }
        public long MoneyWithdrawalLimit { get; private set; }
        public long RepairLimit { get; private set; }
        public byte Index { get; private set; }

        public Rank(string name, byte index, GuildRankPermission guildRankPermission, ulong bankWithdrawalPermissions, long moneyWithdrawalLimit, long repairLimit)
        {
            Name = name;
            Index = index;
            GuildPermission = guildRankPermission;
            BankWithdrawalPermissions = bankWithdrawalPermissions;
            MoneyWithdrawalLimit = moneyWithdrawalLimit;
            RepairLimit = repairLimit;
        }

        public void ChangeName(string name)
        {
            Name = name;
        }

        public void AddPermission(GuildRankPermission guildRankPermission)
        {
            GuildPermission |= guildRankPermission;
        }

        public void RemovePermission(GuildRankPermission guildRankPermission)
        {
            if((GuildPermission & guildRankPermission) == 0)
            {
                GuildPermission |= guildRankPermission;
            }
        }

        public GuildData.Rank GetGuildDataRank()
        {
            return new GuildData.Rank
            {
                RankName = Name,
                PermissionMask = GuildPermission,
                BankWithdrawalPermissions = BankWithdrawalPermissions,
                MoneyWithdrawalLimit = MoneyWithdrawalLimit,
                RepairLimit = RepairLimit
            };
        }

    }
}
