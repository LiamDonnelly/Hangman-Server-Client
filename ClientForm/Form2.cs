using ClassLibrary;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ClientForm
{
    public partial class Form2 : Form
    {
        private paint p;
        private game g;
        private Thread thread2;
        private Thread thread3;
        private Panel panel;
        private string _word;
        private bool locked;
        private int _playerNumber = 0;
        private bool gameStart, win;
        ThreadEnable swap = new ThreadEnable(true);
        private delegate void StartGame();
        private StringBuilder s;



        public Form2()
        {

            InitializeConnection();
            InitializePaint();
            InitializeComponent();
            p = new paint(panel);

            thread2 = new Thread(new ThreadStart(p.Start));
            thread2.Start();
            thread3 = new Thread(new ThreadStart(Recieve));
            thread3.Start();
            locked = true;
            gameStart = false;
            win = false;

        }

        private void InitializeConnection()
        {
            Connection.Connect();
            while (Connection._tcpClient.Connected == false)
            {
                Thread.Sleep(1000);
                Connection.Connect();
            }

            GameConnectionMessagePacket con = new GameConnectionMessagePacket(true);
            Send(con);
        }

        private void InitializePaint()
        {
            this.Size = new System.Drawing.Size(500, 500);
            this.BackColor = Color.White;

            this.panel = new Panel();
            this.panel.Location = new Point(50, 80);
            this.panel.Size = new Size(300, 300);
            this.panel.BackColor = Color.LightGray;

            this.Controls.Add(panel);

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

        public void Recieve()
        {
            while (true)
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
                            case PacketType.WordPacket:
                                _word = ((WordPacket)packet).word;
                                g = new game(_word);

                                if (gameStart == true)
                                    button4.Invoke(new StartGame(EnableLetters));
                                break;

                            case PacketType.ThreadEnable:
                                if (gameStart == false)
                                {
                                    button4.Invoke(new StartGame(EnableLetters));
                                    gameStart = true;
                                }

                                locked = ((ThreadEnable)packet).connected;

                                if (locked == false)
                                {
                                    lock (this)
                                    {
                                        Monitor.Pulse(this);
                                    }
                                }

                                break;

                            case PacketType.GameConnect:
                                _playerNumber = ((GameConnectionMessagePacket)packet).playerNumber;
                                if (_playerNumber == 1)
                                {
                                    gameStart = true;
                                }

                                break;

                            case PacketType.Win:
                                lock (this)
                                {
                                    Monitor.Pulse(this);
                                }
                                MessageBox.Show("You lose!");
                                win = true;
                                button4.Invoke(new StartGame(EnableLetters));
                                break;

                        }
                    }
                }
                catch (EndOfStreamException)
                {

                }
            }
        }

        private void EnableLetters()
        {
            if (win)
            {
                this.Close();
            }
            int size = g.GetWordSize();
            string st = "";
            for (int i = 0; i < size; i++)
            {
                st += "#";
            }
            s = new StringBuilder(st);

            textBox1.Text = s.ToString();

            button4.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
            button7.Enabled = true;
            button8.Enabled = true;
            button9.Enabled = true;
            button10.Enabled = true;
            button11.Enabled = true;
            button12.Enabled = true;
            button13.Enabled = true;
            button14.Enabled = true;
            button15.Enabled = true;
            button16.Enabled = true;
            button17.Enabled = true;
            button18.Enabled = true;
            button19.Enabled = true;
            button20.Enabled = true;
            button21.Enabled = true;
            button22.Enabled = true;
            button23.Enabled = true;
            button24.Enabled = true;
            button25.Enabled = true;
            button26.Enabled = true;
            button27.Enabled = true;
            button28.Enabled = true;
            button29.Enabled = true;
        }

        private void DisableLetters(object sender, EventArgs e)
        {
            if (g.CheckGuess(sender.ToString()) == true)
            {

                int a = g.GetGuessIndex();
                s[a] = g.GetChar();
                textBox1.Text = s.ToString();

                if (g.CheckWin() == true)
                {
                    WinPacket win = new WinPacket(_playerNumber + " wins", _playerNumber);
                    Send(win);
                    MessageBox.Show("You win");
                    this.Close();
                }
                MessageBox.Show(sender.ToString() + " is correct!");
            }
            else
            {
                p.DrawNext();
            }

            ((Button)sender).Enabled = false;
            Send(swap);
            Thread.Sleep(100);
            lock (this)
            {
                Monitor.Wait(this);
            }
        }
    }

    public class paint
    {
        int count = 0;
        int count2 = 0;
        private Panel panel;
        public paint(Panel _panel)
        {
            this.panel = _panel;
            this.panel.Paint += new PaintEventHandler(this.panel_draw);
        }
        public void Start()
        {
            while (true)
            {
                if (count == count2)
                {
                    //Pad
                }
                else
                {
                    panel.Invalidate();
                    count2++;
                }
            }
        }

        public void panel_draw(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            SolidBrush brushBlack = new SolidBrush(Color.Black);
            SolidBrush brushBrown = new SolidBrush(Color.Brown);
            Point[] points = new Point[4];

            if (count == 1 || count > 0)
            {
                //Base
                g.FillRectangle(brushBlack, 10, 260, 100, 10);

                if (count == 2 || count > 1)
                {
                    //Pole
                    g.FillRectangle(brushBlack, 50, 60, 10, 200);

                    if (count == 3 || count > 2)
                    {
                        //top
                        g.FillRectangle(brushBlack, 60, 60, 100, 10);
                        if (count == 4 || count > 3)
                        {
                            //rope
                            g.FillRectangle(brushBrown, 120, 70, 10, 40);
                            if (count == 5 || count > 4)
                            {
                                //head
                                g.FillEllipse(brushBlack, 110, 105, 30, 30);
                                if (count == 6 || count > 5)
                                {
                                    //body
                                    g.FillRectangle(brushBlack, 123, 130, 5, 70);
                                    if (count == 7 || count > 6)
                                    {
                                        //left arm
                                        points = new Point[] { new Point { X = 123, Y = 150 }, new Point { X = 90, Y = 140 }, new Point { X = 90, Y = 145 }, new Point { X = 123, Y = 155 } };
                                        g.FillPolygon(brushBlack, points);
                                        if (count == 8 || count > 7)
                                        {
                                            //right arm2
                                            points = new Point[] { new Point { X = 128, Y = 150 }, new Point { X = 161, Y = 140 }, new Point { X = 161, Y = 145 }, new Point { X = 128, Y = 155 } };
                                            g.FillPolygon(brushBlack, points);
                                            if (count == 9 || count > 8)
                                            {
                                                //left leg
                                                points = new Point[] { new Point { X = 122, Y = 198 }, new Point { X = 100, Y = 220 }, new Point { X = 100, Y = 225 }, new Point { X = 125, Y = 200 } };
                                                g.FillPolygon(brushBlack, points);
                                                if (count == 10 || count > 9)
                                                {
                                                    //right leg
                                                    points = new Point[] { new Point { X = 129, Y = 198 }, new Point { X = 150, Y = 220 }, new Point { X = 150, Y = 225 }, new Point { X = 126, Y = 200 } };
                                                    g.FillPolygon(brushBlack, points);


                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            brushBlack.Dispose();
            brushBrown.Dispose();//  Dispose graphics resources. 
            g.Dispose();
        }

        public void DrawNext()
        {
            count++;
        }
    }

    public class game
    {
        private string _word;
        private int _wordSize;
        private int _count;
        private char _Char;

        public game(string word)
        {
            _word = word;
            SetWordSize();
        }

        public void SetWord(string word)
        {
            _word = word;
        }

        public string GetWord()
        {
            return _word;
        }

        public int GetWordSize()
        {
            return _wordSize;
        }

        private void SetWordSize()
        {
            _wordSize = _word.Length;
        }

        public bool CheckGuess(string guess)
        {
            string str = guess.ToLower();
            _Char = str[35];

            if (_word.Contains(_Char))
            {
                _count++;
                return true;
            }

            return false;
        }

        public bool CheckWin()
        {
            if (_wordSize == _count)
                return true;

            return false;
        }

        public int GetGuessIndex()
        {
            int a = _word.IndexOf(_Char);
            return a;
        }

        public char GetChar()
        {
            return _Char;
        }
    }
}
