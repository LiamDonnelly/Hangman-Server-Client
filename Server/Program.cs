using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Server _server = new Server("127.0.0.1", 4444);

            _server.Start();

            _server.Stop();
        }
    }
}
