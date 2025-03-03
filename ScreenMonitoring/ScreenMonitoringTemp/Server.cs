using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace ScreenMonitoringTemp
{
    public partial class Server : Form
    {
        public Server()
        {
            InitializeComponent();
        }

        private void Server_Load(object sender, EventArgs e)
        {
            new Thread(() => {
                Thread.CurrentThread.IsBackground = true;

                TcpListener listener = new TcpListener(IPAddress.Any, 8080);
                listener.Start();
                try
                {
                    TcpClient client = listener.AcceptTcpClient();
                    NetworkStream n = client.GetStream();
                    BinaryReader reader = new BinaryReader(n);
                    byte[] bytes = new byte[1];
                    int size = 0;

                    while (true)
                    {
                        size = reader.ReadInt32();
                        Array.Resize<byte>(ref bytes, size);
                        bytes = reader.ReadBytes(size);
                        Bitmap bitmap = binTobit(bytes);
                        pictureBox1.Image = bitmap;
                    }
                } catch (Exception) { this.Close(); }
            }).Start();
        }

        private Bitmap binTobit(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                Bitmap bitmap = new Bitmap(ms);
                return bitmap;
            }
        }
    }
}
