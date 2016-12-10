using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using ClassLibrary;

namespace Server
{
    class Client
    {
        public Socket Socket;
        public string Nickname;
        private Thread thread;
        public NetworkStream stream;
        public BinaryReader reader;
        public BinaryWriter writer;


        public Client(Socket socket)
        {
            Socket = socket;

            stream = new NetworkStream(socket, true);
            reader = new BinaryReader(stream, Encoding.UTF8);
            writer = new BinaryWriter(stream, Encoding.UTF8);
        }

        public void Start()
        {
            thread = new Thread(new ThreadStart(SocketMethod));
            thread.Start();
        }
        public void Stop()
        {
            Socket.Close();
            
            if (thread.IsAlive == true)
            {
                thread.Abort();
            }
        }
        private void SocketMethod()
        {
            Server.SocketMethod(this);
        }
    }
}
