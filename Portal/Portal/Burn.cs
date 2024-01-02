using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using Microsoft.VisualBasic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Reflection.Emit;
using System.ComponentModel;

namespace Portal
{
    public partial class Burn : Form
    {
        public const string ISOURL = "https://github.com/eliasailenei/PortableISO/releases/download/V2/release.iso";
        public string isoLoc;
        public int progress;
        public char num;
        public bool CDMode, USBMode;
        public Burn()
        {
            InitializeComponent();
        }

        private void Burn_Load(object sender, EventArgs e)
        {
            string warning = "It looks like there is no USB mass storage devices found. Here are some reasons:" +
     Environment.NewLine + Environment.NewLine +
     "1) The device is not connected/faulty." +
     Environment.NewLine + Environment.NewLine +
     "2) Program was ran without required permissions." +
     Environment.NewLine + Environment.NewLine +
     "3) No drivers or devices isn't recognized as a USB but instead as SATA or NVME storage (select Ignore and then other for tutorial without USB/CD)." +
     Environment.NewLine + Environment.NewLine +
     "4) You want to burn to CD/DVD instead." +
     Environment.NewLine + Environment.NewLine +
     "You have the following options:" +
     Environment.NewLine + Environment.NewLine +
     "Abort - Skip USB configuration and use CD instead" +
     Environment.NewLine + Environment.NewLine +
     "Retry - Scan again for USB's" +
     Environment.NewLine + Environment.NewLine +
     "Ignore - Leave ISO burning section";
            GetDisk();
            FormatList();
            if (listBox1.Items.Count <=0)
            {
               var userError = MessageBox.Show(warning,"ERROR", MessageBoxButtons.AbortRetryIgnore,MessageBoxIcon.Error);
                if (userError == DialogResult.Ignore)
                {
                    this.Close();
                }
                else if (userError == DialogResult.Abort)
                {
                    CDMode = true;
                    radioButton2.Select();
                    listBox1.Hide();
                    textBox1.Text = "To be confirmed with Windows Tool";

                }
                else if (userError == DialogResult.Retry)
                {
                    Burn_Load(sender, e);
                }
            } else
            {
                USBMode = true;
                radioButton1.Select();
            }

        }

        private void GetDisk()
        {
            using (Process process = new Process())
            {
                process.StartInfo.FileName = "powershell";
                process.StartInfo.Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"Get-Disk | Out-File -FilePath 'diskinfo.txt'\"";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                process.Start();
                process.WaitForExit();
            }
        }
        private string FormatList()
        {
            Regex keepOnlyDrive = new Regex(@"^\d");
            Regex keepOnlyUSB = new Regex(@".*USB.*");
            using (StreamReader original = new StreamReader("diskinfo.txt"))
            {
                string line;
                while ((line = original.ReadLine()) != null)
                {
                    if (keepOnlyDrive.IsMatch(line) && keepOnlyUSB.IsMatch(line))
                    {
                        listBox1.Items.Add(FormatString(line));
                    }
                }
            }
            return null;
        }
        static string FormatString(string input)
        {
            string[] words = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string result = string.Join(" | ", words);
            return result;
        }
        public double FileSize(string path)
        {
            int finalRes;
            FileInfo checkLength = new FileInfo(path);
            if (checkLength.Exists)
            {
                finalRes =  Convert.ToInt32(checkLength.Length / Math.Pow(10, 6))+1;
            } else
            {
                return -1;
            }
            if (finalRes > 31000)
            {
                MessageBox.Show("Image cannot be greater than 31GB due to FAT32 limitations");
                this.Close();
            } else
            {
                return finalRes;
            }
            return -1;
             
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            var response = MessageBox.Show("By selecting CD/DVD mode, you can't change your mind due to data protection. Do you want to change?", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (response == DialogResult.Yes)
            {
                CDMode = true;
                radioButton1.Enabled = false;
                listBox1.Hide();
                textBox1.Text = "To be confirmed with Windows Tool";

            }

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                PopulateTextBox(listBox1.SelectedItem.ToString());
            } else
            {
                MessageBox.Show("Select a real drive");
            }
            
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text == "Please select")
                {
                    MessageBox.Show("Please select a device first", "NOTE", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    isoLocs(true);
                    if (isoLoc == null)
                    {
                        MessageBox.Show("Please select a folder.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        isoLocs(true);
                    }
                    textBox2.Text = isoLoc;
                    DisAb(false);
                    label6.Text = "Current Process: Downloading";
                    await downloadTask(ISOURL, isoLoc);
                    if (progressBar1.Value == 100)
                    {
                        label5.Visible = true;
                    }
                    DisAb(true);
                    label6.Text = "Current Process: awaiting";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                this.Close();
                isoLoc = "PLEASE SELECT OR PROGRAM WILL EXIT";
            }

        }
        private void DisAb(bool isable)
        {
            button2.Enabled = isable;
            radioButton1.Enabled = isable;
            radioButton2.Enabled = isable;
            button1.Enabled = isable;
            button3.Enabled = isable;
            button4.Enabled = isable;
            listBox1.Enabled = isable;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text == "Please select")
                {
                    MessageBox.Show("Please select a device first", "NOTE", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    isoLocs(false);
                    textBox2.Text = isoLoc;
                }
            }
            catch (Exception ex){
                MessageBox.Show(ex.ToString());
                isoLoc = "PLEASE SELECT OR PROGRAM WILL EXIT";
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
            } else
            {
                OpenFileDialog file = new OpenFileDialog();
                if (file.ShowDialog() == DialogResult.OK)
                {
                    if (file.FileName.Contains(".iso") | file.FileName.Contains(".ISO"))
                    {
                        isoLoc = file.FileName;
                    } else
                    {
                        MessageBox.Show("Program only supports .ISO/.iso","ERROR",MessageBoxButtons.OK, MessageBoxIcon.Error);
                        isoLocs(false);
                    }
                }

            }


        }
        private void PopulateTextBox(string selected)
        {
            if (CDMode)
            {
                textBox1.Text = "To be confirmed with Windows Tool";
            } else
            {
                textBox1.Text = selected;
               num = selected[0];
                
            }
        }

        public async void button2_Click(object sender, EventArgs e)
        {
            label6.Text = "Current Process: Warming up";
            progressBar1.Value = 0;
            label7.Text = "Current Percentage: awaiting";
            string ISOBurner = Path.Combine(Environment.SystemDirectory, "isoburn.exe");
            string place;
            if (isoLoc != null)
            {
                if (CDMode)
                {
                    place = "1) Burn to a CD/DVD of your choice";
                }
                else
                {
                    place = "1) Erase and target device:" + num.ToString();
                }
                string confirm = "By clicking start, you want to :" + Environment.NewLine +
                     place + Environment.NewLine +
                    "2) Flash " + isoLoc + " onto it" + Environment.NewLine + Environment.NewLine +
                    "Do you agree?";
                var option = MessageBox.Show(confirm, "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                DisAb(false);
                if (option == DialogResult.Yes)
                {
                    if (CDMode)
                    {
                        try
                        {
                            Process.Start(ISOBurner, isoLoc);
                        }
                        catch
                        {
                            MessageBox.Show("Error, couldn't start burner. Check if " + ISOBurner + " comes with your system.");
                        }
                    }
                    else
                    {
                        string userInput = Interaction.InputBox("We need a drive letter for the image drive, please give us one that is not used", "Drive Letter Needed");
                        char letter = userInput.ToUpper()[0];
                        string userInput2 = Interaction.InputBox("We need a drive letter for the main drive, please give us one that is not used", "Drive Letter Needed");
                        char letters = userInput2.ToUpper()[0];
                        if (letter == letters)
                        {
                            MessageBox.Show("Can't have same drive letter.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            this.Close();
                        }
                        await Format(letter, letters);
                        await Extract(letter);
                    }
                }
                button4.Enabled = true;
                var restart = MessageBox.Show("Would you like to restart?", "RESTART?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (restart == DialogResult.Yes)
                {
                    Process.Start("shutdown.exe", "/t 0 /r");
                }
            } else
            {
                MessageBox.Show("No image selected!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
           
        }
        public async Task Format(char letter, char letters)
        {
            label6.Text = "Current Process: Formatting";
            await Task.Run(() =>
            {
                ProcessStartInfo com = new ProcessStartInfo();
                com.FileName = "cmd.exe";
                com.Arguments = "/C diskpart";
                com.CreateNoWindow = true;
                com.UseShellExecute = false;
                com.RedirectStandardInput = true;

                Process p = Process.Start(com);

                p.StandardInput.WriteLine("select disk " + num.ToString());
                p.StandardInput.WriteLine("clean");
                p.StandardInput.WriteLine("create partition primary size = " + FileSize(isoLoc).ToString());
                p.StandardInput.WriteLine("format FS=FAT32 label=Image quick");
                p.StandardInput.WriteLine("assign letter=" + letter.ToString());
                p.StandardInput.WriteLine("create partition primary");
                p.StandardInput.WriteLine("format FS=NTFS label=Storage quick");
                p.StandardInput.WriteLine("assign letter=" + letters.ToString());
                p.StandardInput.WriteLine("exit");
                p.WaitForExit();
            });
            }
        public async Task Extract(char letter)
        {
            label6.Text = "Current Process: Extracting";
            await Task.Run(() =>
            {
                using (Process process = new Process())
            {
                process.StartInfo.FileName = "7-Zip\\7z.exe";
                process.StartInfo.Arguments = @"x -o" + letter + ":"  + " " + isoLoc + " -bd -bsp1 -bso1";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;

                process.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                    {
                        Match match = Regex.Match(e.Data, @"\s(\d+)%");
                        if (match.Success)
                        {
                            int progress = int.Parse(match.Groups[1].Value);
                            if (progress ==99)
                            {
                                progress = 100;
                            }
                            progressBar1.BeginInvoke(new Action(() => progressBar1.Value = progress));
                            label7.BeginInvoke(new Action(() => label7.Text = "Current Percentage: " + progress.ToString()));
                        }
                    }
                };

                process.Start();
                process.BeginOutputReadLine();
                process.WaitForExit();

            }
            });
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public async Task  downloadTask(string fileUrl, string savePath)
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
                                    label7.Invoke((MethodInvoker)delegate {
                                        label7.Text = "Current Percentage:" + progressPercentage.ToString() + "%";
                                    });
                                    prev = progressPercentage;
                                }

                            }
                        }
                    }
                }
            }
        }
    }
}
