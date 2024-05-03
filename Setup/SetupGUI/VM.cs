using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SetupGUI
{
    public partial class VM : Form
    {
   
        public VM()
        {
            InitializeComponent();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            try // make the video player fullscreen
            {
                if (axWindowsMediaPlayer1.fullScreen)
                {
                    axWindowsMediaPlayer1.fullScreen = false; 
                }
                else
                {
                    axWindowsMediaPlayer1.fullScreen = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR:" + ex.Message + ".Wait to video to load for 5 seconds or maybe try to use this link instead --> https://github.com/eliasailenei/PortableISO/raw/main/Videos/vmtut.mp4");
            }
        }
        private void VM_Load(object sender, EventArgs e)
        {
            
            axWindowsMediaPlayer1.URL = "https://github.com/eliasailenei/PortableISO/raw/main/Videos/vmtut.mp4"; // loads the video player with the tutorial
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.virtualbox.org"); // link to virtualbox
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/eliasailenei/PortableISO/releases/tag/V2"); // link to the ISO
        }

        private void button4_Click(object sender, EventArgs e)
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
