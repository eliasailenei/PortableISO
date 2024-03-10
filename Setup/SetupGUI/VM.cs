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
    public partial class VM : Form
    {
        public VM()
        {
            InitializeComponent();
        }

        private void VM_Load(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.URL = "https://www.youtube.com/watch?v=soFluDJSoFc";
        }
    }
}
