using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Web;
using mainUI;
using System.Security.Policy;
using System.Data.SqlTypes;
using System.Runtime.InteropServices;
using CustomConfig;

namespace contin
{
    public partial class Custome : Form
    {
        SQLCheck sql;
        DriveLetters drive;
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
        private string[] OOBE;
        private bool[] config;
        private Size form;
        private Rectangle labl1, labl2, labl3, labl4, bttn1, buttn2, buttn6, listb1;
        public string password { get; set; }
        public string user { get; set; }
        public string topass { get; set; }
        public string language { get; set; }
        public string langCode, langLocale;
        public Custome(SQLCheck sqls, DriveLetters drives)
        {
            this.sql = sqls;
            this.drive = drives;
            InitializeComponent();
            SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            this.Resize += Rsize;
            form = this.Size;
            bttn1 = new Rectangle(button1.Location, button1.Size);
            buttn2 = new Rectangle(button2.Location, button2.Size);
            buttn6 = new Rectangle(button6.Location, button6.Size);
            listb1 = new Rectangle(listBox1.Location, listBox1.Size);
            labl1 = new Rectangle(label1.Location, label1.Size);
            labl2 = new Rectangle(label2.Location, label2.Size);
            labl3 = new Rectangle(label3.Location, label3.Size);   
            labl4 = new Rectangle(label4.Location, label4.Size);
        }
        private void Rsize(object sender, EventArgs e)
        {
            resizeControl(labl1, label1);
            resizeControl(labl2, label2);
            resizeControl(labl3, label3);
            resizeControl(labl4, label4);
            resizeControl(bttn1, button1);
            resizeControl(buttn2, button2);
            resizeControl(buttn6, button6);
            resizeControl(listb1, listBox1);
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
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button1.Enabled = true;
            button2.Enabled = true;
            label4.Text = "Currently set to: " + config[listBox1.SelectedIndex];
        }

        private void Custome_Load(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = false;
            OOBE = new string[] { "HideEULAPage", "HideLocalAccountScreen", "HideOEMRegistrationScreen", "HideOnlineAccountScreens", "HideWirelessSetupInOOBE", "ProtectYourPC", "SkipUserOOBE" };
            config = new bool[] { true, true, true, true, true, true, true };
            listBox1.Items.AddRange(OOBE);
            if (sql.xmlStatus())
            {
                MakeTXT();
                this.Hide();
                File.WriteAllText(Environment.SystemDirectory + "\\done.txt", "done");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            config[listBox1.SelectedIndex] = true;
            label4.Text = "Currently set to: " + config[listBox1.SelectedIndex];
        }

        private void button2_Click(object sender, EventArgs e)
        {
            config[listBox1.SelectedIndex] = false;
            label4.Text = "Currently set to: " + config[listBox1.SelectedIndex];
        }

        private void button6_Click(object sender, EventArgs e)
        {
            DialogResult iquit = MessageBox.Show("Do you want to continue? You won't be able to change after setup is complete!", "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (iquit == DialogResult.Yes)
            {
                MakeTXT();
                this.Hide();
            }
            else
            {
            }
        }
        private void setKeyboard(string langToSeach)
        {
            Dictionary<string, Tuple<string, string>> customLang = new Dictionary<string, Tuple<string, string>>();
            customLang.Add("Arabic", Tuple.Create("ar-SA", "0401:00000401"));
            customLang.Add("Bulgarian", Tuple.Create("bg-BG", "0419:00010419"));
            customLang.Add("Czech", Tuple.Create("cs-CZ", "0405:00000405"));
            customLang.Add("Danish", Tuple.Create("da-DK", "0406:00000406"));
            customLang.Add("German", Tuple.Create("de-DE", "0407:00000407"));
            customLang.Add("Greek", Tuple.Create("el-GR", "0408:00000408"));
            customLang.Add("English", Tuple.Create("en-US", "0409:00000409"));
            customLang.Add("Spanish", Tuple.Create("es-ES", "0C0A:0000100A"));
            customLang.Add("Estonian", Tuple.Create("et-EE", "0425:00010425"));
            customLang.Add("Finnish", Tuple.Create("fi-FI", "040B:0000040B"));
            customLang.Add("French", Tuple.Create("fr-FR", "040C:0000040C"));
            customLang.Add("Hebrew", Tuple.Create("he-IL", "040D:0000040D"));
            customLang.Add("Croatian", Tuple.Create("hr-HR", "041A:0001041A"));
            customLang.Add("Hungarian", Tuple.Create("hu-HU", "040E:0000040E"));
            customLang.Add("Italian", Tuple.Create("it-IT", "0410:00000410"));
            customLang.Add("Japanese", Tuple.Create("ja-JP", "0411:00000411"));
            customLang.Add("Korean", Tuple.Create("ko-KR", "0412:00000412"));
            customLang.Add("Lithuanian", Tuple.Create("lt-LT", "0427:00010427"));
            customLang.Add("Latvian", Tuple.Create("lv-LV", "0426:00010426"));
            customLang.Add("Dutch", Tuple.Create("nl-NL", "0413:00000413"));
            customLang.Add("Norwegian", Tuple.Create("nb-NO", "0414:00000414"));
            customLang.Add("Polish", Tuple.Create("pl-PL", "0415:00000415"));
            customLang.Add("Romanian", Tuple.Create("ro-RO", "0418:00010418"));
            customLang.Add("Russian", Tuple.Create("ru-RU", "0419:00010419"));
            customLang.Add("Slovak", Tuple.Create("sk-SK", "041B:0001041B"));
            customLang.Add("Slovenian", Tuple.Create("sl-SI", "0424:00010424"));
            customLang.Add("Serbian", Tuple.Create("sr-RS", "0C1A:00010C1A"));
            customLang.Add("Swedish", Tuple.Create("sv-SE", "041D:0000041D"));
            customLang.Add("Thai", Tuple.Create("th-TH", "041E:0000041E"));
            customLang.Add("Turkish", Tuple.Create("tr-TR", "041F:0000041F"));
            customLang.Add("Ukrainian", Tuple.Create("uk-UA", "0422:00010422"));
            if (customLang.ContainsKey(langToSeach))
            {
                langCode = customLang[langToSeach].Item1.ToString();
                langLocale = customLang[langToSeach].Item2.ToString();
            }
        }
        private void MakeTXT()
        {
            setKeyboard(language);
            int protect = 1;
            if (config[5] == true)
            {
                protect = 3;
            }
            string batchScript = @"@echo off" + Environment.NewLine +
                    @"start /wait C:\Windows\Setup\Scripts\autorun.exe C:\Windows\Setup\Scripts\autorun.au3" + Environment.NewLine +
                    @"del C:\Windows\Setup\Scripts\autorun.exe" + Environment.NewLine +
                    @"del C:\Windows\Setup\Scripts\autorun.au3" + Environment.NewLine +
                    @"del C:\Windows\Setup\Scripts\Ninite.exe" + Environment.NewLine +
                    @"del %0";
            File.WriteAllText(drive.TLetter.ToString() + ":\\contin\\installer.bat", batchScript);
            string xmlContent = @"<?xml version=""1.0"" encoding=""utf-8""?>" + Environment.NewLine +
     @"<unattend xmlns=""urn:schemas-microsoft-com:unattend"">" + Environment.NewLine +
     @"    <settings pass=""oobeSystem"">" + Environment.NewLine +
     @"        <component name=""Microsoft-Windows-Shell-Setup"" processorArchitecture=""amd64"" publicKeyToken=""31bf3856ad364e35"" language=""neutral"" versionScope=""nonSxS"" xmlns:wcm=""http://schemas.microsoft.com/WMIConfig/2002/State"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">" + Environment.NewLine +
     @"            <FirstLogonCommands>" + Environment.NewLine +
     @"                <SynchronousCommand wcm:action=""add"">" + Environment.NewLine +
     @"                    <CommandLine>C:\tempdelete.bat</CommandLine>" + Environment.NewLine +
     @"                    <Description>Deletes the T: or D: drive</Description>" + Environment.NewLine +
     @"                    <Order>1</Order>" + Environment.NewLine +
     @"                    <RequiresUserInput>true</RequiresUserInput>" + Environment.NewLine +
     @"                </SynchronousCommand>" + Environment.NewLine +
     @"            </FirstLogonCommands>" + Environment.NewLine +
     @"            <OOBE>" + Environment.NewLine +
     @"                <HideEULAPage>" + config[0] + "</HideEULAPage>" + Environment.NewLine +
     @"                <HideLocalAccountScreen>" + config[1] + "</HideLocalAccountScreen>" + Environment.NewLine +
     @"                <HideOEMRegistrationScreen>" + config[2] + "</HideOEMRegistrationScreen>" + Environment.NewLine +
     @"                <HideOnlineAccountScreens>" + config[3] + "</HideOnlineAccountScreens>" + Environment.NewLine +
     @"                <HideWirelessSetupInOOBE>" + config[4] + "</HideWirelessSetupInOOBE>" + Environment.NewLine +
     @"                <NetworkLocation>Work</NetworkLocation>" + Environment.NewLine +
     @"                <ProtectYourPC>" + protect + "</ProtectYourPC>" + Environment.NewLine +
     @"                <SkipMachineOOBE>" + config[6] + "</SkipMachineOOBE>" + Environment.NewLine +
     @"                <SkipUserOOBE>" + config[6] + "</SkipUserOOBE>" + Environment.NewLine +
     @"                <UnattendEnableRetailDemo>false</UnattendEnableRetailDemo>" + Environment.NewLine +
     @"            </OOBE>" + Environment.NewLine +
     @"            <UserAccounts>" + Environment.NewLine +
     @"                <LocalAccounts>" + Environment.NewLine +
     @"                    <LocalAccount wcm:action=""add"">" + Environment.NewLine +
     @"                        <Password>" + Environment.NewLine +
     @"                            <Value>" + password + "</Value>" + Environment.NewLine +
     @"                            <PlainText>true</PlainText>" + Environment.NewLine +
     @"                        </Password>" + Environment.NewLine +
     @"                        <DisplayName>" + user + "</DisplayName>" + Environment.NewLine +
     @"                        <Group>Administrators</Group>" + Environment.NewLine +
     @"                        <Name>" + user + "</Name>" + Environment.NewLine +
     @"                    </LocalAccount>" + Environment.NewLine +
     @"                </LocalAccounts>" + Environment.NewLine +
     @"            </UserAccounts>" + Environment.NewLine +
     @"        </component>" + Environment.NewLine +
     @"    </settings>" + Environment.NewLine +
     @"<settings pass=""windowsPE"">" + Environment.NewLine +
     @"<component name=""Microsoft-Windows-International-Core-WinPE"" processorArchitecture=""amd64"" publicKeyToken=""31bf3856ad364e35"" language=""neutral"" versionScope=""nonSxS"" xmlns:wcm=""http://schemas.microsoft.com/WMIConfig/2002/State"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">" + Environment.NewLine +
     @"    <InputLocale>" + langLocale + "</InputLocale>" + Environment.NewLine +
     @"    <SystemLocale>" + langCode + "</SystemLocale>" + Environment.NewLine +
     @"    <UILanguage>" + langCode + "</UILanguage>" + Environment.NewLine +
     @"    <UILanguageFallback>en-US</UILanguageFallback>" + Environment.NewLine +
     @"    <UserLocale>" + langCode + "</UserLocale>" + Environment.NewLine +
     @"</component>" + Environment.NewLine +
            @"</settings>" + Environment.NewLine + 
     @"</unattend>";


            string xmlLoc = drive.TLetter.ToString() + ":\\contin\\unattend.xml";
           File.WriteAllText(xmlLoc, xmlContent);
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
    }
}
