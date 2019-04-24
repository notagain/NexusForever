﻿using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Network.Message.Model
{

    [Message(GameMessageOpcode.ServerGuildRoster)]
    public class ServerGuildMembers : IWritable
    {
        public ushort GuildRealm { get; set; }
        public ulong GuildId { get; set; }
        public List<GuildMember> GuildMembers { get; set; } = new List<GuildMember>();
        public bool Unknown0 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(GuildRealm, 14u);
            writer.Write(GuildId);
            writer.Write(GuildMembers.Count, 32u);
            GuildMembers.ForEach(w => w.Write(writer));
            writer.Write(Unknown0);
        }
    }
}
