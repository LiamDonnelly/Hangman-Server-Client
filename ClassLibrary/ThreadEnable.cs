using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    [Serializable]
    public class ThreadEnable : Packet
    {
        public bool connected = false;

        public ThreadEnable(bool Connected)
        {
            this.type = PacketType.ThreadEnable;
            this.connected = Connected;
        }
    }
}
