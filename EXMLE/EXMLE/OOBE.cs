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
    public partial class OOBE : Form
    {
        public bool DoNotCleanUpNonPresentDevices { get; set; }
        public bool PersistAllDeviceInstalls { get; set; }
        
        public OOBE()
        {
            InitializeComponent();
        }

        private void OOBE_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Generalize generalize = new Generalize();
            DialogResult response = generalize.ShowDialog();
            if (response == DialogResult.OK)
            {
                DoNotCleanUpNonPresentDevices = generalize.DoNotCleanUpNonPresentDevices;
                PersistAllDeviceInstalls = generalize.PersistAllDeviceInstalls;
            }
            MessageBox.Show(DoNotCleanUpNonPresentDevices.ToString() + " " + PersistAllDeviceInstalls.ToString());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Specialize specialize = new Specialize();
            DialogResult response = specialize.ShowDialog();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("Each button will take you to different form where you can adjust OOBE settings e.g background color or powerplan... If you don't know, leave everything blank and Windows will just default to basic settings. Note: all settings are based on Windows 10 Pro 22H2, some settings might not apply or be seen here!");
        }
    }
}
