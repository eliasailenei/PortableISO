using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO.Compression;
using System.Net;
using CustomConfig;
namespace contin
{
    public partial class Driver : UserControl
    {
        DriveLetters drive;
        public event EventHandler InteractionComplete;
        public Driver(DriveLetters drives)
        {
            drive = drives;
            InitializeComponent();
        }

        private void UserControl1_Load(object sender, EventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Directory.CreateDirectory(drive.TLetter.ToString() + ":\\contin\\PortableDriver");
            using (var client = new WebClient())
            {
                client.DownloadFile("https://github.com/eliasailenei/PortableDriver/releases/download/main/Release.zip", drive.TLetter.ToString() + ":\\contin\\Driver.zip");
            }
            ZipFile.ExtractToDirectory(drive.TLetter.ToString() + ":\\contin\\Driver.zip", drive.TLetter.ToString() + ":\\PortableDriver\\");
            File.Delete(drive.TLetter.ToString() + ":\\contin\\Driver.zip");
            Process pro = new Process();
            pro.StartInfo.FileName = drive.TLetter.ToString() + ":\\contin\\PortableDriver\\PortableDriver.exe";
            pro.StartInfo.Arguments = "--test";
            pro.Start();
            pro.WaitForExit();
            InteractionComplete.Invoke(this, EventArgs.Empty);
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            InteractionComplete.Invoke(this, EventArgs.Empty);
            this.Hide();
        }
    }
}
