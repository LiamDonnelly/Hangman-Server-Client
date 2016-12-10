using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;

namespace ClientForm
{
    class Connection
    {
        private const string hostname = "127.0.0.1";
        private const int port = 4444;

        public static TcpClient _tcpClient;
        public static NetworkStream _stream;
        public static BinaryWriter _writer;
        public  static BinaryReader reader;
        public static bool Connect()
        {
            try
            {
                _tcpClient = new TcpClient();
                _tcpClient.Connect(hostname, port);
                _stream = _tcpClient.GetStream();
                _writer = new BinaryWriter(Connection._stream, Encoding.UTF8);
                reader = new BinaryReader(Connection._stream, Encoding.UTF8);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);

                return false;
            }

            return true;
        }
        public static bool Disconnect()
        {
            try
            {
                _tcpClient.Close();
                
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);

                return false;
            }

            return true;
        }
    }
}
