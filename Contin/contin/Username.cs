using CustomConfig;
using mainUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace contin
{
    public partial class Username : Form
    {
        SQLCheck sql;
        DriveLetters drive;
        bool auto;
        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

        static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);

        static readonly IntPtr HWND_TOP = new IntPtr(0);

        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

        const UInt32 SWP_NOSIZE = 0x0001;

        const UInt32 SWP_NOMOVE = 0x0002;

        const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;



        [DllImport("user32.dll")]

        [return: MarshalAs(UnmanagedType.Bool)]

        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        public string topass { get; set; }
        public string language { get; set; }
        public string username = "PortableISO";
        public bool hasChanged = false;
        private Size form;
        private Rectangle labl2, bttn1, txtb1, buttn6;
        public Username(SQLCheck sqls, DriveLetters drives)
        {
            this.sql = sqls;
            this.drive = drives;
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            this.Resize += Rsize;
            form = this.Size;
            labl2 = new Rectangle(label2.Location, label2.Size);   
            bttn1 = new Rectangle(button1.Location, button1.Size);
            txtb1 = new Rectangle(textBox1.Location, textBox1.Size);    
            buttn6 = new Rectangle(button6.Location, button6.Size);
        }
        private void Rsize(object sender, EventArgs e)
        {
            resizeControl(labl2, label2);
            resizeControl(bttn1, button1 );
            resizeControl(buttn6, button6);
            resizeControl(txtb1, textBox1 );
        }
        private void Username_Load(object sender, EventArgs e)
        {
            SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
            if (sql.xmlStatus() && !String.IsNullOrEmpty(sql.OSUsername))
            {
                auto = true;
                button6_Click(sender, e);
            }
        }
        

        private void textBox1_Enter_1(object sender, EventArgs e)
        {
            if (textBox1.Text == "Example: PortableISO")
            {
                textBox1.Text = "";
                textBox1.ForeColor = SystemColors.WindowText; 
            }
        }

        private void textBox1_Leave_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                textBox1.Text = "Example: PortableISO";
                textBox1.ForeColor = SystemColors.GrayText; 
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.ForeColor = SystemColors.WindowText;
            string invalidUsername = "NONE";
            string administratorUsername = "Administrator";
            string inputText = textBox1.Text;
            char[] invalidPathChars = Path.GetInvalidPathChars();
            string additionalInvalidChars = "&\"/[]:|<>+=;,?*%@";
            string allInvalidChars = new string(invalidPathChars) + additionalInvalidChars;

            if (inputText.IndexOfAny(allInvalidChars.ToCharArray()) != -1 || inputText.Contains(" "))
            {
                MessageBox.Show("Username cannot contain invalid characters or spaces.");
                textBox1.Text = username;
            }
            else if (inputText.Equals(invalidUsername, StringComparison.OrdinalIgnoreCase) || inputText.Equals(administratorUsername, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Username cannot be 'NONE' or 'Administrator'.");
                textBox1.Text = username;
            }
            else
            {
                hasChanged = true;
                username = inputText;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (auto)
            {
                Password form = new Password(sql,drive);
                form.username = sql.OSUsername;
                form.language = language;
                form.topass = topass;
                form.Show();
                this.Close();
            }
            else
            {
                if (hasChanged == false)
                {
                    DialogResult iquit = MessageBox.Show("It looks like you have not changed the default username! Please click on \"Example: PortableISO\" and choose what you want. Otherwise, you will be stuck with \"PortableISO\"! You cannot change the username after clicking Yes. It is recommended you change it...", "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (iquit == DialogResult.Yes)
                    {
                        Password form = new Password(sql, drive);
                        form.username = username;
                        form.language = language;
                        form.topass = topass;
                        form.ShowDialog();
                        this.Close();
                    }
                    else
                    {
                    }
                }
                else if (username == null | username == "")
                {
                    MessageBox.Show("Username can't be empty");
                }
                else
                {
                    Password form = new Password(sql, drive);
                    form.username = username;
                    form.language = language;
                    form.topass = topass;
                    form.ShowDialog();
                    this.Close();
                }
            }
           
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Opacity == 1)
            {
                timer1.Stop();
            }
            Opacity += .4;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (Opacity == 0)
            {
                this.Hide();
            }
            Opacity -= .4;
        }
        private void resizeControl(Rectangle r, Control c)
        {
            float xRatio = (float)this.Width / form.Width;
            float yRatio = (float)this.Height / form.Height;

            int newX = (int)(r.X * xRatio);
            int newY = (int)(r.Y * yRatio);

            int newWidth = (int)(r.Width * xRatio);
            int newHeight = (int)(r.Height * yRatio);

            c.Location = new Point(newX, newY);
            c.Size = new Size(newWidth, newHeight);
        }
    }
}
