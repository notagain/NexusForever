using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model;
using NexusForever.WorldServer.Network.Message.Model.Shared;
using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class MiscHandler
    {
        [MessageHandler(GameMessageOpcode.ClientPing)]
        public static void HandlePing(WorldSession session, ClientPing ping)
        {
            session.Heartbeat.OnHeartbeat();
        }

        /// <summary>
        /// Handled responses to Player Info Requests.
        /// TODO: Put this in the right place, this is used by Mail & Contacts, at minimum. Probably used by Guilds, Circles, etc. too.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="request"></param>
        [MessageHandler(GameMessageOpcode.ClientPlayerInfoRequest)]
        public static void HandlePlayerInfoRequest(WorldSession session, ClientPlayerInfoRequest request)
        {
            Dictionary<byte, ContactType> contactIndexToContactsTypeMap = new Dictionary<byte, ContactType>
            {
                { 1, ContactType.Ignore },
                { 2, ContactType.Friend },
                { 3, ContactType.Rival }
            };
            Character character = CharacterDatabase.GetCharacterById(request.CharacterId);
            if (character != null)
                ContactManager.HandlePlayerInfoResponse(session, character, contactIndexToContactsTypeMap[request.Unknown0]); // TODO: Put this in the right place
            else
                throw new ArgumentException();
        }

        [MessageHandler(GameMessageOpcode.ClientGuildRegister)]
        public static void HandleGuildRegister(WorldSession session, ClientGuildRegister request)
        {
            session.EnqueueMessageEncrypted(new ServerGuildJoin
            {
                Guild = new Guild
                {
                    GuildData = new GuildData
                    {
                        GuildId = 436283028,
                        GuildName = "Kirmmin Klub",
                        Unknown0 = 0,
                        Type = 1,
                        Ranks = new List<GuildData.Rank>
                        {
                            new GuildData.Rank
                            {
                                RankName = "Leader",
                                PermissionMask = -2,
                                Unknown2 = -1,
                                Unknown3 = -1,
                                Unknown4 = -1
                            },
                            new GuildData.Rank
                            {
                                RankName = "Council",
                                PermissionMask = 7104,
                                Unknown2 = -1,
                                Unknown3 = -1,
                                Unknown4 = -1
                            },
                            new GuildData.Rank
                            {
                                RankName = "",
                                PermissionMask = 1,
                                Unknown2 = 0,
                                Unknown3 = 0,
                                Unknown4 = 0
                            },
                            new GuildData.Rank
                            {
                                RankName = "",
                                PermissionMask = 1,
                                Unknown2 = 0,
                                Unknown3 = 0,
                                Unknown4 = 0
                            },
                            new GuildData.Rank
                            {
                                RankName = "",
                                PermissionMask = 1,
                                Unknown2 = 0,
                                Unknown3 = 0,
                                Unknown4 = 0
                            },
                            new GuildData.Rank
                            {
                                RankName = "",
                                PermissionMask = 1,
                                Unknown2 = 0,
                                Unknown3 = 0,
                                Unknown4 = 0
                            },
                            new GuildData.Rank
                            {
                                RankName = "",
                                PermissionMask = 1,
                                Unknown2 = 0,
                                Unknown3 = 0,
                                Unknown4 = 0
                            },
                            new GuildData.Rank
                            {
                                RankName = "",
                                PermissionMask = 1,
                                Unknown2 = 0,
                                Unknown3 = 0,
                                Unknown4 = 0
                            },
                            new GuildData.Rank
                            {
                                RankName = "",
                                PermissionMask = 1,
                                Unknown2 = 0,
                                Unknown3 = 0,
                                Unknown4 = 0
                            },
                            new GuildData.Rank
                            {
                                RankName = "Member",
                                PermissionMask = 2048,
                                Unknown2 = 0,
                                Unknown3 = 0,
                                Unknown4 = 0
                            }
                        },
                        Holomark = new GuildHolomark
                        {
                            HolomarkPart1 = new GuildHolomark.HolomarkPart
                            {
                                GuildStandardPartId = 7,
                                DyeColorRampId1 = 0,
                                DyeColorRampId2 = 0,
                                DyeColorRampId3 = 0
                            },
                            HolomarkPart2 = new GuildHolomark.HolomarkPart
                            {
                                GuildStandardPartId = 5,
                                DyeColorRampId1 = 0,
                                DyeColorRampId2 = 0,
                                DyeColorRampId3 = 0
                            },
                            HolomarkPart3 = new GuildHolomark.HolomarkPart
                            {
                                GuildStandardPartId = 6,
                                DyeColorRampId1 = 0,
                                DyeColorRampId2 = 0,
                                DyeColorRampId3 = 0
                            }
                        },
                        TotalMembers = 1,
                        UsersOnline = 1,
                        CurrentInfluence = 500,
                        DailyBonusRemaining = 200000,
                        Unknown5 = 0,
                        Unknown6 = 0,
                        Unknown7 = 5,
                        BankTabNames = new List<string>
                        {
                            "Testing","","","","","","","","",""
                        },
                        Unknown11 = new GuildData.Info
                        {
                            MessageOfTheDay = "Message1",
                            AdditionalInfo = "Message2",
                            Unknown2 = 1,
                            Unknown3 = 0,
                            Unknown4 = 0,
                            Unknown5 = 1,
                            Unknown6 = 0,
                            Unknown7 = 0,
                            Unknown8 = 0,
                            Age = -1318682848f,
                        }
                    },
                    GuildLeader = new GuildMember
                    {
                        Realm = 1,
                        CharacterId = 1,
                        Rank = 0,
                        Name = "Kirmmin Gutwacker",
                        Unknown4 = 1,
                        Class = Game.Entity.Static.Class.Warrior,
                        Path = Game.Entity.Static.Path.Scientist,
                        Level = 24,
                        Unknown5 = 0,
                        Unknown6 = 0,
                        Unknown7 = 0,
                        Unknown8 = 0,
                        Note = "",
                        Unknown10 = 1,
                        LastOnline = -1f
                    },
                    GuildUnknown = new GuildUnknown()
                },
                Unknown0 = false
            });

            session.EnqueueMessageEncrypted(new ServerGuildResult
            {
                Realm = WorldServer.RealmId,
                CharacterId = session.Player.CharacterId,
                Unknown0 = 0,
                GuildName = "Kirmmin Klub",
                Result = 61
            });

            session.EnqueueMessageEncrypted(new ServerGuildMembers
            {
                GuildRealm = 1,
                GuildId = 436283028,
                GuildMembers = new List<Model.Shared.GuildMember>
                {
                    new Model.Shared.GuildMember
                    {
                        Realm = 1,
                        CharacterId = 1,
                        Rank = 0,
                        Name = "Kirmmin Gutwacker",
                        Unknown4 = 2,
                        Class = Game.Entity.Static.Class.Warrior,
                        Path = Game.Entity.Static.Path.Scientist,
                        Level = 24,
                        Unknown5 = 0,
                        Unknown6 = 0,
                        Unknown7 = 0,
                        Unknown8 = 0,
                        Note = "",
                        Unknown10 = 1,
                        LastOnline = -1f
                    }
                },
                Unknown0 = true
            });
        }
    }
}
