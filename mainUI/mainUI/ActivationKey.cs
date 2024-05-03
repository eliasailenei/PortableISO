using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CustomConfig;
using System.Diagnostics;
namespace mainUI
{
    public partial class ActivationKey : UserControl
    {
        public event EventHandler InteractionComplete;
        bool hasBlacklist, hasRead, hasAccepted;
        Decryptors decryptors = new Decryptors();
        TextBox[] texts = new TextBox[4];
        private string key;
        activationLib active = new activationLib(); // simple oop

        Dictionary<string, bool> blacklist = new Dictionary<string, bool>();
        public ActivationKey()
        {
            InitializeComponent();
        }

        private void ActivationKey_Load(object sender, EventArgs e)
        {
            if (active.getBlacklist() == null || active.getBlacklist().Count == 0)
            {
                hasBlacklist = false;
            }
            else
            {
                blacklist = active.getBlacklist();  
                hasBlacklist = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            key = textBox1.Text + "-" + textBox2.Text + "-" + textBox3.Text + "-" + textBox4.Text + "-" + textBox5.Text;
            key = key.ToUpper();
            if (key == "0----")
            {
                MessageBox.Show("Skipping key activation should only be used for debugging! Proceed at risk!");
                globalStrings.windowsKey = key;
                InteractionComplete.Invoke(this, EventArgs.Empty);
                this.Hide();
            } else if (active.fitPattern(key))
            {
                if (hasBlacklist)
                {
                    if (blacklist.ContainsKey(key))
                    {
                        MessageBox.Show("Key is part of blacklist, if you were sold this key please get a refund immediately as its not genuine!");
                    } else
                    {
                        if (hasAccepted)
                        {
                            MessageBox.Show("Thank you for playing fair! You may continue!");
                            globalStrings.windowsKey = key;
                            File.WriteAllText("windowskey.txt", decryptors.Encrypt(globalStrings.windowsKey,"passforkey",128)); // writing and reading from file
                            this.Hide();
                            InteractionComplete.Invoke(this, EventArgs.Empty);
                        } else
                        {
                            MessageBox.Show("Accept the agreement first!");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Although the key is in a valid format, it might be a blacklisted key. Program failed to load the blacklist keys!");

                }
            }
            else
            {
                MessageBox.Show("This isn't a valid key format. Please provide a legit key!");
            }

        }
       
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Length >= 5)
            {
                textBox2.Focus();
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text.Length >= 5)
            {
                textBox3.Focus();
            } else if (textBox2.Text.Length == 0)
            {
                textBox1.Focus();
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (textBox4.Text.Length >= 5)
            {
                textBox5.Focus();
            }
            else if (textBox4.Text.Length == 0)
            {
                textBox3.Focus();
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (textBox3.Text.Length >= 5)
            {
                textBox4.Focus();
            }
            else if (textBox3.Text.Length == 0)
            {
                textBox2.Focus();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (hasRead)
            {
                if (hasAccepted)
                {
                    hasAccepted = false;
                    checkBox1.Checked = hasAccepted;
                }
                else
                {
                    hasAccepted = true;
                    checkBox1.Checked = hasAccepted;
                }
            } else
            {
                checkBox1.Checked=false;
                MessageBox.Show("Please read the limitations first!");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult iquit = MessageBox.Show("Do you want to terminate the program? This will shutdown your PC!", "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            try
            {
                if (iquit == DialogResult.Yes)
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = @"wpeutil.exe",
                        Arguments = $"shutdown",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    });
                }
                else
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = @"cmd.exe",
                        Arguments = $"",
                        //UseShellExecute = false,
                        //CreateNoWindow = true
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if (textBox5.Text.Length == 0)
            {
                textBox4.Focus();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Thank you for using my project. Currently, there is an issue on how I can give Windows without needing a key. To tackle this problem, you now must provide a key at pre-setup and at setup too. There is no way to validate the key without actually redeeming it, so for now the project runs on a trust system. I will not claim any responsibility on any actions taken by this key check, it is up to you to provide a valid key. The criteria goes as follows:\n\n- The key must be legitimate and not bought from third-party websites but directly from Microsoft.\n\n- The key is a Pro version key or Enterprise (depending on what release/version you choose) and cannot be a Home version.\n\n- The key is not part of the blacklist.\n\nComplying with the criteria would lead to a smoother installation and makes all parties happy. Would it also be noted that not all blacklisted keys are available due to security reasons, so some generated keys might be accepted.\n\nYou can also type 0 to skip this, but you will be breaking the EULA. This should only be used for debug purposes.\n\nAlso, I know that mashing you keyboard gives you a \"valid\" key but thats because of the regex, don't do that!", "Key Check Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            hasRead = true;
        }
    }
}
