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
        public bool isReady { get; set; }  
        public CustomLocation()
        {
            InitializeComponent();
        }
        public event EventHandler InteractionComplete;

        private void button2_Click(object sender, EventArgs e)
        {
            sql.setCustomLocation(textBox1.Text);
            if (sql.xmlStatus())
            {
                if (sql.isOnline)
                {
                    Cursor = Cursors.WaitCursor;
                    remoteData remote = new remoteData(sql);
                    remote.getAutoStatus();
                    Cursor = Cursors.Arrow;
                }
                else
                {
                    localData local = new localData(sql);
                }
                InteractionComplete.Invoke(this, EventArgs.Empty);
                isReady = true; 
            }
            else
            {
                MessageBox.Show("Incompatible XML! Please use one that was made by EXMLE!");
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
