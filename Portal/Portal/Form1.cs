using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Portal
{
    public partial class Form1 : Form
    {
        public bool usb;
        public string isoLocation;
        public Form1()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://paypal.me/eliasppl");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://github.com/eliasailenei");
        }

        private void button3_Click(object sender, EventArgs e)
        {
           ShowWarning();
            usb = true;
          
        }
        private void ShowWarning()
        {
            Warning showDiag = new Warning();
            this.Controls.Add(showDiag);
           showDiag.isUsb = usb;
            showDiag.BringToFront();
            showDiag.Show();
            showDiag.InteractionComplete += (s, args) =>
            {
                showDiag.Hide();
            };
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ADKcheck adk = new ADKcheck();
            adk.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Coming soon!");
        }
    }
}
