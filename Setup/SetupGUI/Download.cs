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
        public const string ISOURL = "https://github.com/eliasailenei/PortableISO/releases/download/V2/release.iso"; // perma link will never change
        string isoLoc;
        public Download()
        {
            InitializeComponent(); // loads UI components
        }

        private void Download_Load(object sender, EventArgs e)
        {
            button1.Enabled = false; // user must first get the image
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            try
            {
                isoLocs(true);
                while (isoLoc == null)
                {
                    MessageBox.Show("Please select a folder.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    isoLocs(true);
                }
                button2.Enabled = false;
                button3.Enabled = false;
                await downloadTask(ISOURL, isoLoc);
                try
                {
                    Activation active = new Activation();
                    active.ShowDialog();
                }
                catch (Exception ex)
                {
                    var message = MessageBox.Show("It looks like Windows Media Player failed to launch. This is mainly caused by computers with the N or KN versions. Microsoft has already given a hotfix, would you like to apply? You will need to restart PC.", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    if (message == DialogResult.Yes)
                    {
                        Process.Start("cmd.exe", "/c DISM /Online /Add-Capability /CapabilityName:Media.MediaFeaturePack~~~~0.0.1.0");
                    }
                    Environment.Exit(0);
                }
                Disk_Mode disk = new Disk_Mode();
                disk.option = mode;
                disk.isoFile = isoLoc;
                this.Close();
                disk.Show();
            } catch (Exception ex) {
                MessageBox.Show(ex.Message + " Try again later.");
            }
           
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
            isoLocs(false);
            while (isoLoc == null)
            {
                MessageBox.Show("Please select a file.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                isoLocs(false);
            }
            Disk_Mode disk = new Disk_Mode();
            disk.option = mode;
            disk.isoFile = isoLoc;
            try
            {
                Activation active = new Activation();
                active.ShowDialog();
            }
            catch (Exception ex)
            {
                var message = MessageBox.Show("It looks like Windows Media Player failed to launch. This is mainly caused by computers with the N or KN versions. Microsoft has already given a hotfix, would you like to apply? You will need to restart PC.", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                if (message == DialogResult.Yes)
                {
                    Process.Start("cmd.exe", "/c DISM /Online /Add-Capability /CapabilityName:Media.MediaFeaturePack~~~~0.0.1.0");
                }
                Environment.Exit(0);
            }
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
                    using (FileStream fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None)) // writing and reading from a file
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
