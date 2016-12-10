using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    [Serializable]
    public class ChatMessagepacket : Packet
    {
        public string message = String.Empty;

        public ChatMessagepacket(string Message)
        {
            this.type = PacketType.ChatMessage;
            this.message = Message;
        }
    }
}
