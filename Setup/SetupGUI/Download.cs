using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SetupGUI
{
    public partial class Download : Form
    {
        public string mode { get; set; }
        public const string ISOURL = "https://github.com/eliasailenei/PortableISO/releases/download/V2/release.iso";
        string ISOBurner = Path.Combine(Environment.SystemDirectory, "isoburn.exe");
        string isoLoc;
        public Download()
        {
            InitializeComponent();
        }

        private void Download_Load(object sender, EventArgs e)
        {
            button1.Enabled = false;
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            showDiagofLicense();
            isoLocs(true);
            while (isoLoc == null)
            {
                MessageBox.Show("Please select a folder.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                isoLocs(true);
            }
            button2.Enabled = false;
            button3.Enabled = false;
            await downloadTask(ISOURL, isoLoc);
            Disk_Mode disk = new Disk_Mode();
            disk.option = mode;
            disk.isoFile = isoLoc;
            this.Close();
            disk.Show();
        }
        private void showDiagofLicense()
        {
            Activation active = new Activation();
            active.ShowDialog();
        }
        private void isoLocs(bool isFolder)
        {
            if (isFolder)
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.Description = "Select a folder where you want the image to be downloaded.";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    isoLoc = Path.Combine(dialog.SelectedPath, "PT_release.iso");
                }
            }
            else
            {
                OpenFileDialog file = new OpenFileDialog();
                if (file.ShowDialog() == DialogResult.OK)
                {
                    if (file.FileName.Contains(".iso") | file.FileName.Contains(".ISO"))
                    {
                        isoLoc = file.FileName;
                    }
                    else
                    {
                        MessageBox.Show("Program only supports .ISO/.iso", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        isoLocs(false);
                    }
                }

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            showDiagofLicense();
            isoLocs(false);
            while (isoLoc == null)
            {
                MessageBox.Show("Please select a file.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                isoLocs(false);
            }
            Disk_Mode disk = new Disk_Mode();
            disk.option = mode;
            disk.isoFile = isoLoc;
            this.Close();
            disk.Show();
        }
        public async Task downloadTask(string fileUrl, string savePath)
        {
            int prev = 0;
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.GetAsync(fileUrl, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();

                    long totalBytes = response.Content.Headers.ContentLength ?? -1;
                    long downloadedBytes = 0;
                    byte[] buffer = new byte[8192];

                    using (Stream contentStream = await response.Content.ReadAsStreamAsync())
                    using (FileStream fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        int bytesRead;
                        while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, bytesRead);
                            downloadedBytes += bytesRead;

                            if (totalBytes > 0)
                            {
                                int progressPercentage = (int)((downloadedBytes * 100) / totalBytes);
                                if (progressPercentage != prev)
                                {
                                    progressBar1.Invoke((MethodInvoker)delegate {
                                        progressBar1.Value = progressPercentage;
                                    });
                                    prev = progressPercentage;
                                }

                            }
                        }
                    }
                }
            }
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
    }
}
