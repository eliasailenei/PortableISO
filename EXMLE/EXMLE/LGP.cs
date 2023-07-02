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
using System.Diagnostics;

namespace EXMLE
{
    public partial class LGP : Form
    {
        public string folderpath { get; set; }
        bool exist = false;

        public LGP()
        {
            InitializeComponent();
        }

        private void LGP_Load(object sender, EventArgs e)
        {
            string filePath = folderpath;


            if (Directory.Exists(folderpath + "\\ebin\\LGP"))
            {

            }
            else
            {
                Directory.CreateDirectory(folderpath + "\\ebin/LGP");
            }

            if (File.Exists(folderpath + "/ebin/LGP/LGP.inf"))
            {
                button3.Enabled = true;
                button4.Enabled = true;
                exist = true;
            }
            else
            {
                button3.Enabled = false;
                button4.Enabled = false;
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            if (exist == false)
            {
                Process.Start("cmd.exe", "/c secedit /export /cfg " + folderpath + "/ebin/LGP/LGP.inf");
                button3.Enabled = true;
                button4.Enabled = true;
                exist = true;
            }
            else if (exist == true)
            {
                MessageBox.Show("Please delete the old file so that there is no clash!");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Process.Start(folderpath + "/ebin/LGP/LGP.inf");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            File.Delete(folderpath + "/ebin/LGP/LGP.inf");
            button3.Enabled = false;
            button4.Enabled = false;
            exist = false;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string xmlPath = folderpath + "/config.xml";

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath);

            XmlNode enterpriseModeNode = xmlDoc.DocumentElement;

            XmlNode lgpNode = enterpriseModeNode.SelectSingleNode("LGP");
            if (lgpNode == null)
            {
                lgpNode = xmlDoc.CreateElement("LGP");
                enterpriseModeNode.AppendChild(lgpNode);
            }

            XmlNode isEnabledNode = lgpNode.SelectSingleNode("IsEnabled");
            if (isEnabledNode == null)
            {
                isEnabledNode = xmlDoc.CreateElement("IsEnabled");
                lgpNode.AppendChild(isEnabledNode);
            }

            XmlNode locationNode = lgpNode.SelectSingleNode("Location");
            if (locationNode == null)
            {
                locationNode = xmlDoc.CreateElement("Location");
                lgpNode.AppendChild(locationNode);
            }

            isEnabledNode.InnerText = exist ? "yes" : "no";
            locationNode.InnerText = exist ? "/ebin/LGP/LGP.inf" : "na";

            xmlDoc.Save(xmlPath);
            Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("LGP stands for Local Group Policy. It will import whatever local group policy you are using now and apply it to the new Windows.");
        }
    }
}
