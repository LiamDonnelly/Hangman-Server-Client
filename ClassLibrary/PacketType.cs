using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public enum PacketType
    {
        Empty,
        Nickname,
        ChatMessage,
        Connection,
        GameConnect,
        WordPacket,
        ThreadEnable,
        Win
    }
}
