using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    [Serializable]
    public class WordPacket : Packet
    {
        public string word = String.Empty;

        public WordPacket(string Message)
        {
            this.type = PacketType.WordPacket;
            this.word = Message;
        }
    }
}
