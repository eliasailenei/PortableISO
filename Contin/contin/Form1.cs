using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CustomConfig;
namespace contin
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DriveLetters drives = new DriveLetters(Environment.SystemDirectory + "\\driveLetters.txt"); // reading and writing a file
            SQLCheck sql = new SQLCheck(); // simple OOP
            
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
            }
            mainUI.OSSelect change = new mainUI.OSSelect(sql, drives); // simple OOP
            change.ShowDialog();
            this.Close();
        }
    }
}
