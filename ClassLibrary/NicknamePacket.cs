using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{   
    [Serializable]
    public class NicknamePacket : Packet
    {
        public string nickname = String.Empty;

        public NicknamePacket(string Nickname)
        {
            this.type = PacketType.Nickname;
            this.nickname = Nickname;
        }
    }
}
