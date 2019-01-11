﻿using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.Server0981, MessageDirection.Server)]
    public class Server0981 : IWritable
    {
        public byte[] AccountItemList { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(AccountItemList.Length / 4);
            writer.WriteBytes(AccountItemList);
        }
    }
}
