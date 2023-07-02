using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EXMLE
{
    public partial class Generalize : Form
    {
        bool doNotCleanUpNonPresentDevices;
        bool persistAllDeviceInstalls;
        public bool DoNotCleanUpNonPresentDevices
        {
            get
            {
                return doNotCleanUpNonPresentDevices;
            }
        }
        public bool PersistAllDeviceInstalls
        {
            get
            {
                return persistAllDeviceInstalls;
            }
        }
        public Generalize()
        {
            InitializeComponent();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                doNotCleanUpNonPresentDevices = true;
                label6.Text = doNotCleanUpNonPresentDevices.ToString();
                label6.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                doNotCleanUpNonPresentDevices = false;
                label6.Text = doNotCleanUpNonPresentDevices.ToString();
                label6.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void Generalize_Load(object sender, EventArgs e)
        {
            label6.Text = doNotCleanUpNonPresentDevices.ToString();
            label7.Text = persistAllDeviceInstalls.ToString();
            label6.ForeColor = System.Drawing.Color.Red;
            label7.ForeColor = System.Drawing.Color.Red;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                persistAllDeviceInstalls = true;
                label7.Text = persistAllDeviceInstalls.ToString();
                label7.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                persistAllDeviceInstalls = false;
                label7.Text = persistAllDeviceInstalls.ToString();
                label7.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            


        }
    }
}
