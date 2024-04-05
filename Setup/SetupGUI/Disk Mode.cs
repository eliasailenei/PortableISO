using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SetupGUI
{
    public partial class Disk_Mode : Form
    {
        public string option { get; set; }
        public string isoFile { get; set; }
        string isoPlace;
        string workingFolder = Path.Combine(Directory.GetCurrentDirectory(), "WorkingDir") ;
        string[] driverOptions = { "Nothing", "This PC Network Drivers Only", "Everything from PC" };
        string[] driveOptions;
        char[] letters;
        bool isUsingUSB, isUsingCD;
        char dLetter, iLetter;
        int driveNum, num;
        int driverIndex = 1;
        public string mode { get; set; }
        public Disk_Mode()
        {
            InitializeComponent();
        }

        private async void Disk_Mode_Load(object sender, EventArgs e)
        {
           await getLetters();
            if (option == "USB")
            {
                await FormatList(@".*USB.*");
                isUsingUSB = true;
            } else if (option == "DVD")
            {
                driveOptions = new string[1];
                driveOptions[0] = "DVD Drive";
            } else
            {
                await FormatList(@"^(?:(?!USB).)*$");
            }
            if (!isUsingUSB)
            {
                textBox1.Text = "N/A";
                textBox2.Text = "N/A";
                textBox1.Enabled = false;
                textBox2.Enabled = false;
            } 
            if (option == "HDD"){
                textBox2.Text = string.Empty;
                textBox2.Enabled = true;
            }
            comboBox2.Items.AddRange(driverOptions);
            comboBox2.SelectedIndex = driverIndex;
            comboBox1.Items.AddRange(driveOptions);
        }
        private async Task getLetters()
        {
            await Task.Run(() =>
            {
                DriveInfo[] drives = DriveInfo.GetDrives();
                HashSet<char> takenLetters = new HashSet<char>(drives.Select(d => char.ToUpper(d.Name[0])));
                letters = new char[takenLetters.Count];

                int pointer = 0;
                foreach (char item in takenLetters)
                {
                    letters[pointer] = item;
                    pointer++;
                }
            });

        }

        private async Task FormatList(string toLookFor)
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();

            await Task.Run(() =>
            {
                List<string> formattedOptions = new List<string>();
                Regex keepOnlyDrive = new Regex(@"^\d");
                Regex keepOnlyUSB = new Regex(toLookFor);

                using (StreamReader original = new StreamReader("diskinfo.txt"))
                {
                    string line;
                    while ((line = original.ReadLine()) != null)
                    {
                        if (keepOnlyDrive.IsMatch(line) && keepOnlyUSB.IsMatch(line))
                        {
                            formattedOptions.Add(FormatString(line));
                        }
                    }
                }

                driveOptions = formattedOptions.ToArray();
                tcs.SetResult(null);
            });

            await tcs.Task; 
        }

        private async Task getDrivers()
        {
            await Task.Run(() =>
            {
                try
                {
                    Process process = new Process();
                    process.StartInfo.FileName = "DISM.exe";
                    process.StartInfo.Arguments = $"/online /export-driver /destination:{workingFolder + "\\Drivers"} ";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();
                    process.WaitForExit();
                } catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                
            });
        }
        static string FormatString(string input)
        {
            string[] words = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string result = string.Join(" | ", words);
            return result;
        }
        private void msgShow()
        {
            MessageBox.Show("If you are planning on using WiFi, please make sure you have the image based on WinRE and NOT WinPE! WinPE DOES NOT support WiFi due to Microsoft inability to be consistent. The latest version of PortableISO is based on WinRE, so you don't need to worry if you have a fresh copy.", "Warning",MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void label4_Click(object sender, EventArgs e)
        {
            msgShow();
        }

        private void label5_Click(object sender, EventArgs e)
        {
            msgShow();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            msgShow();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text != "N/A")
            {
                string outs = textBox2.Text;
                outs = outs.ToUpper();
                if (outs.Length == 1 && char.IsLetter(outs[0]) && char.IsUpper(outs[0]))
                {
                    iLetter = char.ToUpper(outs[0]);
                    if (!letters.Contains(iLetter))
                    {
                        textBox2.Text = iLetter.ToString();
                    }
                    else
                    {
                        textBox2.Text = string.Empty;
                    }
                }
                else
                {
                    textBox2.Text = string.Empty;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Select a folder where you want the save the  working folder";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                workingFolder = Path.Combine(dialog.SelectedPath, "WorkingDir"); 
            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex > 0)
            {
                num = comboBox2.SelectedIndex;
                isoPlace = $"\"{workingFolder}\\out.iso\"";
            }
            else
            {
                num = comboBox2.SelectedIndex;
                isoPlace = $"\"{isoFile}\"";
            }
            if ( textBox2.Text != "" || textBox2.Text == "N/A")
            {
                if (textBox1.Text != "" || textBox1.Text == "N/A")
                {
                    if (comboBox1.SelectedItem != null)
                    {
                        if (Directory.Exists(workingFolder))
                        {
                            label9.Text = "Deleting past attempts";
                            progressBar1.Style = ProgressBarStyle.Marquee;
                            await Task.Run(() =>
                            {
                                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("DISM.exe", $"/unmount-WIM /MountDir:\"{workingFolder}\\WIM\" /discard")
                                {
                                    CreateNoWindow = true,
                                    UseShellExecute = false
                                }).WaitForExit();
                            });
                            progressBar1.Style = ProgressBarStyle.Continuous;
                            Directory.Delete(workingFolder, true);
                        }
                        Directory.CreateDirectory(workingFolder);
                        Directory.CreateDirectory(workingFolder + "\\Drivers");
                        Directory.CreateDirectory(workingFolder + "\\ISO");
                        Directory.CreateDirectory(workingFolder + "\\WIM");
                        if (comboBox2.SelectedIndex > 0)
                        {
                            label9.Text = "Getting all drivers";
                            progressBar1.Style = ProgressBarStyle.Marquee;
                             await getDrivers();
                            if (comboBox2.SelectedIndex == 1)
                            {
                                label9.Text = "Keeping only network drivers";
                                await keepOnlyNetwork();
                            }
                            progressBar1.Style = ProgressBarStyle.Continuous;
                            label9.Text = "Creating image";
                             await CreateISO();
                            progressBar1.Style = ProgressBarStyle.Continuous;
                        }
                        label9.Text = "Burning image";
                        if (option != "DVD")
                        {
                            progressBar1.Style = ProgressBarStyle.Marquee;
                            await Format(isoPlace);
                            await Extract(isoPlace);

                        } else
                        {
                                string ISOBurner = Path.Combine(Environment.SystemDirectory, "isoburn.exe");
                                Console.WriteLine("/q " + isoPlace);
                                Process.Start(ISOBurner, "/q " + isoPlace);
                        }
                        label9.Text = "Done";
                        progressBar1.Value = progressBar1.Maximum;
                        MessageBox.Show("All Done!");
                        Environment.Exit(0);
                    }
                    else
                    {
                        MessageBox.Show("Select a drive.");
                    }
                } else
                {
                    MessageBox.Show("Provide a letter");
                }
            } else
            {
                MessageBox.Show("Provide a letter");
            }
            
           
        }
        public async Task CreateISO()
        {
            var processes = new[]
            {
        new { FileName = "7-Zip\\7z.exe", Arguments = $@"x -o""{workingFolder}\ISO"" ""{isoFile}"" -bd -bsp1 -bso1" },
        new { FileName = "DISM.exe", Arguments = $"/Mount-Image /ImageFile:\"{workingFolder}\\ISO\\sources\\boot.wim\" /Index:1 /MountDir:\"{workingFolder}\\WIM\"" },
        new { FileName = "DISM.exe", Arguments = $"/Add-Driver /Image:\"{workingFolder}\\WIM\" /Driver:\"{workingFolder}\\Drivers\" /Recurse" },
        new { FileName = "DISM.exe", Arguments = $"/unmount-WIM /MountDir:\"{workingFolder}\\WIM\" /commit" },
        new { FileName = "oscdimg\\oscdimg.exe", Arguments = $"-m -o -u2 -udfver102 -bootdata:2#p0,e,b\"oscdimg\\etfsboot.com\"#pEF,e,b\"oscdimg\\efisys.bin\" \"{workingFolder}\\ISO\" \"{workingFolder}\\out.iso\"" }
    };

            await Task.Run(() =>
            {
                foreach (var processInfo in processes)
                {
                    using (Process process = new Process())
                    {
                        process.StartInfo.FileName = processInfo.FileName;
                        process.StartInfo.Arguments = processInfo.Arguments;
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.CreateNoWindow = true;
                        process.StartInfo.RedirectStandardOutput = true;

                        process.OutputDataReceived += (sender, e) =>
                        {
                            if (e.Data != null)
                            {
                                string pattern = @"\b\d+\b";

                                Match match = Regex.Match(e.Data, pattern);

                                if (match.Success)
                                {
                                    foreach (Match m in match.Groups)
                                    {
                                        try
                                        {
                                            if (int.Parse(m.Value) > 100)
                                            {
                                                progressBar1.BeginInvoke(new Action(() => progressBar1.Style = ProgressBarStyle.Marquee));
                                            }
                                            else if (int.Parse(m.Value) < 3)
                                            {
                                                progressBar1.BeginInvoke(new Action(() => progressBar1.Style = ProgressBarStyle.Marquee));
                                            }
                                            else
                                            {
                                                if (progressBar1.Style != ProgressBarStyle.Continuous)
                                                {
                                                    progressBar1.BeginInvoke(new Action(() => progressBar1.Style = ProgressBarStyle.Continuous));

                                                }
                                                progressBar1.BeginInvoke(new Action(() => progressBar1.Value = int.Parse(m.Value)));
                                            }
                                        } catch {
                                            progressBar1.BeginInvoke(new Action(() => progressBar1.Style = ProgressBarStyle.Marquee));
                                        }
                                        
                                        
                                    }
                                }
                                else
                                {

                                }
                            }
                        };

                        process.Start();
                        process.BeginOutputReadLine();
                        process.WaitForExit();
                    }
                }

                progressBar1.BeginInvoke(new Action(() => progressBar1.Value = 0));
            });
        }
        public async Task Format(string fileLoc)
        {
            await Task.Run(() =>
            {
                ProcessStartInfo com = new ProcessStartInfo();
                com.FileName = "cmd.exe";
                com.Arguments = "/C diskpart";
                com.CreateNoWindow = true;
                com.UseShellExecute = false;
                com.RedirectStandardInput = true;

                Process p = Process.Start(com);
                p.StandardInput.WriteLine("select disk " + driveNum.ToString());
                if (isUsingUSB)
                {
                    p.StandardInput.WriteLine("clean");
                } else
                {
                    Process pro = new Process();
                    pro.StartInfo.FileName = "powershell";
                    pro.StartInfo.Arguments = $"$Partition = Get-Partition -DiskNumber {driveNum} | Sort-Object -Property Size | Select-Object -Last 1;$NewSize = $Partition.Size - {FileSize()}MB;Resize-Partition -InputObject $Partition -Size $NewSize";
                    pro.StartInfo.UseShellExecute = false;
                    pro.StartInfo.CreateNoWindow = true;
                    pro.Start();
                    pro.WaitForExit();
                    
                }
                p.StandardInput.WriteLine("create partition primary size = " + FileSize().ToString());
                p.StandardInput.WriteLine("format FS=FAT32 label=Image quick");
                p.StandardInput.WriteLine("assign letter=" + iLetter.ToString());
                if (isUsingUSB)
                {
                    p.StandardInput.WriteLine("create partition primary");
                    p.StandardInput.WriteLine("format FS=NTFS label=Storage quick");
                    p.StandardInput.WriteLine("assign letter=" + dLetter.ToString());
                }
                p.StandardInput.WriteLine("exit");
                p.WaitForExit();
            });
        }
        public async Task Extract(string fileLoc)
        {
            await Task.Run(() =>
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = "7-Zip\\7z.exe";
                    process.StartInfo.Arguments = @"x -o" + iLetter + ":" + " " + fileLoc + " -bd -bsp1 -bso1";
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
                                if (progress == 99)
                                {
                                    progress = 100;
                                }
                                if (progressBar1.Style != ProgressBarStyle.Continuous)
                                {
                                    progressBar1.BeginInvoke(new Action(() => progressBar1.Style = ProgressBarStyle.Continuous));

                                }
                                progressBar1.BeginInvoke(new Action(() => progressBar1.Value = progress));
                            }
                        }
                    };

                    process.Start();
                    process.BeginOutputReadLine();
                    process.WaitForExit();

                }
            });
        }
        public double FileSize()
        {
            string path;
            if (num > 0)
            {
                path = $"{workingFolder}\\out.iso";
            }
            else
            {
                path = $"{isoFile}";
            }

            int finalRes;
            FileInfo checkLength = new FileInfo(path);
            if (checkLength.Exists)
            {
                finalRes = Convert.ToInt32(checkLength.Length / Math.Pow(10, 6)) + 1;
            }
            else
            {
                MessageBox.Show("failed");
                return -1;
            }
            if (finalRes > 31000)
            {
                MessageBox.Show("Image cannot be greater than 31GB due to FAT32 limitations");
                this.Close();
            }
            else
            {
                return finalRes;
            }
            MessageBox.Show("failed");
            return -1;

        }
        private async Task keepOnlyNetwork()
        {
            await Task.Run(() =>
            { 
            Process n = new Process();
                n.StartInfo.FileName = "powershell.exe";
                n.StartInfo.Arguments = "cd "+ workingFolder + "\\Drivers" + "; Get-ChildItem -Recurse -Filter *.inf | ForEach-Object { $folderPath = (Get-Item $_.FullName).Directory.FullName; if ((Get-Content $_.FullName | Select-String -Pattern 'Network')) { Write-Host \"Network driver found: $_.FullName\" } else { Write-Host \"Non-network driver, deleting folder: $_.FullName\"; Remove-Item -Path $folderPath -Recurse -Force } }";
                n.StartInfo.UseShellExecute = false;
                n.StartInfo.CreateNoWindow = true;
                n.Start();
                n.WaitForExit();
            });
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

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string item = comboBox1.SelectedItem.ToString();
            if (!string.IsNullOrEmpty(item))
            {
                if (int.TryParse(item[0].ToString(), out int num))
                {
                    driveNum = num;
                }
                else
                {
                    if (option != "DVD")
                    {
                        MessageBox.Show("This type of drive is not supported!");
                    }
                }
            }
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text != "N/A")
            {
                string outs = textBox1.Text;
                outs = outs.ToUpper();
                if (outs.Length == 1 && char.IsLetter(outs[0]) && char.IsUpper(outs[0]))
                {
                    dLetter = char.ToUpper(outs[0]);
                    if (!letters.Contains(dLetter))
                    {
                        textBox1.Text = dLetter.ToString();
                    }
                    else
                    {
                        textBox1.Text = string.Empty;
                    }
                }
                else
                {
                    textBox1.Text = string.Empty;
                }
            }
            
        }

        private void label8_Click(object sender, EventArgs e)
        {
            string message =
@"Don't understand? Don't worry, this will briefly explain everything.

Drive - Where you select what drive you want the image to be installed onto. For CD/DVD's, you can select your option once drivers are loaded.

Driver - You have a selection of 3 options:

  * Nothing - does not include any drivers (best for ethernet)

  * This PC Network Drivers Only - extracts any installed network drivers and places them into the image (best for WiFi)

  * Everything - extracts all drivers and places them into the image (best for laptops but not speed and storage)

Warning - warns the user that WinRE must be used for WiFi. See the GitHub repo for more.

Main Letter - A one-letter character to be for the main USB drive letter.

Image Letter - A one-letter character to be for the image USB drive letter.

NOTE: If selected USB, it gets split into two partitions as you don't need the whole drive as an image drive.

NOTE 2: Users who chose a hard drive don't need to use main/image letter. Their data IS NOT lost neither.

NOTE 3: Everything will break your display drivers, keep drivers as default for best experience.";

            MessageBox.Show(message, "Explanation");

        }

    }
}
