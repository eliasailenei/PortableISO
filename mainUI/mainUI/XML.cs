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
using BCrypt;
namespace mainUI
{
    public partial class XML : UserControl
    {
        public event EventHandler InteractionComplete;
        public string pass;
        public SQLCheck sql { get; set; }
        public XML()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            KeyGenerator key = new KeyGenerator();
            key.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (sql.serverUsername == null || pass == null || sql.serverKeyEnc == null || sql.serverKeyPassEnc == null)
            {
                MessageBox.Show("You have left critical information blank, please fill in the correct details.");
            } else
            {
                if (sql.userLogin(pass) == false)
                {
                    MessageBox.Show("Please check the login, it looks like you typed them wrong.");
                } else
                {
                    Cursor = Cursors.WaitCursor;
                    remoteData remote = new remoteData(sql);
                    Cursor = Cursors.Arrow;
                    InteractionComplete.Invoke(this, EventArgs.Empty);
                    this.Hide();
                }
            }
        }

        private void XML_Load(object sender, EventArgs e)
        {
            if (sql == null || String.IsNullOrEmpty(sql.serverUsername))
            {
                textBox1.Text = string.Empty;
            }
            else
            {
                textBox1.Text = sql.serverUsername;
            }

            if (sql == null || String.IsNullOrEmpty(sql.serverKeyEnc))
            {
                textBox2.Text = string.Empty;
            }
            else
            {
                textBox2.Text = sql.serverKeyEnc;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            sql.serverUsername = textBox1.Text;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            Decryptors dec = new Decryptors();
            pass = textBox2.Text;
            sql.serverPasswordEnc = dec.Encrypt(pass, "unlock", 128);
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            sql.serverKeyEnc = textBox3.Text;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            Decryptors newEnc = new Decryptors();
            sql.serverKeyPassEnc = newEnc.Encrypt(textBox4.Text, "unlock", 128); 
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
