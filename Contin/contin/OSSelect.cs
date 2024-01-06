using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.Remoting.Lifetime;
using System.Threading;
using System.Net;
using System.IO.Compression;

namespace mainUI
{
    public partial class OSSelect : Form
    {
        private int ver;
        DataTable Windows;
        private Rectangle lab1, lab2, lab3, lab4, lab6, lab8, but3, but4,  but6, but7, list1, list2;
        private Size form;
        private bool notAdded = true;
        private string rel, toPass;
        private string ISODown = @"T:\contin\";
        
        public OSSelect()
        {
            InitializeComponent();
            pictureBox1.Visible = false;
           FormBorderStyle = FormBorderStyle.None;
           WindowState = FormWindowState.Maximized;
           button6.Visible = false;
            this.Resize += OSSelect_rsize;
            form = this.Size;
            lab1 = new Rectangle(label1.Location, label1.Size);
            lab2 = new Rectangle(label2.Location, label2.Size);
            lab3 = new Rectangle(label3.Location, label3.Size);
            lab4 = new Rectangle(label4.Location, label4.Size);
            lab6 = new Rectangle(label6.Location, label6.Size);
            lab8 = new Rectangle(label8.Location, label8.Size);
            but3 = new Rectangle(button3.Location, button3.Size);
            but4 = new Rectangle(button4.Location, button4.Size);
            but6 = new Rectangle(button6.Location, button6.Size);
            but7 = new Rectangle(button7.Location, button7.Size);
            list1 = new Rectangle(listBox1.Location, listBox1.Size);
            list2 = new Rectangle(listBox2.Location, listBox2.Size);
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

        private void getLatest()
        {
            //This would get the latest version of required tool.
            //if (Directory.Exists("T:\\contin\\MSWISO"))
            //{
            //    Directory.Delete("T:\\contin\\MSWISO", true);
            ////}
            Directory.CreateDirectory("T:\\contin\\MSWISO");
            using (var client = new WebClient())
            {
                client.DownloadFile("https://github.com/eliasailenei/MSWISO/releases/download/Release/Stable.zip", "T:\\contin\\MSWISO.zip");
            }
            ZipFile.ExtractToDirectory("T:\\contin\\MSWISO.zip", "T:\\contin\\MSWISO");
            File.Delete("T:\\contin\\MSWISO.zip");
            // Gets DeployWindows at the same time as it both uses MSWISO
            using (var client = new WebClient())
            {
                client.DownloadFile("https://github.com/eliasailenei/DeployWindows/releases/download/Release/Program.zip", "T:\\contin\\DeployWindows.zip");
            }
            ZipFile.ExtractToDirectory("T:\\contin\\DeployWindows.zip", "T:\\contin\\");
            File.Delete("T:\\contin\\DeployWindows.zip");
        }
        
        private void OSSelect_Load_1(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.Default;
            getLatest();
            Windows = new DataTable();
            Windows.Columns.Add("Version");
            Windows.Columns.Add("Release");

            int[] versions = { 10, 11 };

            foreach (int item in versions)
            {
                populateRelease(item);
            }
        }

        public void populateRelease(int ver)
        {
            Process infoGain = new Process();
            infoGain.StartInfo.FileName = "MSWISO\\MSWISO.exe";
            infoGain.StartInfo.Arguments = "--ESDMode=True --WinVer=Windows_" + ver + " --Release=";
            infoGain.StartInfo.UseShellExecute = false;
            infoGain.StartInfo.CreateNoWindow = true;
            infoGain.Start();
            infoGain.WaitForExit();
            getInput(ver);

        }
        public void getInput(int ver)
        {
            string[] allEntries = File.ReadAllLines("output.txt");
            foreach (string release in allEntries)
            {
                DataRow releaseRow = Windows.NewRow();
                releaseRow["Version"] = ver;
                releaseRow["Release"] = release;
                Windows.Rows.Add(releaseRow);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                rel = listBox1.SelectedItem.ToString();
            } catch (Exception ex)
            {
                Console.WriteLine(ex.ToString() + " due to empty space being clicked. Error is ignored as it is not critical.");
            }
            if (notAdded)
            {
                notAdded = false;
                string[] languageArray = { "Arabic", "Bulgarian", "Czech", "Danish", "German", "Greek", "English", "Spanish", "Estonian", "Finnish", "French", "Hebrew", "Croatian", "Hungarian", "Italian", "Japanese", "Korean", "Lithuanian", "Latvian", "Dutch", "Norwegian", "Polish", "Romanian", "Russian", "Slovak", "Slovenian", "Serbian", "Swedish", "Thai", "Turkish", "Ukrainian" };
                listBox2.Items.Clear();
                listBox2.Items.AddRange(languageArray);
            }
        }
  
        private void OSSelect_rsize(object sender, EventArgs e)
        {
            resizeControl(lab1, label1);
            resizeControl(lab2, label2);
            resizeControl(lab3, label3);
            resizeControl(lab4, label4);
            resizeControl(lab6, label6);
            resizeControl(lab8, label8);

            resizeControl(but3, button3);
            resizeControl(but4, button4);
            resizeControl(but6, button6);
            resizeControl(but7, button7);

            resizeControl(list1, listBox1);
            resizeControl(list2, listBox2);
        }
        private void button3_Click(object sender, EventArgs e)
        {
            ver = 10;
            listBox1.Items.Clear();
            foreach (DataRow row in Windows.Rows)
            {
                if (row["Version"].ToString() == "10")
                {
                    listBox1.Items.Add(row["Release"].ToString());
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ver = 11;
            listBox1.Items.Clear();
            foreach (DataRow row in Windows.Rows)
            {
                if (row["Version"].ToString() == "11")
                {
                    listBox1.Items.Add(row["Release"].ToString());
                }
            }
        }

       
        static string argsFormat(string input)
        {
            return input.Replace(' ', '_');
        }
        
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            button6.Visible = true;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            DialogResult iquit = MessageBox.Show("Do you want to terminate the program? This will shutdown your PC!", "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

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
                this.Close();
            }
            Opacity -= .4;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Form2 sel = new Form2();

            
                toPass = @"--ESDMode=True --Location=" + ISODown + " --WinVer=Windows_" + ver + " --Release=" + argsFormat(rel) + " --Language=" + listBox2.SelectedItem.ToString();
            timer2.Start();
            sel.topass = toPass;
            sel.language = listBox2.SelectedItem.ToString();
            sel.ShowDialog();
        }
    }
}
