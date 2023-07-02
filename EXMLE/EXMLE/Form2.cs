using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;


namespace EXMLE
{
    public partial class Form2 : Form
    {
        bool initiate;
        private DataTable table;
        string drivenum = "0";
        int otherDriveSize;
        int lefts;
        int drivenumr = 1;
        bool visible;
        bool makeextradrives;
        int cout = 1;
        string cdrive;
        public string folderpath { get; set; }

        public Form2()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("PortableISO will always default to 0, so make sure you choose the right drive... PortableISO also uses 20GB of your drive initially to download and extract Windows... Make sure you have at least 40/50GB free! ");
        }



        private void button2_Click(object sender, EventArgs e)
        {
            if (initiate == false)
            {
                initiate = true;
                MessageBox.Show("You only need to do this once, we need to know how much space you have. Click OK to continue...");
                string total = Microsoft.VisualBasic.Interaction.InputBox("How much storage do you predict your disk drive will have in GB? 1GB = 1000MB", "Initiate");


                if (!string.IsNullOrEmpty(total))
                {
                    if (int.TryParse(total, out int totalSize))
                    {
                        if (totalSize > 35)
                        {
                            textBox2.Text = total;
                        }
                        else
                        {
                            MessageBox.Show("Please enter a disk drive size above 35GB.", "Insufficient Storage");
                            this.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid input. Please enter a valid numeric value.", "Invalid Input");
                        this.Close();
                    }
                }
                cdrive = Microsoft.VisualBasic.Interaction.InputBox("How much storage would you allocate for the C Drive? 1GB = 1000MB", "Initiate");


                if (!string.IsNullOrEmpty(cdrive))
                {
                    if (int.TryParse(cdrive, out int cdriveSize))
                    {
                        if (cdriveSize >= 12)
                        {
                            otherDriveSize = cdriveSize - 20;
                            if (otherDriveSize >= 0)
                            {
                                textBox3.Text = cdrive;
                                textBox4.Text = otherDriveSize.ToString();
                            }
                            else
                            {
                                MessageBox.Show("Insufficient storage for the other drive. Please allocate more space.", "Insufficient Storage");
                                this.Close();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Insufficient storage for the C drive. You need at least 12GB for Windows.", "Insufficient Storage");
                            this.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid input. Please enter a valid numeric value.", "Invalid Input");
                        this.Close();
                    }
                }
                if (string.IsNullOrEmpty(total) || string.IsNullOrEmpty(cdrive))
                {
                    MessageBox.Show("Invalid input, closing form!");
                    this.Close();
                }




                int remaininginMB = otherDriveSize * 1000;
                while (remaininginMB != 0)
                {
                    string newdirve = Microsoft.VisualBasic.Interaction.InputBox("How much storage would you allocate for the new drive in MB? 1GB = 1000MB", "Initiate");
                    int.TryParse(newdirve, out int newDriveValue);
                    int lefts = remaininginMB - newDriveValue;
                    remaininginMB = remaininginMB - newDriveValue;
                    if (remaininginMB <= 0)
                    {
                        MessageBox.Show("Error: No more space left.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
                    drivenumr = drivenumr + 1;
                    string drivename = Microsoft.VisualBasic.Interaction.InputBox("Name for drive.", "Initiate");
                    char driveLetter = '\0'; // Initialize with null character

                    while (!char.IsLetter(driveLetter) || driveLetter < 'A' || driveLetter > 'Z' || driveLetter == 'C' || driveLetter == 'F')
                    {
                        string input = Microsoft.VisualBasic.Interaction.InputBox("Letter for drive.", "Initiate");

                        if (input.Length != 1)
                        {
                            MessageBox.Show("Please enter only a single character for the drive letter.");
                        }
                        else
                        {
                            driveLetter = char.ToUpper(input[0]);

                            if (!char.IsLetter(driveLetter) || driveLetter < 'A' || driveLetter > 'Z')
                            {
                                MessageBox.Show("Please enter a valid alphabetic character for the drive letter (A-Z).");
                            }
                            else if (driveLetter == 'C' || driveLetter == 'F')
                            {
                                MessageBox.Show("This letter cannot be used. Please choose a different one.");
                            }
                        }
                    }

                    string letter = driveLetter.ToString();

                    DialogResult result = MessageBox.Show("Do you want to make your drive visible?", "Visible", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        visible = true;
                    }
                    else
                    {
                        visible = false;
                    }
                    table.Rows.Add(drivename, drivenumr, newDriveValue, letter, result);
                    DialogResult results = MessageBox.Show("Do you want to leave the space unallocated?", "End loop?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (results == DialogResult.Yes)
                    {
                        break;
                    }
                    else
                    {

                    }

                }
            }
            else
            {
                MessageBox.Show("Try the form again to modify");
            }




        }

        private void Form2_Load(object sender, EventArgs e)
        {
            this.Size = new Size(313, 175);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            makeextradrives = true;
            if ((checkBox1.Checked))
            {
                this.Size = new Size(313, 440);
                DataSet data = new DataSet("Configuration");
                table = new DataTable("Disks");
                table.Columns.Add("Drive Name");
                table.Columns.Add("Disk Number");
                table.Columns.Add("Disk Space");
                table.Columns.Add("Disk Letter");
                table.Columns.Add("Disk Visibility");
                data.Tables.Add(table);
                dataGridView1.DataSource = table;
            }
            else
            {
                this.Size = new Size(313, 175);
                dataGridView1.DataSource = null;
                table = null;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            string xmlPath = folderpath + "\\config.xml";

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath);

            XmlNode enterpriseModeNode = xmlDoc.DocumentElement;

            XmlNode lgpNode = enterpriseModeNode.SelectSingleNode("Drives");
            if (lgpNode != null)
            {
                enterpriseModeNode.RemoveChild(lgpNode);
            }

            lgpNode = xmlDoc.CreateElement("Drives");
            enterpriseModeNode.AppendChild(lgpNode);

            XmlNode isEnabledNode = lgpNode.SelectSingleNode("DiskNum");
            if (isEnabledNode == null)
            {
                isEnabledNode = xmlDoc.CreateElement("DiskNum");
                lgpNode.AppendChild(isEnabledNode);
            }
            XmlNode cma = lgpNode.SelectSingleNode("CdriveAllocation");
            if (cma == null)
            {
                cma = xmlDoc.CreateElement("CdriveAllocation");
                lgpNode.AppendChild(cma);
            }
            XmlNode locationNode = lgpNode.SelectSingleNode("CreateNewDrives");
            if (locationNode == null)
            {
                locationNode = xmlDoc.CreateElement("CreateNewDrives");
                lgpNode.AppendChild(locationNode);
            }
            isEnabledNode.InnerText = drivenum;
            locationNode.InnerText = makeextradrives.ToString();
            cma.InnerText = cdrive;

            try
            {
                if (table != null && lgpNode != null && xmlDoc != null)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        XmlNode rowNode = xmlDoc.CreateElement("Partition_" + cout);
                        lgpNode.AppendChild(rowNode);
                        cout++;
                        foreach (DataColumn column in table.Columns)
                        {
                            string columnName = SanitizeElementName(column.ColumnName);
                            XmlNode columnNode = xmlDoc.CreateElement(columnName);
                            columnNode.InnerText = row[column].ToString();
                            rowNode.AppendChild(columnNode);
                        }
                    }
                    xmlDoc.Save(xmlPath);
                }
                this.Close();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private string SanitizeElementName(string name)
        {
            char[] invalidChars = new char[] { ' ', '\'', '"' };
            string sanitized = name;
            foreach (char invalidChar in invalidChars)
            {
                sanitized = sanitized.Replace(invalidChar, '_');
            }

            return sanitized;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            drivenum = textBox1.Text;
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("Use this tool to help you setup your disk storage and make new disks for future.");
        }
    }
}
