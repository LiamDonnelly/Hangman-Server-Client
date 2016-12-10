using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    [Serializable]
    public class WinPacket : Packet
    {
        public string message = String.Empty;
        public int playernum = 0;
        public WinPacket(string Message, int player)
        {
            this.type = PacketType.Win;
            this.message = Message;
            this.playernum = player;
        }
    }
}
