using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CustomConfig;
namespace mainUI
{
    public partial class DualBoot : UserControl
    {
        public event EventHandler InteractionComplete;
        public DriveLetters drive { get; set; }
        public bool intent { get; set; }
        public DualBoot(bool theIntent)
        {
            intent = theIntent;
            InitializeComponent();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (intent)
            {
                MessageBox.Show("Please resolve the drive letter problem then you can quit.");
            } else
            {
                this.Hide();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private async void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.Text = textBox1.Text.ToUpper(); //text
            char[] unacceptableLetters = await drive.GetLettersAsync(); 
            string inputText = textBox1.Text.Trim(); 
            if (string.IsNullOrEmpty(inputText))
            {
                textBox1.Clear();
                return;
            }
            char inputChar = char.ToUpper(inputText[0]);
            if (inputText.Length == 1 && char.IsLetter(inputChar) && char.IsUpper(inputChar))
            {
                if (!unacceptableLetters.Contains(inputChar) && inputChar != drive.TLetter)
                {
                    drive.CLetter = inputChar;
                }
                else
                {
                    textBox1.Clear();
                }
            }
            else
            {
                textBox1.Clear();
            }
        }

        private async void textBox2_TextChanged(object sender, EventArgs e)
        {
            textBox2.Text = textBox2.Text.ToUpper();
            char[] unacceptableLetters = await drive.GetLettersAsync();
            string inputText = textBox2.Text.Trim();
            if (string.IsNullOrEmpty(inputText))
            {
                textBox1.Clear();
                return;
            }
            char inputChar = char.ToUpper(inputText[0]);
            if (inputText.Length == 1 && char.IsLetter(inputChar) && char.IsUpper(inputChar))
            {
                if (!unacceptableLetters.Contains(inputChar) && inputChar != drive.CLetter)
                {
                    drive.TLetter = inputChar;
                }
                else
                {
                    textBox2.Clear();
                }
            }
            else
            {
                textBox2.Clear();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (drive.CLetter != 'C' && drive.TLetter != 'T')
            {
                if (File.Exists(Environment.SystemDirectory + "\\driveLetters.txt")){
                    File.Delete(Environment.SystemDirectory + "\\driveLetters.txt");
                }
                string toWrite = drive.CLetter.ToString() + "\n" + drive.TLetter.ToString();
                File.WriteAllText(Environment.SystemDirectory + "\\driveLetters.txt", toWrite);
                InteractionComplete.Invoke(this, EventArgs.Empty);
                this.Hide();
              }
        }

        private void DualBoot_Load(object sender, EventArgs e)
        {
            if (intent)
            {
                label2.Show();
            } else
            {
                label2.Hide();
            }
        }
    }
}
