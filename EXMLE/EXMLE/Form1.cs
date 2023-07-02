using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace EXMLE
{
    public partial class Form1 : Form
    {
        bool createnew = false;
        string folderlocation;
        bool folder = true;
        bool cleaninstall = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private void makexml(string folderlocation)
        {
            XmlDocument newxml = new XmlDocument();
            XmlElement rootElement = newxml.CreateElement("PortableISO");
            newxml.AppendChild(rootElement);

            XmlElement enterpriseModeElement = newxml.CreateElement("EnterpriseMode");
            rootElement.AppendChild(enterpriseModeElement);

            XmlElement isEnabledElement = newxml.CreateElement("IsEnabled");
            isEnabledElement.InnerText = "yes";
            enterpriseModeElement.AppendChild(isEnabledElement);

            string filePath = folderlocation + "/config.xml";
            newxml.Save(filePath);
        }

        private void checkxml(string folderlocation)
        {

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(folderlocation + "/config.xml");
                XmlNode node = xmlDoc.SelectSingleNode("PortableISO/EnterpriseMode/IsEnabled");
                if (node != null && node.InnerText == "yes" || node != null && node.InnerText == "no")
                {
                    MessageBox.Show("XML file imported sucessfully!");
                }
                else
                {
                    MessageBox.Show("It looks like the current config.xml wasn't made by EMXMLE or is non-exsistant. We will make one instead and get rid of old one...");
                    createnew = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("It looks like the current config.xml wasn't made by EMXMLE or is non-exsistant. We will make one instead and get rid of old one...");
                createnew = true;
            }



        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                folderlocation = folderBrowserDialog1.SelectedPath;
                makexml(folderlocation);
                textBox1.Text = folderlocation + "\\config.xml";
                Directory.CreateDirectory(folderlocation + "/ebin");
                List<Button> buttons = new List<Button> { button3, button4, button5, button6, button7, button8, button9, button10, button11 };

                for (int i = 0; i < buttons.Count; i++)
                {
                    buttons[i].Enabled = true;
                }
                richTextBox1.Enabled = true;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                folderlocation = folderBrowserDialog1.SelectedPath;
                checkxml(folderlocation);
                isebinthere();
                if (createnew == true && folder == false)
                {
                    File.Delete(folderlocation + "/config.xml");
                    makexml(folderlocation);
                    textBox1.Text = folderlocation + "\\config.xml";
                    Directory.CreateDirectory(folderlocation + "/ebin");
                }
                else if (createnew == true)
                {
                    File.Delete(folderlocation + "/config.xml");
                    makexml(folderlocation);
                    textBox1.Text = folderlocation + "\\config.xml";
                }
                else
                {
                    textBox1.Text = folderlocation + "\\config.xml";
                }
                List<Button> buttons = new List<Button> { button3, button4, button5, button6, button7, button8, button9, button10, button11 };

                for (int i = 0; i < buttons.Count; i++)
                {
                    buttons[i].Enabled = true;
                }
                richTextBox1.Enabled = true;
            }
        }
        private void isebinthere()
        {
            if (Directory.Exists(folderlocation + "/ebin"))
            {
                folder = true;
            }
            else
            {
                MessageBox.Show("ebin folder is missing. We will create one now");
                folder = false;
                Directory.CreateDirectory(folderlocation + "/ebin");
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            LGP transfer = new LGP();
            transfer.folderpath = folderlocation;
            transfer.ShowDialog();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            List<Button> buttons = new List<Button> { button3, button4, button5, button6, button7, button8, button9, button10, button11 };

            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].Enabled = false;
            }
            richTextBox1.Enabled = false;

        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            PortableISO transfers = new PortableISO();
            transfers.folderpath = folderlocation;
            transfers.clean = cleaninstall;
            transfers.Show();
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            Form2 transfers = new Form2();
            transfers.folderpath = folderlocation;
            transfers.Show();
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            OOBE transfers = new OOBE();
            transfers.Show();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("Welcome to EXMLE! You can start with making a new XML file if you are new here or import one to tweak its settings. Click on the indivdual buttons to help you build your XML file to automate PortableISO PE...");
        }
    }
}

