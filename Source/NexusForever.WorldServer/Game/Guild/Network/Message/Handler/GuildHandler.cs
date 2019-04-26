﻿using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model;
using NexusForever.WorldServer.Network.Message.Model.Shared;
using NLog;
using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class GuildHandler
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        [MessageHandler(GameMessageOpcode.ClientGuildRegister)]
        public static void HandleGuildRegister(WorldSession session, ClientGuildRegister request)
        {
            log.Info($"{request.UnitId}, {request.GuildType}, {request.GuildName}, {request.MasterTitle}, {request.CouncilTitle}, {request.MemberTitle}, {request.GuildHolomark.BackgroundIcon.GuildStandardPartId}, {request.GuildHolomark.ForegroundIcon.GuildStandardPartId}, {request.GuildHolomark.ScanLines.GuildStandardPartId}, {request.Unknown0}");

            session.EnqueueMessageEncrypted(new ServerGuildJoin
            {
                Guild = new Guild
                {
                    GuildData = new GuildData
                    {
                        GuildId = 1,
                        GuildName = request.GuildName,
                        Unknown0 = 0,
                        Type = request.GuildType,
                        Ranks = new List<GuildData.Rank>
                        {
                            new GuildData.Rank
                            {
                                RankName = request.MasterTitle,
                                PermissionMask = -2,
                                Unknown2 = -1,
                                Unknown3 = -1,
                                Unknown4 = -1
                            },
                            new GuildData.Rank
                            {
                                RankName = request.CouncilTitle,
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
                                RankName = request.MemberTitle,
                                PermissionMask = 2048,
                                Unknown2 = 0,
                                Unknown3 = 0,
                                Unknown4 = 0
                            }
                        },
                        Holomark = request.GuildHolomark,
                        TotalMembers = 1,
                        UsersOnline = 1,
                        CurrentInfluence = 500,
                        DailyBonusRemaining = 200000,
                        Unknown5 = 0,
                        Unknown6 = 1,
                        Unknown7 = 1,
                        BankTabNames = new List<string>
                        {
                            "","","","","","","","","",""
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
                            AgeInDays = -0.00070f //-1318682848,
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
                GuildName = request.GuildName,
                Result = Game.Guild.Static.GuildResult.YouCreated
            });

            session.EnqueueMessageEncrypted(new ServerGuildMembers
            {
                GuildRealm = 1,
                GuildId = 1,
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

            session.EnqueueMessageEncrypted(new ServerGuildNameplateChange
            {
                RealmId = 1,
                GuildId = 1
            });

            session.Player.EnqueueToVisible(new ServerGuildNameplateChangeUnit
            {
                UnitId = session.Player.Guid,
                GuildName = request.GuildName,
                GuildType = request.GuildType
            }, true);


            // Do guild holomark visual
            var itemVisualUpdate = new ServerItemVisualUpdate
            {
                Guid = session.Player.Guid
            };

            foreach (ItemVisual itemVisual in session.Player.GetAppearance())
                itemVisualUpdate.ItemVisuals.Add(itemVisual);

            itemVisualUpdate.ItemVisuals.Add(new ItemVisual
            {
                Slot = Game.Entity.Static.ItemSlot.GuildStandardScanLines,
                DisplayId = 2961 // request.GuildHolomark.HolomarkPart1.GuildStandardPartId
            });
            itemVisualUpdate.ItemVisuals.Add(new ItemVisual
            {
                Slot = Game.Entity.Static.ItemSlot.GuildStandardBackgroundIcon,
                DisplayId = 5506 // request.GuildHolomark.HolomarkPart1.GuildStandardPartId
            });
            itemVisualUpdate.ItemVisuals.Add(new ItemVisual
            {
                Slot = Game.Entity.Static.ItemSlot.GuildStandardForegroundIcon,
                DisplayId = 5432 // request.GuildHolomark.HolomarkPart1.GuildStandardPartId
            });
            itemVisualUpdate.ItemVisuals.Add(new ItemVisual
            {
                Slot = Game.Entity.Static.ItemSlot.GuildStandardChest,
                DisplayId = 5411 // request.GuildHolomark.HolomarkPart1.GuildStandardPartId
            });
            itemVisualUpdate.ItemVisuals.Add(new ItemVisual
            {
                Slot = Game.Entity.Static.ItemSlot.GuildStandardBack,
                DisplayId = 7163
            });
            itemVisualUpdate.ItemVisuals.Add(new ItemVisual
            {
                Slot = Game.Entity.Static.ItemSlot.GuildStandardShoulderL,
                DisplayId = 7164
            });
            itemVisualUpdate.ItemVisuals.Add(new ItemVisual
            {
                Slot = Game.Entity.Static.ItemSlot.GuildStandardShoulderR,
                DisplayId = 7165
            });
            // 7163 - Near Back
            // 7164 - Near Left
            // 7165 - Near Right
            // 5580 - Far Back
            // 5581 - Far Left
            // 5582 - Far Right

            if (!session.Player.IsLoading)
                session.Player.EnqueueToVisible(itemVisualUpdate, true);
        }

        [MessageHandler(GameMessageOpcode.ClientGuildHolomarkUpdate)]
        public static void HandleHolomarkUpdate(WorldSession session, ClientGuildHolomarkUpdate clientGuildHolomarkUpdate)
        {
            log.Info($"{clientGuildHolomarkUpdate.Unknown0}, {clientGuildHolomarkUpdate.Unknown1}");
        }

        [MessageHandler(GameMessageOpcode.ClientGuildOperation)]
        public static void HandleOperation(WorldSession session, ClientGuildOperation clientGuildOperation)
        {
            log.Info($"{clientGuildOperation.Guid}, {clientGuildOperation.Id}, {clientGuildOperation.Text}, {clientGuildOperation.Operation}");

            if(clientGuildOperation.Operation == Game.Guild.Static.GuildOperation.InviteMember)
            {
                WorldSession targetSession = Shared.Network.NetworkManager<WorldSession>.GetSession(s => s.Player?.Name == clientGuildOperation.Text);
                targetSession?.EnqueueMessageEncrypted(new ServerGuildInvite
                {
                    PlayerName = "PlayerName",
                    GuildName = "GuildName",
                    Taxes = 0,
                    GuildType = Game.Guild.Static.GuildType.Guild,
                    Unknown4 = 1
                });
            }
        }
    }
}
