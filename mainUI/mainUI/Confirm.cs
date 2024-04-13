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
using System.Net;

namespace mainUI
{
    public partial class Confirm : UserControl
    {
        public Confirm()
        {
            InitializeComponent();
        }

        
        public event EventHandler InteractionComplete;
        private void button2_Click(object sender, EventArgs e)
        {
            if (richTextBox1.Text.Contains("I am only a student that wants to help other people with installing Windows. If you wish to remove your service from my project, feel free to contact me on GitHub. I have only used resources that are already free to the public, but will still respect your wishes."))
            {
                InteractionComplete.Invoke(this, EventArgs.Empty);
                this.Hide();
            }
            else
            {
                MessageBox.Show("Program has detected that the disclaimer has been tampered with or license is missing! Due to this, program will shutdown your PC as it might be a malicious copy. You can get a geniune copy from github.com/eliasailenei. ");
                Process.Start(new ProcessStartInfo
                {
                    FileName = @"wpeutil.exe",
                    Arguments = $"shutdown",
                    UseShellExecute = false,
                    CreateNoWindow = true
                });
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult iquit = MessageBox.Show("Do you want to disagree with the terms and services of this program? This will shutdown your PC! If would like changes, say your opinion at github.com/eliasailenei.", "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            try
            {
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
                    this.Hide();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void Confirm_Load(object sender, EventArgs e)
        {
            if (File.Exists("Readme.txt"))
            {
                File.Delete("Readme.txt");
            }
            using (var client = new WebClient())
            {
                client.DownloadFile("https://raw.githubusercontent.com/eliasailenei/PortableISO/main/Readme.txt", "Readme.txt");
            }
            richTextBox1.Text = File.ReadAllText("Readme.txt");
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
