using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientGuildRegister, MessageDirection.Client)]
    public class ClientGuildRegister : IReadable
    {
        

        public void Read(GamePacketReader reader)
        {
            
        }
    }
}
