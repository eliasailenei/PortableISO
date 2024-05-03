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
    public partial class Backup : Form
    {
        public Backup()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/eliasailenei/PortableBackup/releases/tag/V1");
            this.Close();

        }

        private void Backup_Load(object sender, EventArgs e)
        {

        }
    }
}
