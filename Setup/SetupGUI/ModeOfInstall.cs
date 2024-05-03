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
            InitializeComponent(); // this is where the UI components are loaded
        }

        private async void ModeOfInstall_Load(object sender, EventArgs e)
        {
            string content = "You have 4 options; it is recommended you choose USB if you have a USB drive.\n\nRemember that the image is not always 700MB and can be more than a 1GB depending on drivers and if its WinRE or WinPE.\n\nAllow at least 30 seconds for the drives to scan.\nEXAMINERS USE VM!";
            richTextBox1.AppendText(content); // adds the text that the user can follow
            try
            {
                File.Delete("diskinfo.txt");// deletes any previous attempts
                File.Delete("letters.txt");
            }
            catch {
                Console.WriteLine("This isn't users first time");
            }
            
            button1.Enabled = false;
            await USBLoad(); // get list of drives
        }

        private async Task USBLoad()
        {
            pictureBox1.Visible = true;
            scanDone = false;
            await getUSB(); // init the list
            await FormatList(); // make the list appropriate
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
                    process.StartInfo.Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"Get-Disk | Out-File -FilePath 'diskinfo.txt'\""; // this will get us all the disks avaliable
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
                Regex keepOnlyDrive = new Regex(@"^\d"); // we only want drives
                Regex keepOnlyUSB = new Regex(@".*USB.*"); // we only want disks

                using (StreamReader original = new StreamReader("diskinfo.txt")) // read the output from PS // Writing and reading from files
                {
                    string line;
                    while ((line = original.ReadLine()) != null)
                    {
                        if (keepOnlyDrive.IsMatch(line) && keepOnlyUSB.IsMatch(line))
                        {
                            noOfUSB++; // count the number of usbs avaliable
                        }
                    }
                }
            });
        }

        private async void label10_Click(object sender, EventArgs e)
        {
            label10.Text = "Scanning again";
            await USBLoad(); // rescan for drives
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Download down = new Download();
            down.mode = "USB";
            this.Close();
            down.Show(); // go to next setup and remember that we want a USB install
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (scanDone)
            {
                Download down = new Download();
                down.mode = "DVD";
                this.Close();
                down.Show(); // go to next setup and remember that we want a DVD install
            } else
            {
                MessageBox.Show("Please wait, drives are still loading!"); // the user must wait untill the disks have finished scanning
            }
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (scanDone)
            {
                Download down = new Download();
                down.mode = "HDD";
                this.Close();
                down.Show(); // go to next setup and remember that we want a HDD install
            }
            else
            {
                MessageBox.Show("Please wait, drives are still loading!"); // the user must wait untill the disks have finished scanning
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                VM vM = new VM();
                this.Close();
                vM.Show(); // launch the video for the user to follow
            } catch (Exception ex)
            {
                var message = MessageBox.Show("It looks like Windows Media Player failed to launch. This is mainly caused by computers with the N or KN versions. Microsoft has already given a hotfix, would you like to apply? You will need to restart PC.", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                if (message == DialogResult.Yes)
                {
                    Process.Start("cmd.exe", "/c DISM /Online /Add-Capability /CapabilityName:Media.MediaFeaturePack~~~~0.0.1.0"); // sometimes the user has a KN version, MS has recently put a patch for these users
                }
            }
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // close all windows algorithm
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
