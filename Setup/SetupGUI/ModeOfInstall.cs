using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SetupGUI
{
    public partial class ModeOfInstall : Form
    {
        int noOfUSB;
        bool scanDone;
        public ModeOfInstall()
        {
            InitializeComponent();
        }

        private async void ModeOfInstall_Load(object sender, EventArgs e)
        {
            string content = "You have 4 options; it is recommended you choose USB if you have a USB drive.\n\nRemember that the image is not always 700MB and can be more than a 1GB depending on drivers and if its WinRE or WinPE.\n\nAllow at least 30 seconds for the drives to scan.";
            richTextBox1.AppendText(content);
            try
            {
                File.Delete("diskinfo.txt");
                File.Delete("letters.txt");
            }
            catch {
                Console.WriteLine("This isn't users first time");
            }
            
            button1.Enabled = false;
            await USBLoad();
        }

        private async Task USBLoad()
        {
            pictureBox1.Visible = true;
            scanDone = false;
            await getUSB();
            await FormatList();
            if (noOfUSB > 0)
            {
                button1.Enabled = true;
             }
            label10.Text = "Click here to rescan";
            pictureBox1.Visible = false;
            scanDone = true;
        }

        private async Task getUSB()
        {
            await Task.Run(() =>
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = "powershell";
                    process.StartInfo.Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"Get-Disk | Out-File -FilePath 'diskinfo.txt'\"";
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;

                    process.Start();
                    process.WaitForExit();
                }
            });
        }
        private async Task FormatList()
        {
            await Task.Run(() =>
            {
                Regex keepOnlyDrive = new Regex(@"^\d");
                Regex keepOnlyUSB = new Regex(@".*USB.*");

                using (StreamReader original = new StreamReader("diskinfo.txt"))
                {
                    string line;
                    while ((line = original.ReadLine()) != null)
                    {
                        if (keepOnlyDrive.IsMatch(line) && keepOnlyUSB.IsMatch(line))
                        {
                            noOfUSB++;
                        }
                    }
                }
            });
        }

        private async void label10_Click(object sender, EventArgs e)
        {
            label10.Text = "Scanning again";
            await USBLoad();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Download down = new Download();
            down.mode = "USB";
            this.Close();
            down.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (scanDone)
            {
                Download down = new Download();
                down.mode = "DVD";
                this.Close();
                down.Show();
            } else
            {
                MessageBox.Show("Please wait, drives are still loading!");
            }
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (scanDone)
            {
                Download down = new Download();
                down.mode = "HDD";
                this.Close();
                down.Show();
            }
            else
            {
                MessageBox.Show("Please wait, drives are still loading!");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            VM vM = new VM();
            this.Close();
            vM.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            List<Form> formsToClose = new List<Form>();
            foreach (Form form in Application.OpenForms)
            {
                formsToClose.Add(form);
            }
            foreach (Form form in formsToClose)
            {
                form.Close();
            }
        }
    }
}
