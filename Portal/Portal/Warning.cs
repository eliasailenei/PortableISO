using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Portal
{
    public partial class Warning : UserControl
    {
      public bool isUsb { get; set; }
        public Burn usb = new Burn(); 
        public Warning()
        {
            InitializeComponent();

        }
        public event EventHandler InteractionComplete;
        private void button4_Click(object sender, EventArgs e)
        {
            InteractionComplete.Invoke(this, EventArgs.Empty);
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
                InteractionComplete.Invoke(this, EventArgs.Empty);
                this.Hide();
            if (isUsb == true)
            {
                try
                {
                    usb.ShowDialog();
                } catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    this.Hide();
                }
                
                
            }
        }
    }
}
