using CustomConfig;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mainUI
{
    public partial class CustomLocation : UserControl
    {
        public SQLCheck sql { get; set; }
        public CustomLocation()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            sql.setCustomLocation(textBox1.Text);
            if (sql.xmlStatus())
            {
                MessageBox.Show("Done");
                this.Hide();
            } else
            {
                MessageBox.Show("Not a valid path, shutting down!");
                Process.Start(new ProcessStartInfo
                {
                    FileName = @"wpeutil.exe",
                    Arguments = $"shutdown",
                    UseShellExecute = false,
                    CreateNoWindow = true
                });

            }
        }
    }
}
