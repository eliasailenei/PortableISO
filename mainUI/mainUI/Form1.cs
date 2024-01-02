using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Diagnostics;
namespace mainUI
{
    public partial class Form1 : Form
    {
        private Size original;
        private Rectangle image1, maintext, subtext, credit, pleasewait, buttoncontinue, wifi, wifitext, dis, distext, shut, shuttext;
        private string winxshell = @"X:\Windows\System32\WinXShell";
        public Form1()
        {
            InitializeComponent();
            this.Resize += Form1_rsize;
            original = this.Size;
            image1 = new Rectangle(pictureBox1.Location, pictureBox1.Size);
            maintext = new Rectangle(label1.Location, label1.Size);
            subtext = new Rectangle(label2.Location, label2.Size);
            credit = new Rectangle(label3.Location, label3.Size);
            pleasewait = new Rectangle(label4.Location, label4.Size);
            buttoncontinue = new Rectangle(button1.Location, button1.Size);
            wifi = new Rectangle(button3.Location, button3.Size);
            wifitext = new Rectangle(label7.Location, label7.Size);
            dis = new Rectangle(button2.Location, button2.Size);
            distext = new Rectangle(label6.Location, label6.Size);
            shuttext = new Rectangle(label8.Location, label8.Size);
            shut = new Rectangle(button4.Location, button4.Size);

        }
        private void resize_Control(Control c, Rectangle r)
        {
            float xRatio = (float)(this.Width) / (float)(original.Width);
            float yRatio = (float)(this.Height) / (float)(original.Height);
            int newX = (int)(r.X * xRatio);
            int newY = (int)(r.Y * yRatio);

            int newWidth = (int)(r.Width * xRatio);
            int newHeight = (int)(r.Height * yRatio);

            c.Location = new Point(newX, newY);
            c.Size = new Size(newWidth, newHeight);

        }
        private void button1_Click(object sender, EventArgs e)
        {
           // this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (ready)
            {
                Confirm showDiag = new Confirm();
                int centerX = (Screen.PrimaryScreen.Bounds.Width - showDiag.Width) / 2;
                int centerY = (Screen.PrimaryScreen.Bounds.Height - showDiag.Height) / 2;
                showDiag.Location = new Point(centerX, centerY);
                this.Controls.Add(showDiag); 
                showDiag.BringToFront();     
                showDiag.Show();
                showDiag.InteractionComplete += (s, args) =>
                {
                    showDiag.Hide();
                    DiskSelect sel = new DiskSelect();
                    timer2.Start();
                    sel.ShowDialog();
                };
            }
            else
            {
                net();
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (Opacity == 0)
            {
                this.Hide();
            }
            Opacity -= .4;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Process.Start(@"X:\Windows\System32\WinXShell\WinXShell.exe", "-ui -jcfg wxsUI\\UI_Resolution.zip");
           
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Process.Start(@"X:\Windows\System32\WinXShell\WinXShell.exe", "-ui -jcfg wxsUI\\UI_WIFI.zip");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DialogResult iquit = MessageBox.Show("Do you want to terminate the program? This will shutdown your PC!", "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (iquit == DialogResult.Yes)
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = @"wpeutil.exe",
                    Arguments = $"shutdown",
                    UseShellExecute = false,
                    CreateNoWindow = true
                });
            }
            else
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = @"cmd.exe",
                    Arguments = $"",
                    //UseShellExecute = false,
                    //CreateNoWindow = true
                });
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void Form1_rsize(object sender, EventArgs e)
        {
            resize_Control(pictureBox1, image1);
            resize_Control(label1, maintext);
            resize_Control(label2, subtext);
            resize_Control(label3, credit);
            resize_Control(label4, pleasewait);
            resize_Control(button1, buttoncontinue);
            resize_Control(button2, dis);
            resize_Control(button3, wifi);
            resize_Control(button4, shut);
            resize_Control(label6, distext);
            resize_Control(label7, wifitext);
            resize_Control(label8, shuttext);
        }
        bool ready;
        private void Form1_Load(object sender, EventArgs e)
        {
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;

            net();

           
        }
        private void net()
        {
            bool isReachable = CheckInternetConnection();
           
            if (!isReachable)
            {
               
                
                label4.Text = "No network! Try Wi-Fi!";
                button1.Text = "Try again";
            }
            else
            {
                ready = true;
                button1.Text = ">>>>>>>>>>";
                label4.Text = "You may continue!";
            }
        }
        static bool CheckInternetConnection()
        {
            try
            {
                string host = "8.8.8.8"; 

                using (Ping ping = new Ping())
                {
                    PingReply reply = ping.Send(host);
                    if (reply.Status == IPStatus.Success)
                    {
                        return true; 
                    }
                }
            }
            catch
            {
                return false;
            }

            return false;
        }


        
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Opacity == 1)
            {
                timer1.Stop();
            }
            Opacity += .4;
        }
    }

}