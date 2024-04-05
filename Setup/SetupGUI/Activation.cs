using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SetupGUI
{
    public partial class Activation : Form
    {
        private bool _agreed;
        public Activation()
        {
            InitializeComponent();
        }

        private void Activation_Load(object sender, EventArgs e)
        {
            string projectMessage = @"Thank you for using my project. Currently, there is an issue on how I can give Windows without needing a key. To tackle this problem, you now must provide a key at pre-setup and at setup too. There is no way to validate the key without actually redeeming it, so for now the project runs on a trust system. I will not claim any responsibility on any actions taken by this key check, it is up to you to provide a valid key. The criteria goes as follows:

- The key must be legitimate and not bought from third-party websites but directly from Microsoft.

- The key is a Pro version key or Enterprise (depending on what release/version you choose) and cannot be a Home version.

- The key is not part of the blacklist.

Complying with the criteria would lead to a smoother installation and makes all parties happy. Would it also be noted that not all blacklisted keys are available due to security reasons, so some generated keys might be accepted.
";
            richTextBox1.AppendText(projectMessage);
            richTextBox1.SelectionStart = 0;
            richTextBox2.AppendText("By using this setup, you agree to use a key that follows the criteria listed and has been verified as legitimate by purchasing it from Microsoft and by using VAMT. Help for setting up VAMT was provided in the video.");
            axWindowsMediaPlayer1.URL = "https://github.com/eliasailenei/PortableISO/raw/main/Videos/activetut.mp4";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (axWindowsMediaPlayer1.fullScreen)
                {
                    axWindowsMediaPlayer1.fullScreen = false;
                }
                else
                {
                    axWindowsMediaPlayer1.fullScreen = true;
                }
            } catch (Exception ex)
            {
                MessageBox.Show("ERROR:" + ex.Message + ".Wait to video to load for 5 seconds or maybe try to use this link instead --> https://github.com/eliasailenei/PortableISO/raw/main/Videos/activetut.mp4");
            }
            
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (_agreed)
            {
                _agreed = false;
            } else
            {
                _agreed = true;
            }
            checkBox1.Checked = _agreed;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (_agreed)
            {
                MessageBox.Show("Thank you for playing fair! Keep the key safe as you will need it at setup! ");
                axWindowsMediaPlayer1.Dispose();
                Backup backup = new Backup();
                backup.ShowDialog();
                this.Close();
            }
            else
            {
                MessageBox.Show("Program will not continue until you agree, you may close the setup by clicking the arrow pointing at the door.");

            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
