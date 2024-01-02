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

namespace contin
{
    public partial class Custome : Form
    {
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
        public Custome()
        {
            InitializeComponent();
            SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            this.Resize += Rsize;
            form = this.Size;
            bttn1 = new Rectangle(button1.Location, button1.Size);
            buttn2 = new Rectangle(button2.Location, button2.Size);
            buttn6 = new Rectangle(button6.Location, button6.Size);
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
            }
            else
            {
            }
        }
        private void MakeTXT()
        {
            int protect = 1;
            if (config[5] == true)
            {
                protect = 3;
            }
            string batchScript = @"@echo off" + Environment.NewLine +
                    @"setlocal" + Environment.NewLine +
                    @"echo Dont panic! We are just setting up your Windows. This shouldn't take long..." + Environment.NewLine +
                    @"echo." + Environment.NewLine + 
                    @"echo Please DO NOT close this or you be stuck with looping install!" + Environment.NewLine +
                    @"echo." + Environment.NewLine + 
                    @"echo Step 1. Installing your software (you need to close it yourself)" + Environment.NewLine +
                    @"echo." + Environment.NewLine + 
                    @":CHECK_RUNNING" + Environment.NewLine +
                    @"tasklist | find /i ""setup.exe"" > nul" + Environment.NewLine +
                    @"if errorlevel 1 (" + Environment.NewLine +
                    @"    goto SETUP_DONE" + Environment.NewLine +
                    @") else (" + Environment.NewLine +
                    @"    timeout /t 5 /nobreak > nul" + Environment.NewLine +
                    @"    goto CHECK_RUNNING" + Environment.NewLine +
                    @")" + Environment.NewLine +
                    @":SETUP_DONE" + Environment.NewLine +
                    @"del setup.exe" + Environment.NewLine +
                    @"del %0";
            File.WriteAllText("C:\\Windows\\System32\\installer.bat", batchScript);
            string xmlContent = @"<?xml version=""1.0"" encoding=""utf-8""?>" + Environment.NewLine +
     @"<unattend xmlns=""urn:schemas-microsoft-com:unattend"">" + Environment.NewLine +
     @"    <settings pass=""oobeSystem"">" + Environment.NewLine +
     @"        <component name=""Microsoft-Windows-Shell-Setup"" processorArchitecture=""amd64"" publicKeyToken=""31bf3856ad364e35"" language=""neutral"" versionScope=""nonSxS"" xmlns:wcm=""http://schemas.microsoft.com/WMIConfig/2002/State"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">" + Environment.NewLine +
     @"            <FirstLogonCommands>" + Environment.NewLine +
     @"                <SynchronousCommand wcm:action=""add"">" + Environment.NewLine +
     @"                    <CommandLine>move T:\contin\Extras\setup.exe C:\Windows\System32\setup.exe</CommandLine>" + Environment.NewLine +
     @"                    <Description>Moves your ninite setup to the root folder.</Description>" + Environment.NewLine +
     @"                    <Order>1</Order>" + Environment.NewLine +
     @"                    <RequiresUserInput>true</RequiresUserInput>" + Environment.NewLine +
     @"                </SynchronousCommand>" + Environment.NewLine +
     @"        <SynchronousCommand wcm:action=""add"">" + Environment.NewLine +
     @"            <CommandLine>schtasks /create /tn RunInstaller /sc ONLOGON /ru Administrator /tr ""C:\Windows\System32\installer.bat""</CommandLine>" + Environment.NewLine +
     @"            <Description>Create a logon script.</Description>" + Environment.NewLine +
     @"            <Order>2</Order>" + Environment.NewLine +
     @"            <RequiresUserInput>true</RequiresUserInput>" + Environment.NewLine +
     @"        </SynchronousCommand>" + Environment.NewLine +
     @"        <SynchronousCommand wcm:action=""add"">" + Environment.NewLine +
     @"            <CommandLine>schtasks /change /tn RunInstaller /enabled true</CommandLine>" + Environment.NewLine +
     @"            <Description>Enables the script.</Description>" + Environment.NewLine +
     @"            <Order>3</Order>" + Environment.NewLine +
     @"            <RequiresUserInput>true</RequiresUserInput>" + Environment.NewLine +
     @"        </SynchronousCommand>" + Environment.NewLine +
     @"                <SynchronousCommand wcm:action=""add"">" + Environment.NewLine +
     @"                    <CommandLine>C:\tempdelete.bat</CommandLine>" + Environment.NewLine +
     @"                    <Description>Deletes the T: or D: drive</Description>" + Environment.NewLine +
     @"                    <Order>4</Order>" + Environment.NewLine +
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
     @"<SetupUILanguage>" + Environment.NewLine +
     @"<UILanguage>en-US</UILanguage>" + Environment.NewLine +
     @"</SetupUILanguage>" + Environment.NewLine +
     @"<InputLocale>en-US</InputLocale>" + Environment.NewLine +
     @"<SystemLocale>en-US</SystemLocale>" + Environment.NewLine +
     @"<UserLocale>en-US</UserLocale>" + Environment.NewLine +
     @"</component>" + Environment.NewLine +
     @"</settings>" + Environment.NewLine + 
     @"</unattend>";


            string xmlLoc = "T:\\contin\\unattend.xml";
           File.WriteAllText(xmlLoc, xmlContent);
           //// Clean clean = new Clean();
           // clean.isExpress = true;
           // clean.topass = topass;
           // timer2.Start();
           // clean.ShowDialog();
           this.Close();
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
