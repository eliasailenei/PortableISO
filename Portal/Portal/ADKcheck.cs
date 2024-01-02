using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Microsoft.VisualBasic;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;

namespace Portal
{
    public partial class ADKcheck : Form
    {
        public string ADKLocation = "C:\\Program Files (x86)\\Windows Kits\\10\\Assessment and Deployment Kit";
        public string DeploymentTools;
        public string workingFolder = "C:\\";
        public string exportFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "PT_modify.iso");
        public string exportFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        public string driverFolder = null;
        private StringBuilder packagePathBuilder = new StringBuilder();
        public string packagePath;
        public bool isDriverPresent;

        public ADKcheck()
        {
            InitializeComponent();
        }

        private void ADKcheck_Load(object sender, EventArgs e)
        {
            CheckADK();
            DeploymentTools = ADKLocation + "\\Deployment Tools";
            textBox1.Text = workingFolder;
            textBox2.Text = exportFolder;
            textBox3.Text = "N/A";
            richTextBox1.AppendText("- Defaults loaded\n");
            richTextBox1.AppendText("- Drivers are not to be installed\n");
            richTextBox1.AppendText("- Image will be saved at: " + exportFolder + "\n");

            string[] ocPackages = {
        "WinPE-WMI",
        "WinPE-NetFx",
        "WinPE-Scripting",
        "WinPE-PowerShell",
        "WinPE-DismCmdlets",
    };

            foreach (string ocPackage in ocPackages)
            {
                string packagePath = $"/PackagePath:\"{ADKLocation}\\Windows Preinstallation Environment\\amd64\\WinPE_OCs\\{ocPackage}.cab\" ";
                string enUsPackagePath = $"/PackagePath:\"{ADKLocation}\\Windows Preinstallation Environment\\amd64\\WinPE_OCs\\en-us\\{ocPackage}_en-us.cab\" ";

                packagePath += enUsPackagePath;

                packagePathBuilder.Append(packagePath);
            }

            packagePath = packagePathBuilder.ToString();
        }



        public void CheckADK()
        {
            if (!ADKExist())
            {
                string contain = " ";
                var userError = MessageBox.Show("No sign of ADK in local path! Click Abort to exit, Retry to download and install ADK or Ignore and specify where the ADK is if its on a different drive/folder.", "ERROR", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error);
                if (userError == DialogResult.Ignore)
                {
                    FolderBrowserDialog dialog = new FolderBrowserDialog();
                    dialog.Description = "Select the folder where Assessment and Deployment Kit are located. For example: D:\\Extra\\Windows Kits\\10\\Assessment and Deployment Kit\\";
                    dialog.ShowNewFolderButton = false;
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        contain = Path.Combine(dialog.SelectedPath);
                    }
                    else
                    {
                        this.Close();
                    }
                    if (contain.Contains("Assessment and Deployment Kit"))
                    {
                        ADKLocation = contain;
                    }
                    else
                    {
                        MessageBox.Show("Could not find Assessment and Deployment Kit folder, try again.", "FOLDER ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    CheckADK();

                }
                else if (userError == DialogResult.Abort)
                {
                    this.Close();

                }
                else if (userError == DialogResult.Retry)
                {
                    this.Size = new System.Drawing.Size(575, 120);
                    button1.Visible = false;
                    GetADK();
                }
            }
        }

        public async void GetADK()
        {
            try
            {
                await Install("https://go.microsoft.com/fwlink/?linkid=2120254", "ADK.exe");
                await Install("https://go.microsoft.com/fwlink/?linkid=2120253", "WPE.exe");
                var restart = MessageBox.Show("Would you like to restart now?", "RESTART?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (restart == DialogResult.Yes)
                {
                    Process.Start("shutdown.exe", "/t 0 /r");
                }
                
            } catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "INSTALL ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            this.Close();
        }

        async Task Install(string uri, string file)
        {
            await downloadTask(uri, file);

            Process process = new Process();
            process.StartInfo.FileName = file;

            try
            {
                process.Start();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
               MessageBox.Show(ex.ToString(), "INSTALL ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public bool ADKExist()
        {
            return Directory.Exists(ADKLocation);
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

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Select the folder where you want the program to keep its extractions.";
            dialog.ShowNewFolderButton = false;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                workingFolder = Path.Combine(dialog.SelectedPath);
                textBox1.Text = workingFolder;
                richTextBox1.AppendText("- Working Folder path updated to:" + workingFolder + "\n");
            }
            else if (dialog.SelectedPath == null)
            {
                MessageBox.Show("No changes made.", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("No changes made.", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Select the folder where you want to keep the ISO.";
            dialog.ShowNewFolderButton = false;
            string userInput = Interaction.InputBox("We need a name for the ISO (.iso at the end is not acceptable)", "ISO name needed");
            if (userInput == null)
            {
                userInput = "PT_modify.iso";
            }
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                exportFolder = dialog.SelectedPath;
                exportFile = dialog.SelectedPath + "\\ISOExtra\\" + userInput + ".iso";
                textBox2.Text = exportFolder;
                richTextBox1.AppendText("- Location of final image path updated to:" + exportFolder + "\n");
                MessageBox.Show("Note: ISO is located at " + dialog.SelectedPath);
            }
            else if (dialog.SelectedPath == null)
            {
                MessageBox.Show("No changes made.", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("No changes made.", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Select the folder where you keep the drivers.";
            dialog.ShowNewFolderButton = false;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                driverFolder = Path.Combine(dialog.SelectedPath);
                textBox3.Text = driverFolder;
                richTextBox1.AppendText("- Driver Folder path updated to:" + driverFolder + "\n");
                isDriverPresent = true;
                richTextBox1.AppendText("- Drivers are to be installed\n");
            }
            else if (dialog.SelectedPath == null)
            {
                MessageBox.Show("No changes made.", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Information);
                isDriverPresent = false;
                driverFolder = "N/A";
                richTextBox1.AppendText("- Driver option reverted\n");
            }
            else
            {
                MessageBox.Show("No changes made.", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Information);
                isDriverPresent = false;
                driverFolder = "N/A";
                richTextBox1.AppendText("- Driver option reverted\n");

            }
        }

       

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            string[] allItems = checkedListBox1.Items.Cast<string>().ToArray();
            int index = e.Index;
            
            if (e.NewValue == CheckState.Checked)
            {
                if (!packagePathBuilder.ToString().Contains(allItems[index]))
                {
                    packagePathBuilder.Append($"/PackagePath:\"{ADKLocation}\\Windows Preinstallation Environment\\amd64\\WinPE_OCs\\{allItems[index]}.cab\" ");
                    packagePathBuilder.Append($"/PackagePath:\"{ADKLocation}\\Windows Preinstallation Environment\\amd64\\WinPE_OCs\\en-us\\{allItems[index]}_en-us.cab\" ");
                    richTextBox1.AppendText($"- {allItems[index]} is to be added\n");
                }
            }
            else
            {
                string itemToRemove = $"/PackagePath:\"{ADKLocation}\\Windows Preinstallation Environment\\amd64\\WinPE_OCs\\{allItems[index]}.cab\" " +
                          $"/PackagePath:\"{ADKLocation}\\Windows Preinstallation Environment\\amd64\\WinPE_OCs\\en-us\\{allItems[index]}_en-us.cab\" ";
                packagePathBuilder.Replace(itemToRemove, "");
                richTextBox1.AppendText($"- {allItems[index]} is to be removed\n");
            }
            packagePath = string.Join(" ", packagePathBuilder.ToString().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
        }
        private async void button5_Click(object sender, EventArgs e)
        {
            statusButton(false);
            richTextBox1.Clear();
            richTextBox1.AppendText("- Starting ISO creation! Do not close!\n");
            richTextBox1.AppendText("- Cleaning up any failed attempts\n");
            await runProcess(DeploymentTools + "\\amd64\\DISM\\imagex.exe", "/cleanup");
            await runProcess(Environment.SystemDirectory + "\\dism.exe", "/unmount-WIM /MountDir:\"" + workingFolder + "\\Mount\"" + " /discard");
            try
            {
                string[] folders = { workingFolder + "\\Mount", workingFolder + "\\WinPE_x64" };
                foreach (string folder in folders)
                {

                    if (Directory.Exists(folder))
                    {
                        Directory.Delete(folder, true);
                    }
                    Directory.CreateDirectory(folder);
                    if (Directory.Exists(exportFolder + "\\ISOExtra"))
                    {
                        Directory.Delete(exportFolder + "\\ISOExtra", true);
                    }
                }
            }
            catch (DirectoryNotFoundException ex)
            {

                richTextBox1.AppendText("- Error:" + ex.ToString() + "\n");
            }
            catch (Exception ex)
            {
                richTextBox1.AppendText("- Error:" + ex.ToString() + "\n");
            }
            richTextBox1.AppendText("- Mounting image\n");
            await runProcess(Environment.SystemDirectory + "\\esentutl.exe", "/y \"" + ADKLocation + "\\Windows Preinstallation Environment\\amd64\\en-us\\winpe.wim\" /d \"" + workingFolder + "\\WinPE_x64" + "\\winpe.wim\" /o");
            await runProcess(DeploymentTools + "\\amd64\\DISM\\dism.exe", "/Mount-Wim /WimFile:\"" + workingFolder + "\\WinPE_x64\\winpe.wim\" /index:1 /MountDir:\"" + workingFolder + "\\Mount\"");
            richTextBox1.AppendText("- Adding features to the image (don't panic if frozen or see errors)\n");
            await runProcess(DeploymentTools + "\\amd64\\DISM\\dism.exe", "/image:\"" + workingFolder + "\\Mount\"" + " /add-package " + packagePath);
            richTextBox1.AppendText("- Waiting for user\n");
            MessageBox.Show("You are nearly done! Please add any required files to the image, a window will appear when you click OK.", "NOTE", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            await runProcess(Environment.SystemDirectory + "\\explorer.exe", workingFolder + "\\Mount");
            var option = MessageBox.Show("If you are ready to mount, click Yes otherwise click No (unmounts the image)", "CONFIRM", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (option == DialogResult.No)
            {
                richTextBox1.AppendText("- Discarding image\n");
                await runProcess(DeploymentTools + "\\amd64\\DISM\\imagex.exe", "/cleanup");
                await runProcess(Environment.SystemDirectory + "\\dism.exe", "/unmount-WIM /MountDir:\"" + workingFolder + "\\Mount\"" + " /discard");
                try
                {
                    Directory.Delete(workingFolder + "\\Mount", true);
                    Directory.Delete(workingFolder + "\\WinPE_x64", true);
                }
                catch (DirectoryNotFoundException ex)
                {

                    richTextBox1.AppendText("- Error:" + ex.ToString() + "\n");
                }
                this.Close();
            }
            if (isDriverPresent)
            {
                richTextBox1.AppendText("- Adding your drivers\n");
                await runProcess(Environment.SystemDirectory + "\\dism.exe", "/Image:\"" + workingFolder + "\\Mount\"" + " /Add-Driver /Driver:\"" + driverFolder+ "\" /Recurse");
            } else
            {
                richTextBox1.AppendText("- Driver installation has been opted out\n");
            }
            richTextBox1.AppendText("- Committing image\n");
            await runProcess(Environment.SystemDirectory + "\\dism.exe", "/unmount-WIM /MountDir:\"" + workingFolder + "\\Mount\"" + " /commit");
            await runProcess(DeploymentTools + "\\amd64\\DISM\\imagex.exe", "/cleanup");
            richTextBox1.AppendText("- Creating ISO image (don't panic if frozen)\n");
            string s1 = "@echo off" + Environment.NewLine +
                             "call \"" + DeploymentTools + "\\DandISetEnv.bat\"" + Environment.NewLine +
                             "cd /d \"" + ADKLocation + "\\Windows Preinstallation Environment\" && copype amd64 \"" + exportFolder + "\\ISOExtra\"";
            string s2 = "@echo off" + Environment.NewLine +
                             "call \"" + DeploymentTools + "\\DandISetEnv.bat\"" + Environment.NewLine +
                             "cd /d \"" + ADKLocation + "\\Windows Preinstallation Environment\" && MakeWinPEMedia /ISO \"" + exportFolder + "\\ISOExtra\" " + "\"" + exportFile;
            File.WriteAllText("s1.bat", s1);
            File.WriteAllText("s2.bat", s2);
            await runProcess("cmd.exe", "/c s1.bat");
            File.Delete(exportFolder + "\\ISOExtra\\media\\sources\\boot.wim");
            await runProcess(Environment.SystemDirectory + "\\esentutl.exe", "/y \"" + workingFolder + "\\WinPE_x64\\winpe.wim\" /d \"" + exportFolder + "\\ISOExtra\\media\\sources\\boot.wim\" /o");
            await runProcess("cmd.exe", "/c s2.bat");
            richTextBox1.AppendText("- Image has been created!\n");
            statusButton(true);
        }

        public async Task runProcess(string fileName, string argu)
        {
            using (Process process = new Process())
            {
                process.StartInfo.FileName = fileName;
                process.StartInfo.Arguments = argu;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.CreateNoWindow = true; 
                process.OutputDataReceived += (s, outputEventArgs) =>
                {
                    if (outputEventArgs.Data != null)
                    {
                        Task.Run(() =>
                        {
                            richTextBox2.BeginInvoke(new Action(() => richTextBox2.Clear()));
                            richTextBox2.BeginInvoke(new Action(() => richTextBox2.AppendText(outputEventArgs.Data)));
                        });
                    }
                };
                process.Start();
                process.BeginOutputReadLine();
                using (StreamWriter sw = process.StandardInput)
                {
                    if (sw.BaseStream.CanWrite)
                    {
                        sw.WriteLine("Y");
                        sw.Close(); 
                    }
                }
                await Task.Run(() =>
                {
                    process.WaitForExit();
                });
            }
        }



        public void statusButton(bool status)
        {
            button2.Enabled = status;
            button3.Enabled = status;
            button4.Enabled = status;
            button5.Enabled = status;
            button6.Enabled = status;
            checkedListBox1.Enabled = status;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
