using System;
using System.Collections.Generic;
using ClassLibrary;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ClientForm
{
    public partial class Form1 : Form
    {
        private bool _connectState = false;
        private delegate void AppendStringDelegate(string str);
        private Thread thread;
        private Thread thread1;
        private bool _started;
        private string nickname = "";

        public Form1()
        {
            InitializeComponent();
            thread1 = new Thread(new ThreadStart(form));
            thread1.Start();
            _started = false;

        }

        //Handles connecting to the server
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox2.Text.CompareTo("") == 1)
            {
                switch (_connectState)
                {
                    case true:
                        SendConnectionMessage(false);
                        button2.Enabled = false;
                        thread.Abort();
                        Connection.Disconnect();
                        _connectState = false;
                        button1.Text = "Connect";
                        break;

                    case false:
                        nickname = textBox2.Text;
                        textBox2.Enabled = false;
                        button2.Enabled = true;
                        Connection.Connect();
                        SendNickName(nickname);
                        SendConnectionMessage(true);
                        _connectState = true;
                        button1.Text = "Disconnect";
                        thread = new Thread(new ThreadStart(Recieve));
                        thread.Start();
                        break;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string msg = nickname + ": " + textBox1.Text;
            ChatMessagepacket chatMessage = new ChatMessagepacket(msg);

            Send(chatMessage);

            textBox1.Text = "";
        }

        public void Send(Packet data)
        {
            MemoryStream mem = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();

            bf.Serialize(mem, data);
            byte[] buffer = mem.GetBuffer();

            Connection._writer.Write(buffer.Length);
            Connection._writer.Write(buffer);
            Connection._writer.Flush();
        }

        private void SendConnectionMessage(bool value)
        {
            ConnectionMessagePacket con = new ConnectionMessagePacket(value);
            Send(con);

        }
        private void SendNickName(string nickname)
        {
            NicknamePacket name = new NicknamePacket(nickname);
            Send(name);
        }

        public void Recieve()
        {
            while (_connectState == true)
            {
                int incomingbytes;
                try
                {

                    while ((incomingbytes = Connection.reader.ReadInt32()) != 0)
                    {
                        Console.WriteLine("Recevied");

                        byte[] bytes = Connection.reader.ReadBytes(incomingbytes);

                        MemoryStream mem = new MemoryStream(bytes);
                        BinaryFormatter bf = new BinaryFormatter();
                        Packet packet = bf.Deserialize(mem) as Packet;

                        switch (packet.type)
                        {
                            case PacketType.ChatMessage:
                                string message = ((ChatMessagepacket)packet).message;
                                richTextBox1.Invoke(new AppendStringDelegate(AppendString), new object[] { message });

                                break;

                        }
                    }
                }
                catch (EndOfStreamException)
                {

                }
            }
        }

        private void AppendString(string str)
        {
            richTextBox1.Text += str + "\n";
        }

        private void form()
        {
            
            while (true)
            {
                if (_started)
                {
                    Application.Run(new Form2());
                    _started = false;
                }
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            _started = true;
        }
    }
}
