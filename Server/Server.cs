using System;
using System.Collections.Generic;
using ClassLibrary;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace Server
{
    class Server
    {
        TcpListener _tcpListener;
        private static int count = 0;
        private static string _wordGuess;
        private static string[] _words;
        private static int _turn = 0;
        private static int _playerNumber = 1;
        static List<Client> clientList = new List<Client>();
        static List<Client> hangmanList = new List<Client>();

        public Server(string _ip, int _port)
        {
            IPAddress ip = IPAddress.Parse(_ip);
            _tcpListener = new TcpListener(ip, _port);
            _words = new string[] { "tables", "computer", "laptop", "drinks", "train" };
            SetWord();
        }

        public void Start()
        {
            _tcpListener.Start();

            Console.WriteLine("Online...");

            while (true)
            {
                Socket socket = _tcpListener.AcceptSocket();
                Client client = new Client(socket);

                clientList.Add(client);

                Console.WriteLine("Connection Made");

                client.Start();
            }
        }

        public void Stop()
        {
            _tcpListener.Stop();
            foreach (Client a in clientList)
            {
                a.Stop();
            }
        }

        public static void SocketMethod(Client c)
        {
            Socket socket = c.Socket;
            NetworkStream stream = c.stream;
            BinaryReader reader = c.reader;
            BinaryWriter writer = c.writer;
            int incomingbytes;

            try
            {
                while ((incomingbytes = reader.ReadInt32()) != 0)
                {
                    byte[] bytes = reader.ReadBytes(incomingbytes);

                    MemoryStream mem = new MemoryStream(bytes);

                    BinaryFormatter bf = new BinaryFormatter();

                    Packet packet = bf.Deserialize(mem) as Packet;
                    PacketCase(packet, c);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void PacketCase(Packet packet, Client c)
        {

            switch (packet.type)
            {
                case PacketType.ChatMessage:
                    string message = ((ChatMessagepacket)packet).message;
                    Console.WriteLine(message);
                    ChatMessagepacket chat = new ChatMessagepacket(message);
                    foreach (Client cl in clientList)
                    {
                        Send(chat, cl);
                    }
                    break;

                case PacketType.Nickname:
                    c.Nickname = ((NicknamePacket)packet).nickname;
                    break;

                case PacketType.Connection:
                    if (((ConnectionMessagePacket)packet).connected == true)
                    {
                        Console.WriteLine(c.Nickname + ": Connected");

                        message = c.Nickname + ": Connected";
                        chat = new ChatMessagepacket(message);
                        foreach (Client cl in clientList)
                        {
                            Send(chat, cl);
                        }
                    }

                    if (((ConnectionMessagePacket)packet).connected == false)
                    {
                        Console.WriteLine(c.Nickname + ": Disconnected");
                        message = c.Nickname + ": Disconnected";
                        chat = new ChatMessagepacket(message);
                        foreach (Client cl in clientList)
                        {
                            Send(chat, cl);
                        }
                    }
                    break;

                case PacketType.GameConnect:
                    hangmanList.Add(c);
                    int pos = clientList.FindIndex(a => a.Nickname == null);
                    clientList.RemoveAt(pos);
                    
                    GameConnectionMessagePacket playnum = new GameConnectionMessagePacket(_playerNumber);
                    Send(playnum, c);
                    _playerNumber++;
                    
                    if (hangmanList.Count == 2)
                    {
                        foreach (Client hm in hangmanList)
                        {
                            WordPacket word = new WordPacket(_wordGuess);
                            Send(word, hm);

                        }
                    }
                    break;

                case PacketType.ThreadEnable:
                    TurnActivator();
                    break;

                case PacketType.Win:
                    Console.WriteLine(((WinPacket)packet).message);
                    
                    if (((WinPacket)packet).playernum == 1)
                    {
                        WinPacket win = new WinPacket("You lose", 2);
                        Send(win, hangmanList[1]);
                    }
                    if (((WinPacket)packet).playernum == 2)
                    {
                        WinPacket win = new WinPacket("You lose", 1);
                        Send(win, hangmanList[0]);
                    }
                    hangmanList.Clear();
                    SetWord();
                    break;

            }
        }

        private static void Send(Packet data, Client c)
        {
            if (c.Socket.Connected == true)
            {
                try
                {

                    BinaryWriter _writer = c.writer;
                    MemoryStream mem = new MemoryStream();
                    BinaryFormatter bf = new BinaryFormatter();

                    bf.Serialize(mem, data);
                    byte[] buffer = mem.GetBuffer();

                    _writer.Write(buffer.Length);
                    _writer.Write(buffer);
                    _writer.Flush();
                    count++;

                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                }
            }
            else
            {
                c.Stop();
                clientList.RemoveAt(count);

            }
        }

        private static void SetWord()
        {
            Random ran = new Random();
            int num = ran.Next(5);
            _wordGuess = _words[num];
        }

        private static void TurnActivator()
        {
            ThreadEnable _lock = new ThreadEnable(true);
            ThreadEnable _unlock = new ThreadEnable(false);
            switch (_turn)
            {
                case 0:
                    
                    Send(_lock, hangmanList[0]);
                    Send(_unlock, hangmanList[1]);
                    break;

                case 1:
                    
                    Send(_lock, hangmanList[1]);
                    Send(_unlock, hangmanList[0]);
                    break;
            }
            TurnSwitch();
        }

        private static void TurnSwitch()
        {
            int temp = _turn;

            if (temp == 0)
            {
                _turn = 1;
            }

            if (temp == 1)
            {
                _turn = 0;
            }
        }
    }
}


