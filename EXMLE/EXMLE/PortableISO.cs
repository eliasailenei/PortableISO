using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic.Devices;
using System.Xml;

namespace EXMLE
{
    public partial class PortableISO : Form
    {
        public bool clean { get; set; }
        bool bioskey;
        bool wlan;
        public string folderpath { get; set; }
        string winkey, ssid, nkey;
        bool sizechange;
        public PortableISO()
        {
            InitializeComponent();
        }

        private void PortableISO_Load(object sender, EventArgs e)
        {
            this.Size = new Size(this.Size.Width, this.Size.Height - 102);
            label4.Visible = false;
            label5.Visible = false;
            label6.Visible = false;
            richTextBox1.Visible = false;
            textBox4.Visible = false;
            linkLabel2.Visible = false;
            string[] windowsVersions = { "Windows 7 (Unsupported)", "Windows 8.1 (Unsupported)", "Windows 10", "Windows 11" };
            for (int i = 0; i <= 3; i++)
            {
                comboBox1.Items.Add(windowsVersions[i]);
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            label4.Visible = true;
            label5.Visible = true;
            label6.Visible = true;
            richTextBox1.Visible = true;
            textBox4.Visible = true;
            wlan = true;
            sizechange = true;
            if (radioButton1.Checked)
            {
                this.Size = new Size(399, 357);
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            label4.Visible = false;
            label5.Visible = false;
            label6.Visible = false;
            richTextBox1.Visible = false;
            textBox4.Visible = false;
            ssid = string.Empty;
            nkey = string.Empty;
            if (radioButton2.Checked)
            {
                this.Size = new Size(399, 253);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("In order for this program to function, you will need to get a Product key that is valid. This allows this project to be legal and also determine what Windows version would be installed...");
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            bioskey = true;
            MessageBox.Show("Although this might get you a valid key, not all PC's have this feature. Make sure you still provide a license key!");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Do you want to proceed? This will mean that any configurations will not be used! Just a clean install!", "DANGER ZONE!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                clean = true;
                linkLabel2.Visible = true;
            }
            else
            {
                clean = false;
                MessageBox.Show("No changes done...");
                linkLabel2.Visible = false;
            }
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("This will mean that any configurations will not be used! Just a clean install! You only need to complete PortableISO and Installation. The rest is not applicable. You may reset by clicking the button and selecting No.");
        }

     

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            string textToEncode = textBox4.Text;
            byte[] textBytes = Encoding.UTF8.GetBytes(textToEncode);
            string encodedText = Convert.ToBase64String(textBytes);
            nkey = encodedText;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

            if (bioskey == true && string.IsNullOrEmpty(richTextBox2.Text))
            {
                MessageBox.Show("Please enter a Key... PortableISO requires you to have a key for backup!");
            }
            else if (string.IsNullOrEmpty(richTextBox2.Text))
            {
                MessageBox.Show("Please enter a Key... PortableISO requires you to have a key for backup!");
            }
            else if (comboBox1.SelectedItem == null || comboBox1.SelectedItem.ToString() == "")
            {
                MessageBox.Show("Please enter a windows version!");
            }
            else if (wlan == true && (string.IsNullOrEmpty(richTextBox1.Text) || string.IsNullOrEmpty(nkey)))
            {
                MessageBox.Show("Error: no WiFi config! Please enter a SSID and a password!");
            }
            string xmlPath = folderpath + "/config.xml";

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath);

            XmlNode enterpriseModeNode = xmlDoc.DocumentElement;

            XmlNode portableISOConfig = enterpriseModeNode.SelectSingleNode("PortableISOConfig");
            if (portableISOConfig == null)
            {
                portableISOConfig = xmlDoc.CreateElement("PortableISOConfig");
                enterpriseModeNode.AppendChild(portableISOConfig);
            }

            string[] nodeNames = { "WindowsVer", "WinKey", "PreloadKey", "CleanInstall", "IsInWLANMode", "SSID", "Nkey" };

            foreach (string nodeName in nodeNames)
            {
                XmlNode selectedNode = portableISOConfig.SelectSingleNode(nodeName);
                if (selectedNode == null)
                {
                    XmlNode newNode = xmlDoc.CreateElement(nodeName);
                    portableISOConfig.AppendChild(newNode);

                    // Set the values for the newly created node
                    if (nodeName == "WindowsVer")
                    {
                        if (comboBox1 != null && comboBox1.SelectedIndex >= 0)
                        {
                            newNode.InnerText = comboBox1.SelectedItem.ToString();
                        }
                    }
                    else if (nodeName == "WinKey")
                    {
                        newNode.InnerText = richTextBox2.Text;
                    }
                    else if (nodeName == "PreloadKey")
                    {
                        newNode.InnerText = bioskey ? "True" : "False";
                    }
                    else if (nodeName == "CleanInstall")
                    {
                        newNode.InnerText = clean ? "True" : "False";
                    }
                    else if (nodeName == "IsInWLANMode")
                    {
                        newNode.InnerText = wlan ? "True" : "False";
                    }
                    else if (nodeName == "SSID")
                    {
                        newNode.InnerText = richTextBox1.Text;
                    }
                    else if (nodeName == "Nkey")
                    {
                        newNode.InnerText = nkey;
                    }
                }
                else
                {
                    // Update the existing node with the variable values
                    if (nodeName == "WindowsVer")
                    {
                        if (comboBox1 != null && comboBox1.SelectedIndex >= 0)
                        {
                            selectedNode.InnerText = comboBox1.SelectedItem.ToString();
                        }
                    }
                    else if (nodeName == "WinKey")
                    {
                        selectedNode.InnerText = richTextBox2.Text;
                    }
                    else if (nodeName == "PreloadKey")
                    {
                        selectedNode.InnerText = bioskey ? "True" : "False";
                    }
                    else if (nodeName == "CleanInstall")
                    {
                        selectedNode.InnerText = clean ? "True" : "False";
                    }
                    else if (nodeName == "IsInWLANMode")
                    {
                        selectedNode.InnerText = wlan ? "True" : "False";
                    }
                    else if (nodeName == "SSID")
                    {
                        selectedNode.InnerText = richTextBox1.Text;
                    }
                    else if (nodeName == "Nkey")
                    {
                        selectedNode.InnerText = nkey;
                    }
                }
            }
            xmlDoc.Save(xmlPath);
            Close();
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("Very crutial step into PortableISO. This is where you get to choose what version of Windows you want and how to get it...");
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            string textToEncode = richTextBox1.Text;
            byte[] textBytes = Encoding.UTF8.GetBytes(textToEncode);
            string encodedText = Convert.ToBase64String(textBytes);
            winkey = encodedText;
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {
            string textToEncode = richTextBox2.Text;
            byte[] textBytes = Encoding.UTF8.GetBytes(textToEncode);
            string encodedText = Convert.ToBase64String(textBytes);
            ssid = encodedText;
        }
    }
}
