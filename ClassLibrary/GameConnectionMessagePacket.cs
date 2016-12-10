using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    [Serializable]
    public class GameConnectionMessagePacket  : Packet
    {
        public bool connected = true;
        public int playerNumber = 0;
        public GameConnectionMessagePacket(bool Connected)
        {
            this.type = PacketType.GameConnect;
            this.connected = Connected;
        }

        public GameConnectionMessagePacket(int playerNumber)
        {
            this.type = PacketType.GameConnect;
            this.playerNumber = playerNumber;
        }
    }
}
