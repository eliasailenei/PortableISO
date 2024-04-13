using CustomConfig;
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
        SQLCheck sql;
        DriveLetters drive;
        bool auto;
        public DataTable dataBase;
        public string[] toInstall;
        public int toInstallPointer;
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
        private Rectangle labl1, labl2, labl3, lstv1, lstb1, lnk1, lnk2;
        public string language { get; set; }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Opacity == 1)
            {
                timer1.Stop();
            }
            Opacity += .4;
        }
        public CustomDownload(SQLCheck sqls, DriveLetters drives)
        {
            this.sql = sqls;
            this.drive = drives;
            InitializeComponent();
            SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
            FormBorderStyle = FormBorderStyle.None;
             WindowState = FormWindowState.Maximized;
            this.Resize += Rsize;
            form = this.Size;
            labl1 = new Rectangle(label1.Location, label1.Size);
            labl2 = new Rectangle(label2.Location, label2.Size);
            labl3 = new Rectangle(label3.Location, label3.Size);
            lnk1 = new Rectangle(linkLabel1.Location, linkLabel1.Size);
            lnk2 = new Rectangle(linkLabel2.Location, linkLabel2.Size);
            lstv1 = new Rectangle(listView1.Location, listView1.Size);
            lstb1 = new Rectangle(listBox1.Location, listBox1.Size);
        }
        private void Rsize(object sender, EventArgs e)
        {
            resizeControl(labl1, label1);
            resizeControl(labl2, label2);
            resizeControl(labl3, label3);
            resizeControl(lnk1, linkLabel1);
            resizeControl(lnk2, linkLabel2);
            resizeControl(lstv1, listView1);
            resizeControl(lstb1, listBox1);
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

        private async void Form1_Load(object sender, EventArgs e)
        {
            label2.BeginInvoke(new Action(() => label2.Visible = true));
            await getLatest();
            await getSource();
            await popData();
            await pngDownload();
            await popList();
            label2.BeginInvoke(new Action(() => label2.Visible = false));
            if (sql.xmlStatus() && !String.IsNullOrEmpty(sql.applicationLine))
            {
                auto = true;
                doFinal();
            }
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            doFinal();
        }
        private void doFinal()
        {
            string finalResult = string.Empty;
            if (auto)
            {
                linkLabel2.Enabled = false;
                finalResult = sql.applicationLine;
            } else
            {
                if (toInstall != null && toInstall.Length > 0)
                {
                    linkLabel2.Enabled = false;
                    StringBuilder result = new StringBuilder();
                    foreach (string selectedText in toInstall)
                    {
                        result.Append($"'{selectedText}' + ");
                    }
                    finalResult = result.ToString();

                    if (finalResult.EndsWith("+ "))
                    {
                        finalResult = finalResult.Remove(finalResult.Length - 2);
                    }
                }
            }
            if (!String.IsNullOrEmpty(finalResult)) {
                Process process = new Process();
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = drive.TLetter.ToString() + ":\\contin\\NiniteForCMD\\NiniteForCMD.exe",
                    Arguments = "SELECT " + finalResult + ",LOCATION " + drive.TLetter.ToString() + ":\\contin\\,DOWNLOAD",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                process.Start();
                process.WaitForExit();
                if (Directory.Exists(drive.TLetter.ToString() + ":\\contin\\Extras"))
                {
                    Directory.Delete((drive.TLetter.ToString() + ":\\contin\\Extras"));
                }
                Directory.CreateDirectory(drive.TLetter.ToString() + ":\\contin\\Extras");
                File.Copy(drive.TLetter.ToString() + ":\\contin\\setup.exe", drive.TLetter.ToString() + ":\\contin\\Extras\\setup.exe");
                if (auto)
                {
                    Username user = new Username(sql, drive);
                    timer2.Start();
                    user.language = language;
                    user.Show();
                    this.Close();
                }
                else
                {
                    Driver showDiag = new Driver(drive);
                    int centerX = (Screen.PrimaryScreen.Bounds.Width - showDiag.Width) / 2;
                    int centerY = (Screen.PrimaryScreen.Bounds.Height - showDiag.Height) / 2;
                    showDiag.Location = new Point(centerX, centerY);
                    this.Controls.Add(showDiag);
                    showDiag.BringToFront();
                    showDiag.Show();
                    showDiag.InteractionComplete += (s, args) =>
                    {
                        Username user = new Username(sql, drive);
                        timer2.Start();
                        user.language = language;
                        user.ShowDialog();
                        this.Close();
                    };
                    
                }
                
            } else
            {
                linkLabel2.Enabled = true;
                MessageBox.Show("Please select some programs");
            }

                
            }
            
        
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Username user = new Username(sql, drive);
            timer2.Start();
            user.language = language;
            user.ShowDialog();
            this.Close();
        }

        private async Task getLatest()
        {
            await Task.Run(() =>
            {
                if (Directory.Exists(drive.TLetter.ToString() + ":\\contin\\NiniteForCMD"))
                {
                    Directory.Delete(drive.TLetter.ToString() + ":\\contin\\NiniteForCMD", true);
                }
                Directory.CreateDirectory(drive.TLetter.ToString() + ":\\contin\\NiniteForCMD");
                //This would get the latest version of required tool.
                try
                {
                    using (var client = new WebClient())
                    {
                        client.DownloadFile("https://github.com/eliasailenei/NiniteForCMD/releases/download/Release/Program.zip", drive.TLetter.ToString() + ":\\contin\\NiniteForCMD.zip");
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("It looks like your ISP or network administrator has blocked Ninite. Please use a proxy / VPN or connect to a different network. You can also request for a whitelist. Here is complier error: " + e, "NETWORK ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = @"wpeutil.exe",
                        Arguments = $"shutdown",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    });
                }

                
                ZipFile.ExtractToDirectory(drive.TLetter.ToString() + ":\\contin\\NiniteForCMD.zip", drive.TLetter.ToString() + ":\\contin\\NiniteForCMD");
                File.Delete(drive.TLetter.ToString() + ":\\contin\\NiniteForCMD.zip");
            });
        }
        private async Task getSource()
        {
            await Task.Run(() =>
            {
                Process process = new Process();
                process.StartInfo.FileName = drive.TLetter.ToString() + ":\\contin\\NiniteForCMD\\NiniteForCMD.exe";
                process.StartInfo.Arguments = "LOCATION " + drive.TLetter.ToString() + ":\\contin\\,EXPORT ALL";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                process.WaitForExit();
            });
        }
        private async Task popData()
        {
            await Task.Run(() =>
            {
                dataBase = new DataTable();
                dataBase.Columns.Add("Title");
                dataBase.Columns.Add("Value");
                dataBase.Columns.Add("SRC");
                string[] source = File.ReadAllLines(drive.TLetter.ToString() + ":\\contin\\EXPORT.txt");
                foreach (string item in source)
                {
                    DataRow newRow = dataBase.NewRow();
                    string[] parts = item.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 3)
                    {
                        newRow["Value"] = parts[0].Trim();
                        newRow["SRC"] = parts[1].Trim();
                        newRow["Title"] = parts[2].Trim();
                        dataBase.Rows.Add(newRow);
                    }
                }
            });
        }
        private async Task popList()
        {
            await Task.Run(() =>
            {
                int pointer = 0;
                foreach (DataRow row in dataBase.Rows)
                {
                    int imageIndex = pointer;
                    listView1.BeginInvoke(new Action(() => listView1.Items.Add(row["Title"].ToString(), imageIndex)));
                    pointer++;
                }
                listView1.BeginInvoke(new Action(() => listView1.LargeImageList = imageList1));
            });
        }
        private async Task pngDownload()
        {
            await Task.Run(() =>
            {
               imageList1 = new ImageList();
               imageList1.ColorDepth = ColorDepth.Depth32Bit;
               imageList1.ImageSize = new Size(50, 50);
                int point = 0;

                try
                {
                    if (Directory.Exists("ICONS"))
                    {
                        Directory.Delete("ICONS", true);
                    }

                    Directory.CreateDirectory("ICONS");

                    foreach (DataRow row in dataBase.Rows)
                    {
                        string icon = row["SRC"].ToString();
                        if (!string.IsNullOrEmpty(icon))
                        {
                            string png = $"ICONS/ICON_{point}.png";

                            try
                            {
                                using (WebClient client = new WebClient())
                                {
                                    client.DownloadFile(icon, png);
                                }

                                imageList1.Images.Add(Image.FromFile(png));
                            }
                            catch (Exception ex)
                            {
                                if (png != null)
                                {
                                    Console.WriteLine("Minor error: PNG not possible to install, debug code: " + ex);
                                    File.Delete(png);
                                    using (WebClient client = new WebClient())
                                    {
                                        client.DownloadFile("https://static.vecteezy.com/system/resources/thumbnails/017/178/563/small/cross-check-icon-symbol-on-transparent-background-free-png.png", png);
                                    }
                                  imageList1.Images.Add(Image.FromFile(png));
                                }
                            }
                            point++;
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("It looks like your ISP or network administrator has blocked Ninite. Please use a proxy / VPN or connect to a different network. You can also request for a whitelist. Here is complier error: " + e, "NETWORK ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = @"wpeutil.exe",
                        Arguments = $"shutdown",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    });
                }
            });
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem selItem = listView1.SelectedItems[0];
                string selected = selItem.Text;
                if (toInstall == null)
                {
                    toInstall = new string[] { selected };
                }
                else
                {
                    if (toInstall.Contains(selected))
                    {
                        toInstall = toInstall.Where(item => item != selected).ToArray();
                    }
                    else
                    {
                        toInstall = toInstall.Concat(new string[] { selected }).ToArray();
                    }
                }
                listBox1.Items.Clear();
                listBox1.Items.AddRange(toInstall.Where(item => item != null).ToArray());
            }
        }


    }
}

