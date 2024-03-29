﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Compression;
namespace SetupGUI
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            if (!Directory.Exists("7-Zip") || !Directory.Exists("oscdimg"))
            {
                try
                {
                    if (Directory.Exists("7-Zip"))
                    {
                        Directory.Delete("7-Zip", true);
                    }

                    if (Directory.Exists("oscdimg"))
                    {
                        Directory.Delete("oscdimg", true);
                    }

                    using (var client = new WebClient())
                    {
                        client.DownloadFile("https://github.com/eliasailenei/PortableISO/releases/download/Portal/misc.zip", "misc.zip");
                    }

                    ZipFile.ExtractToDirectory("misc.zip", Directory.GetCurrentDirectory());

                    File.Delete("misc.zip");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }

            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ModeOfInstall mode = new ModeOfInstall();
            this.Hide();
           mode.Show();
          
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CreateImagecs createImagecs = new CreateImagecs();
            createImagecs.Show();   
        }

        private void button4_Click(object sender, EventArgs e)
        {
            List<Form> formsToClose = new List<Form>();
            foreach (Form form in Application.OpenForms)
            {
                formsToClose.Add(form);
            }
            foreach (Form form in formsToClose)
            {
                form.Close();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/eliasailenei/EXMLE/releases/download/Latest/Release.zip");

        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}
