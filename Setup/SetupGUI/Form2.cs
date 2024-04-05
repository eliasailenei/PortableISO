using System;
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
            // the reason why we delete everything is usually because the other files are corrupted as user cancels the extraction midway

            // this is also the reason why this program is flagged as malware as it deletes and downloads new files as soon as its open
            if (!Directory.Exists("7-Zip") || !Directory.Exists("oscdimg") || !File.Exists("AxInterop.WMPLib.dll") || !File.Exists("Interop.WMPLib.dll"))
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
                    if (File.Exists("AxInterop.WMPLib.dll"))
                    {
                        File.Delete("AxInterop.WMPLib.dll");
                    }
                    if (File.Exists("Interop.WMPLib.dll"))
                    {
                        File.Delete("Interop.WMPLib.dll");
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
            // start install
            ModeOfInstall mode = new ModeOfInstall();
            this.Hide();
           mode.Show();
          
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // legacy image creator
            CreateImagecs createImagecs = new CreateImagecs();
            createImagecs.Show();   
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // close all windows algorithm
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
