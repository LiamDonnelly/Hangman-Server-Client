using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    [Serializable]
    public class ConnectionMessagePacket : Packet
    {
        public bool connected = true;

        public ConnectionMessagePacket(bool Connected)
        {
            this.type = PacketType.Connection;
            this.connected = Connected;
        }
    }
}
