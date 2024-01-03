using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection.Emit;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace contin
{
    public partial class CustomDownload : Form
    {
        // Known issues - can't display the rest of the 8 missing programs, but minor
        public string loc = "T:\\contin\\";
        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        static readonly IntPtr HWND_TOP = new IntPtr(0);
        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        const UInt32 SWP_NOSIZE = 0x0001;
        const UInt32 SWP_NOMOVE = 0x0002;
        const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        private Size form;
        private bool isFail;
        private Rectangle bttn1, bttn2, bttn3, bttn4, bttn5, bttn6, bttn7, bttn8, bttn9, bttn10, bttn11, bttn12, bttn13, bttn14, bttn15, bttn16, bttn17, bttn18, bttn19, bttn20, cntrlbox1, cntrlbox2, cntrlbox3, cntrlbox4, cntrlbox5, cntrlbox6, cntrlbox7, cntrlbox8, cntrlbox9, cntrlbox10, cntrlbox11, cntrlbox12, cntrlbox13, cntrlbox14, cntrlbox15, cntrlbox16, cntrlbox17, cntrlbox18, cntrlbox19, cntrlbox20, lbl1, lbl2, lbl3, lbl4, lbl5, text;

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Opacity == 1)
            {
                timer1.Stop();
            }
            Opacity += .4;
        }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Username user = new Username();
            timer2.Start();
            user.ShowDialog();
        }

        private string myDir = AppDomain.CurrentDomain.BaseDirectory;
        private void button1_Click(object sender, EventArgs e)
        {

        }

        static string[] value, src, title, iconLoc;
        static int[][] pages = new int[5][];
        static int add, index, currentPage, pointy;
        static HashSet<string> selected = new HashSet<string>();
        HashSet<int> displayedNumbers = new HashSet<int>();

        private void linkLabel3_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (selected.Count > 0)
            {
                string selectedItems = string.Join("\n", selected);
                MessageBox.Show(selectedItems);
            }
        }
        private void AddChecks()
        {
            foreach (int num in pages[currentPage].Distinct().ToArray())
            {
                foreach (Control c in this.Controls)
                {
                    if (c is CheckBox checkBox)
                    {
                        if (c.Name == "checkBox" + num.ToString())
                        {
                            checkBox.Checked = true;
                        }
                    }
                }

            }
        }

        public CustomDownload()
        {
            InitializeComponent();
            SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
            FormBorderStyle = FormBorderStyle.None;
             WindowState = FormWindowState.Maximized;
            this.Resize += Rsize;
            form = this.Size;
            for (int i = 1; i <= 20; i++)
            {
                string checkBoxName = "checkBox" + i;
                Control[] foundControls = this.Controls.Find(checkBoxName, true);

                if (foundControls.Length > 0 && foundControls[0] is CheckBox)
                {
                    CheckBox checkBox = foundControls[0] as CheckBox;
                    checkBox.Click += Changer;
                }
            }

            bttn1 = new Rectangle(button1.Location, button1.Size);
            bttn2 = new Rectangle(button2.Location, button2.Size);
            bttn3 = new Rectangle(button3.Location, button3.Size);
            bttn4 = new Rectangle(button4.Location, button4.Size);
            bttn5 = new Rectangle(button5.Location, button5.Size);
            bttn6 = new Rectangle(button6.Location, button6.Size);
            bttn7 = new Rectangle(button7.Location, button7.Size);
            bttn8 = new Rectangle(button8.Location, button8.Size);
            bttn9 = new Rectangle(button9.Location, button9.Size);
            bttn10 = new Rectangle(button10.Location, button10.Size);
            bttn11 = new Rectangle(button11.Location, button11.Size);
            bttn12 = new Rectangle(button12.Location, button12.Size);
            bttn13 = new Rectangle(button13.Location, button13.Size);
            bttn14 = new Rectangle(button14.Location, button14.Size);
            bttn15 = new Rectangle(button15.Location, button15.Size);
            bttn16 = new Rectangle(button16.Location, button16.Size);
            bttn17 = new Rectangle(button17.Location, button17.Size);
            bttn18 = new Rectangle(button18.Location, button18.Size);
            bttn19 = new Rectangle(button19.Location, button19.Size);
            bttn20 = new Rectangle(button20.Location, button20.Size);
            cntrlbox1 = new Rectangle(checkBox1.Location, checkBox1.Size);
            cntrlbox2 = new Rectangle(checkBox2.Location, checkBox2.Size);
            cntrlbox3 = new Rectangle(checkBox3.Location, checkBox3.Size);
            cntrlbox4 = new Rectangle(checkBox4.Location, checkBox4.Size);
            cntrlbox5 = new Rectangle(checkBox5.Location, checkBox5.Size);
            cntrlbox6 = new Rectangle(checkBox6.Location, checkBox6.Size);
            cntrlbox7 = new Rectangle(checkBox7.Location, checkBox7.Size);
            cntrlbox8 = new Rectangle(checkBox8.Location, checkBox8.Size);
            cntrlbox9 = new Rectangle(checkBox9.Location, checkBox9.Size);
            cntrlbox10 = new Rectangle(checkBox10.Location, checkBox10.Size);
            cntrlbox11 = new Rectangle(checkBox11.Location, checkBox11.Size);
            cntrlbox12 = new Rectangle(checkBox12.Location, checkBox12.Size);
            cntrlbox13 = new Rectangle(checkBox13.Location, checkBox13.Size);
            cntrlbox14 = new Rectangle(checkBox14.Location, checkBox14.Size);
            cntrlbox15 = new Rectangle(checkBox15.Location, checkBox15.Size);
            cntrlbox16 = new Rectangle(checkBox16.Location, checkBox16.Size);
            cntrlbox17 = new Rectangle(checkBox17.Location, checkBox17.Size);
            cntrlbox18 = new Rectangle(checkBox18.Location, checkBox18.Size);
            cntrlbox19 = new Rectangle(checkBox19.Location, checkBox19.Size);
            cntrlbox20 = new Rectangle(checkBox20.Location, checkBox20.Size);
            lbl1 = new Rectangle(linkLabel1.Location, linkLabel1.Size);
            lbl2 = new Rectangle(linkLabel2.Location, linkLabel2.Size);
            lbl3 = new Rectangle(linkLabel3.Location, linkLabel3.Size);
            lbl4 = new Rectangle(linkLabel4.Location, linkLabel4.Size);
            lbl5 = new Rectangle(linkLabel5.Location, linkLabel5.Size);
            text = new Rectangle(label1.Location, label1.Size);
        }
        private void Rsize(object sender, EventArgs e)
        {
            resizeControl(bttn1, button1);
            resizeControl(bttn2, button2);
            resizeControl(bttn3, button3);
            resizeControl(bttn4, button4);
            resizeControl(bttn5, button5);
            resizeControl(bttn6, button6);
            resizeControl(bttn7, button7);
            resizeControl(bttn8, button8);
            resizeControl(bttn9, button9);
            resizeControl(bttn10, button10);
            resizeControl(bttn11, button11);
            resizeControl(bttn12, button12);
            resizeControl(bttn13, button13);
            resizeControl(bttn14, button14);
            resizeControl(bttn15, button15);
            resizeControl(bttn16, button16);
            resizeControl(bttn17, button17);
            resizeControl(bttn18, button18);
            resizeControl(bttn19, button19);
            resizeControl(bttn20, button20);
            resizeControl(cntrlbox1, checkBox1);
            resizeControl(cntrlbox2, checkBox2);
            resizeControl(cntrlbox3, checkBox3);
            resizeControl(cntrlbox4, checkBox4);
            resizeControl(cntrlbox5, checkBox5);
            resizeControl(cntrlbox6, checkBox6);
            resizeControl(cntrlbox7, checkBox7);
            resizeControl(cntrlbox8, checkBox8);
            resizeControl(cntrlbox9, checkBox9);
            resizeControl(cntrlbox10, checkBox10);
            resizeControl(cntrlbox11, checkBox11);
            resizeControl(cntrlbox12, checkBox12);
            resizeControl(cntrlbox13, checkBox13);
            resizeControl(cntrlbox14, checkBox14);
            resizeControl(cntrlbox15, checkBox15);
            resizeControl(cntrlbox16, checkBox16);
            resizeControl(cntrlbox17, checkBox17);
            resizeControl(cntrlbox18, checkBox18);
            resizeControl(cntrlbox19, checkBox19);
            resizeControl(cntrlbox20, checkBox20);
            resizeControl(lbl1, linkLabel1);
            resizeControl(lbl2, linkLabel2);
            resizeControl(lbl3, linkLabel3);
            resizeControl(lbl4, linkLabel4);
            resizeControl(lbl5, linkLabel5);
            resizeControl(text, label1);
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
            if (Directory.Exists("T:\\contin\\NiniteForCMD"))
            {
                Directory.Delete("T:\\contin\\NiniteForCMD", true);
            }
            //This would get the latest version of required tool.
            using (var client = new WebClient())
            {
                client.DownloadFile("https://github.com/eliasailenei/NiniteForCMD/releases/download/Release/Program.zip", "T:\\contin\\NiniteForCMD.zip");
            }
            Directory.CreateDirectory("T:\\contin\\NiniteForCMD");
            ZipFile.ExtractToDirectory("T:\\contin\\NiniteForCMD.zip", "T:\\contin\\NiniteForCMD");
            File.Delete("T:\\contin\\NiniteForCMD.zip");
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            getLatest();
            Start(sender, e);
        }
        private void Start(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < 5; i++)
                {
                    pages[i] = new int[128];
                }
                pointy = 0;
                GetSource(sender, e);
                string Ninite = @"EXPORT.txt";
                string[] lines = File.ReadAllLines(Ninite);
                value = new string[lines.Length + 1];
                MessageBox.Show(value.Length.ToString());
                src = new string[lines.Length + 1];
                title = new string[lines.Length + 1];
                iconLoc = new string[lines.Length + 1];
                int point = 1;
                foreach (string line in lines)
                {
                    string[] parts = line.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length >= 3)
                    {
                        value[point] = parts[0].Trim();
                        src[point] = parts[1].Trim();
                        title[point] = parts[2].Trim();
                    }
                    point++;
                }

                pngDownload();
                Changer(sender, e);
            }
            catch (Exception ex)
            {
                isFail = true;
                GetSource(sender, e);
            }
        }
        private void GetSource(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = loc +@"NiniteForCMD\NiniteForCMD.exe",
                Arguments = $"EXPORT ALL",
                UseShellExecute = false,
                CreateNoWindow = true
            });
            if (isFail)
            {
                isFail = false;
                Start(sender, e);
            }
            
        }

        async Task pngDownload()
        {

            int point = 0;
            try
            {

                if (Directory.Exists("ICONS"))
                {
                    Directory.Delete("ICONS", true);
                }
                Directory.CreateDirectory("ICONS");
                foreach (string icon in src)
                {
                    if (!string.IsNullOrEmpty(icon))
                    {
                        using (WebClient client = new WebClient())
                        {
                            client.DownloadFile(icon, "ICONS/ICON_" + point + ".png");
                        }
                        iconLoc[point] = "ICONS/ICON_" + point + ".png";
                    }

                    point++;
                }
                foreach (string png in iconLoc)
                {
                    try
                    {
                        using (Image img = Image.FromFile(png))
                        {

                        }
                    }
                    catch (Exception ex)
                    {
                        if (png != null)
                        {
                            File.Delete(png);
                            using (WebClient client = new WebClient())
                            {
                                client.DownloadFile("https://static.vecteezy.com/system/resources/thumbnails/017/178/563/small/cross-check-icon-symbol-on-transparent-background-free-png.png", png);
                            }
                        }

                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("It looks like your ISP or network administrator has blocked Ninite. Please use a proxy / VPN or connect to a different network. You can also request for a whitelist. ");
                this.Close();
            }



        }

        private void linkLabel4_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            StringBuilder result = new StringBuilder();
            foreach (string selectedText in selected)
            {
                result.Append($"'{selectedText}' + ");
            }
            string finalResult = result.ToString();

            if (finalResult.EndsWith("+ "))
            {
                finalResult = finalResult.Remove(finalResult.Length - 2);
            }

            Process process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                FileName = @"NiniteForCMD\NiniteForCMD.exe",
                Arguments = "SELECT " + finalResult + " ,DOWNLOAD",
                UseShellExecute = false,
                CreateNoWindow = true
            };
            process.Start();
            process.WaitForExit();
            File.Move(myDir + "\\setup.exe", myDir + "\\Extras\\setup.exe");
            Username user = new Username();
            timer2.Start();
            user.ShowDialog();
        }



        private void Changer(object sender, EventArgs e)
        {
            try
            {
                foreach (Control c in this.Controls)
                {
                    if (c is CheckBox)
                    {
                        CheckBox checkBox = (CheckBox)c;
                        index = int.Parse(checkBox.Name.Replace("checkBox", ""));
                        checkBox.Text = title[index + add];

                        if (checkBox.Checked)
                        {
                            string checkBoxName = checkBox.Name;
                            Regex numOnly = new Regex("\\d+", RegexOptions.IgnoreCase);
                            Match match = numOnly.Match(checkBoxName);
                            int num = int.Parse(match.Value);
                            string toPass = title[num + add];

                            if (match.Success)
                            {
                                if (!selected.Contains(toPass))
                                {
                                    Selected(toPass);
                                    pages[currentPage][pointy] = index;
                                    pointy++;
                                }
                            }
                        }
                        else
                        {
                            string checkBoxName = checkBox.Name;
                            Regex numOnly = new Regex("\\d+", RegexOptions.IgnoreCase);
                            Match match = numOnly.Match(checkBoxName);
                            int num = int.Parse(match.Value);
                            string toPass = title[num + add];
                            if (match.Success)
                            {
                                if (selected.Contains(toPass))
                                {
                                    selected.Remove(toPass);
                                    for (int i = 0; i < pages[currentPage].Length; i++)
                                    {
                                        if (pages[currentPage][i] == int.Parse(match.Value))
                                        {
                                            pages[currentPage][i] = 0;
                                        }
                                    }

                                }
                            }
                        }
                    }
                    else if (c is Button)
                    {
                        Button button = (Button)c;
                        index = int.Parse(button.Name.Replace("button", ""));
                        button.Text = " ";
                        button.BackgroundImage = Image.FromFile(iconLoc[index + add]);
                        button.BackgroundImageLayout = ImageLayout.Stretch;
                    }
                }
            } catch {
                GetSource(sender, e);
            }
           
        }

        private void Selected(string passed)
        {
            selected.Add(passed);
        }

        private void UpdateAdd()
        {
            switch (currentPage)
            {
                case 1:
                    add = 20;
                    break;
                case 2:
                    add = 40;
                    break;
                case 3:
                    add = 60;
                    break;
                case 4:
                    add = 76;
                    break;
                default:
                    add = 0;
                    break;
            }
        }

        private void Locations()
        {
            foreach (Control control in this.Controls)
            {
                if (control is CheckBox checkBox)
                {
                    checkBox.Checked = false;
                }
            }
        }

        private void linkLabel1_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (currentPage > 0)
            {
                currentPage--;
                UpdateAdd();
                Locations();
                AddChecks();
                Changer(sender, e);
            }

        }

        private void linkLabel2_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (currentPage < 4)
            {

                currentPage++;
                UpdateAdd();
                Locations();
                AddChecks();
                Changer(sender, e);
            }

        }
    }
}

