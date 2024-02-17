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
using System.Text.RegularExpressions;
using System.IO;
using System.IO.Compression;
using System.Net;
using CustomConfig;
namespace mainUI
{
    
    public partial class DiskSelect : Form
    {
        private Rectangle  labl1, labl2, labl3, buttn1, buttn2, buttn3, buttn4, buttn5, buttn6, buttn7, buttn8, labl8, labl5, buttn9, bttn10;
        public bool isMBR;
        public SQLCheck sql;
        public DriveLetters drive = new DriveLetters();
        public string systemLetter = "C";
        private Size original;
        string[] lists;
        int i;
        int diskNum = 0;
        public DiskSelect(SQLCheck newSQL)
        {
            sql = newSQL;
            InitializeComponent();
            this.Resize += DiskSelect_resiz;
            original = this.Size;
            labl1 = new Rectangle(label1.Location, label1.Size);
            labl2 = new Rectangle(label2.Location, label2.Size);
           labl3 = new Rectangle(label3.Location, label3.Size);
 buttn1 = new Rectangle(button1.Location, button1.Size);
             buttn2 = new Rectangle(button2.Location, button2.Size);
             buttn3 = new Rectangle(button3.Location, button3.Size);
             buttn4 = new Rectangle(button4.Location, button4.Size);
             buttn5 = new Rectangle(button5.Location, button5.Size);
             buttn6 = new Rectangle(button6.Location, button6.Size);
             buttn7 = new Rectangle(button7.Location, button7.Size);
            buttn9 = new Rectangle(button9.Location, button9.Size);
            buttn8 = new Rectangle(button8.Location, button8.Size);
            bttn10 = new Rectangle(button10.Location, button10.Size);
            labl8 = new Rectangle(label8.Location, label8.Size);
            labl5 = new Rectangle(label5.Location, label5.Size);
        }
        private void DiskSelect_resiz(object sender, EventArgs e)
        {
            resizeControl(buttn8, button8);
            resizeControl(buttn7, button7);
            resizeControl(buttn6, button6);
            resizeControl(buttn5, button5);
            resizeControl(buttn4, button4);
            resizeControl(buttn3, button3);
            resizeControl(buttn2, button2);
            resizeControl(buttn1, button1);
            resizeControl(labl8, label8);
            resizeControl(labl3, label3);
            resizeControl(labl2, label2);
            resizeControl(labl1, label1);
            resizeControl(labl5, label5);
            resizeControl(buttn9, button9);
            resizeControl(bttn10, button10);

        }
        private void resizeControl(Rectangle r, Control c)
        {
            float xRatio = (float)this.Width / original.Width;
            float yRatio = (float)this.Height / original.Height;

            int newX = (int)(r.X * xRatio);
            int newY = (int)(r.Y * yRatio);

            int newWidth = (int)(r.Width * xRatio);
            int newHeight = (int)(r.Height * yRatio);

            c.Location = new Point(newX, newY);
            c.Size = new Size(newWidth, newHeight);
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
                this.Hide();
            }
            Opacity -= .4;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            DialogResult iquit = MessageBox.Show("Do you wish to erase drive " + diskNum + "?", "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (iquit == DialogResult.Yes)
            {
                Cursor.Current = Cursors.WaitCursor;
                DriveMake();
                timer2.Start();
                if (Directory.Exists(drive.TLetter.ToString() + "contin"))
                {
                    Directory.Delete(drive.TLetter.ToString() + "contin", true);
                }
                Directory.CreateDirectory(drive.TLetter.ToString() + "contin");
                using (var client = new WebClient())
                {
                    client.DownloadFile("https://github.com/eliasailenei/PortableISO/releases/download/Contin/Release.zip", drive.TLetter.ToString() + "contin\\contin.zip");
                }
                ZipFile.ExtractToDirectory(drive.TLetter.ToString() + "contin\\contin.zip", drive.TLetter.ToString() + "contin");
                File.Delete(drive.TLetter.ToString() + "contin\\contin.zip");
                Process.Start(drive.TLetter.ToString() + "contin\\contin.exe", diskNum.ToString());
                this.Hide();
            }
            else
            {
                Process.Start("cmd.exe");
            }
                
        }
        private bool isFailedSetup()
        {
            if (Directory.Exists(drive.TLetter.ToString() + "contin") || Directory.Exists("D:\\contin"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private async void DiskSelect_Load(object sender, EventArgs e)
        {
            button10.Text = "Dual Boot";
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            label3.Visible = false;
            button9.Text = "Change to MBR";
            try
            {
                File.Delete("diskinfo.txt");
                File.Delete("info.txt");
            }
            catch (Exception ex)
            {
            }
            if (isFailedSetup())
            {
                var errorBox = MessageBox.Show("It looks like the attempt to boot to Windows has failed. This is mainly due to your computer having a BIOS and not a UEFI system which mainly occurs on old PC's\\VM's. If you have the option to change to a EFI/UEFI system in the BIOS or just want to close your computer click Abort and be shut down. If you want to try to use MBR which might the fix the problem click Retry (only limited to 4TB per drive). To continue using a GPT drive, click Ignore.", "CRITICAL ERROR", MessageBoxButtons.AbortRetryIgnore,MessageBoxIcon.Error);
                if (errorBox == DialogResult.Abort)
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = @"wpeutil.exe",
                        Arguments = $"shutdown",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    });
                } else if (errorBox == DialogResult.Retry)
                {
                    isMBR = true;
                    button9.Text = "Switch to GPT";
                } else
                {
                    MessageBox.Show("No changes made");
                }
            }
            if (await drive.LetterCollision())
            {
                DualBoot showDiag = new DualBoot();
                int centerX = (Screen.PrimaryScreen.Bounds.Width - showDiag.Width) / 2;
                int centerY = (Screen.PrimaryScreen.Bounds.Height - showDiag.Height) / 2;
                showDiag.Location = new Point(centerX, centerY);
                this.Controls.Add(showDiag);
                showDiag.drive = drive;
                showDiag.intent = true;
                showDiag.BringToFront();
                showDiag.Show();
                showDiag.InteractionComplete += (s, args) =>
                {
                    showDiag.Hide();
                };
            }
            if (sql.xmlStatus() && !String.IsNullOrEmpty(sql.diskNumber))
            {
                MessageBox.Show(sql.diskNumber);
            } else
            {
                MessageBox.Show("You wasted your time idiot");
            }
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            button1.Text = "Please wait...";
            button2.Text = "Please wait...";
            button3.Text = "Please wait...";
            GetDisk();
            FormatList();
            PutToArray();
            ButtonChange();
            if (button1.Text == "Please wait...")
            {
                string errorMessage = "It appears that no drives were found!\n\nPossible reasons for this issue:\n\n";

                errorMessage += "Reason 1: Drive Connection Issue\n";
                errorMessage += "-----------------------------\n";
                errorMessage += "1. Ensure that your drive is properly connected to a power source, such as a PSU.\n";
                errorMessage += "2. Check that all connections are securely plugged into the motherboard.\n";
                errorMessage += "3. If you're using an M.2 drive, make sure to enable it in your BIOS, especially if it's an older generation.\n";
                errorMessage += "4. Verify that all drives are ENABLED in the motherboard settings.\n";
                errorMessage += "5. Avoid using unsupported drives like pen drives; it's recommended to use HDD or, even better, an SSD.\n\n";

                errorMessage += "Reason 2: Intel Rapid Storage Technology or Secure Boot\n";
                errorMessage += "---------------------------------------------------\n";
                errorMessage += "I've learned this the hard way.... If you're using an Intel CPU, especially a new one, you might have Intel Rapid Storage Technology (RST) enabled. This is common in gaming products like ROG motherboards, laptops, and high-end motherboards.\n";
                errorMessage += "While RST significantly improves PC speed, WinPE doesn't support it, and I haven't implemented it due to its limited usage. To resolve this:\n";
                errorMessage += "- Access your motherboard's UEFI settings.\n";
                errorMessage += "- Search for options related to RST or Intel Rapid Storage Technology and disable it.\n";
                errorMessage += "- Additionally, Secure Boot might hinder disk access. Disable Secure Boot after ensuring you've backed up BitLocker keys.\n\n";

                errorMessage += "Please note that while code issues could also be a cause, they're relatively rare as I've thoroughly tested it.";

                MessageBox.Show(errorMessage, "No drives!", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        private void FormatList()
        {
            Regex keepOnlyDrive = new Regex(@"^\d");
            using (StreamReader original = new StreamReader("diskinfo.txt"))
            using (StreamWriter good = new StreamWriter("info.txt"))
            {
                string line;
                while ((line = original.ReadLine()) != null)
                {
                    if (keepOnlyDrive.IsMatch(line))
                    {
                        good.WriteLine(line);
                    }
                }
            }
        }
        private void PutToArray()
        {
            string[] allDisks = File.ReadAllLines("info.txt");
            QuickSortAlgorithm(allDisks, 0, allDisks.Length - 1);
            lists = allDisks;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            CheckWhat(sender, e);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (!isMBR)
            {
                var bx = MessageBox.Show("Are you sure you want to use a MBR drive?", "CONFIRM",MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (bx == DialogResult.Yes)
                {
                    isMBR = true;
                    button9.Text = "Change to GPT";
                }
            } else if (isMBR)
            {
                var bx = MessageBox.Show("Are you sure you want to use a GPT drive?", "CONFIRM", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (bx == DialogResult.Yes)
                {
                    isMBR = false;
                    button9.Text = "Change to MBR";
                }
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            DualBoot showDiag = new DualBoot();
            int centerX = (Screen.PrimaryScreen.Bounds.Width - showDiag.Width) / 2;
            int centerY = (Screen.PrimaryScreen.Bounds.Height - showDiag.Height) / 2;
            showDiag.Location = new Point(centerX, centerY);
            this.Controls.Add(showDiag);
            showDiag.drive = drive;
            showDiag.intent = false;
            showDiag.BringToFront();
            showDiag.Show();
            showDiag.InteractionComplete += (s, args) =>
            {
                showDiag.Hide();
            };
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
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

        private void button2_Click(object sender, EventArgs e)
        {
            CheckWhat(sender, e);
        }
        private void button3_Click(object sender, EventArgs e)
        {
            CheckWhat(sender, e);
        }
        private void CheckWhat(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            string toEraze;
            toEraze = (clickedButton.Text);
            char diskNums = ' ';
            try
            {
                 diskNums= toEraze[0];
            } catch (IndexOutOfRangeException)
            {
                MessageBox.Show("Box is empty");
            }
            File.WriteAllText("dis.txt", diskNums.ToString());
            using (StreamReader reader = new StreamReader("dis.txt"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (int.TryParse(line, out diskNum))
                    {

                    }

                }
            }
            label3.Visible = true;
            label3.Text = "Disk " + diskNum + " is chosen.";

            if (lists == null)
            {
                MessageBox.Show(diskNum.ToString());
            }
        }
        private void DriveMake()
        {
            string scriptContent = " ";
            string filePath = @"X:\clear.txt";
            if (!isMBR)
            {
                scriptContent = "select disk " + diskNum + Environment.NewLine +
                                   "clean" + Environment.NewLine +
                                   "convert gpt" + Environment.NewLine +
                                   "create partition primary" + Environment.NewLine +
                                   "format fs=ntfs label=System quick" + Environment.NewLine +
                                   "assign letter=" + drive.CLetter.ToString() + Environment.NewLine +
                                   "shrink desired=20000 " + Environment.NewLine +
                                   "create partition primary size=20000" + Environment.NewLine +
                                   "format fs=ntfs label=Temp quick" + Environment.NewLine +
                                   "assign letter=" + drive.TLetter.ToString() + Environment.NewLine +
                                   "shrink desired=512" + Environment.NewLine +
                                   "create partition efi size=512" + Environment.NewLine +
                                   "format quick fs=fat32 label=EFI" + Environment.NewLine;
            } else if (isMBR)
            {
                       scriptContent = "select disk " + diskNum + Environment.NewLine +
                       "clean" + Environment.NewLine +
                       "convert mbr" + Environment.NewLine +
                       "create partition primary" + Environment.NewLine +
                       "format fs=ntfs label=System quick" + Environment.NewLine +
                       "assign letter=" + drive.CLetter.ToString() + Environment.NewLine +
                       "shrink desired=20000" + Environment.NewLine +
                       "create partition primary" + Environment.NewLine +
                       "assign letter=" + drive.TLetter.ToString() + Environment.NewLine +
                       "assign letter=T" + Environment.NewLine;

            }



            File.WriteAllText(filePath, scriptContent);

            ProcessStartInfo com = new ProcessStartInfo();
            com.FileName = "cmd.exe";
            com.Arguments = "/C diskpart /s X:\\clear.txt";
            com.CreateNoWindow = true;
            com.UseShellExecute = false;
            com.RedirectStandardInput = true;
            com.RedirectStandardOutput = true;

            Process p = Process.Start(com);

            p.StandardInput.WriteLine("exit");

            p.WaitForExit();

            string output = p.StandardOutput.ReadToEnd();
        }

        static void QuickSortAlgorithm(string[] array, int left, int right)
        {
            if (left < right)
            {
                int pivotIndex = Partition(array, left, right);

                QuickSortAlgorithm(array, left, pivotIndex - 1);
                QuickSortAlgorithm(array, pivotIndex + 1, right);
            }
        }
        static int Partition(string[] array, int left, int right)
        {
            string pivotValue = array[right];
            int i = left - 1;

            for (int j = left; j < right; j++)
            {
                if (ExtractNumber(array[j]) <= ExtractNumber(pivotValue))
                {
                    i++;
                    Swap(array, i, j);
                }
            }

            Swap(array, i + 1, right);
            return i + 1;
        }
        static int ExtractNumber(string input)
        {
            string numericPart = new string(input.TakeWhile(char.IsDigit).ToArray());
            return int.Parse(numericPart);
        }

        static void Swap(string[] array, int i, int j)
        {
            string temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
        private void ButtonChange()
        {
            button1.Text = i < lists.Length ? lists[i] : "";
            button2.Text = i + 1 < lists.Length ? lists[i + 1] : "";
            button3.Text = i + 2 < lists.Length ? lists[i + 2] : "";
        }
        private void button5_Click(object sender, EventArgs e)
        {
            if (lists == null)
            {
                MessageBox.Show("Please scan for drives!");
            }
            else if (i > 0)
            {
                i--;
                ButtonChange();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (lists == null)
            {
                MessageBox.Show("Please scan for drives!");
            }
            else if (i < lists.Length - 3)
            {
                i++;
                ButtonChange();
            }
        }

        
    }
}

