using CustomConfig;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace contin
{
    public partial class Password : Form
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
        private string password;
        private string passwordagain;
        private Size form;
        private Rectangle labl2, bttn1, txtb1, txtb2, buttn6;
        public string username { get; set; }
        public string topass { get; set; }
        public string language { get; set; }
        public Password(SQLCheck sqls, DriveLetters drives)
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
            txtb2 = new Rectangle(textBox2.Location, textBox2.Size);
            
        }
        private void Rsize(object sender, EventArgs e)
        {
            resizeControl(labl2, label2);
            resizeControl(bttn1, button1);
            resizeControl(buttn6, button6);
            resizeControl(txtb1, textBox1);
            resizeControl(txtb2 , textBox2);
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
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.PasswordChar = '*';
            password = textBox1.Text;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            textBox2.PasswordChar = '*';
            passwordagain = textBox2.Text;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (auto)
            {
                Custome form = new Custome(sql, drive);
                form.user = username;
                form.language = language;
                form.password = password;
                form.topass = topass;
                form.Show();
                this.Close();
            } else
            {
                if (password == null || textBox1.Text == "")
                {

                    MessageBox.Show("Password cannot be empty!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (passwordagain == null || textBox2.Text == "")
                {

                    MessageBox.Show("You forgot to enter the password again! Try again!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (!passwordagain.Equals(password))
                {
                    MessageBox.Show("Passwords do not match!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (passwordagain.Equals(password))
                {
                    Custome form = new Custome(sql, drive);
                    form.user = username;
                    form.language = language;
                    form.password = password;
                    form.topass = topass;
                    form.ShowDialog();
                    this.Close();
                }
            }
            
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == "Enter password here...")
            {
                textBox1.Text = "";
                textBox1.ForeColor = SystemColors.WindowText;
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                textBox1.Text = "Enter password here...";
                textBox1.ForeColor = SystemColors.GrayText;
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Password_Load(object sender, EventArgs e)
        {
            SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
            if (sql.xmlStatus() && !String.IsNullOrEmpty(sql.OSPassword))
            {
                auto = true;
                Decryptors decryptors = new Decryptors();
                password = decryptors.DESDecrypt(sql.OSPassword, username, 128);
                button6_Click(sender,e);
            }
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            if (textBox2.Text == "and again here.")
            {
                textBox2.Text = "";
                textBox2.ForeColor = SystemColors.WindowText;
            }
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                textBox2.Text = "and again here.";
                textBox2.ForeColor = SystemColors.GrayText;
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
    }
}
