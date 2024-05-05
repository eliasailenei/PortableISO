using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Xml.Linq;
using System.Security.Cryptography;
namespace SetupLoader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent(); // load up UI components (placed by .net)
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label1.Text = "Checking internet connection"; 
            label1.Location = new Point(10,10);
            pictureBox2.Hide();
        }

        private async void Form1_Shown(object sender, EventArgs e)
        {
            buttonStatus(false); // we don't need to show UI now
           await loadPE(); // open PENetwork which is a third party tool that will init the internet connection
            setNet(); // gets the network reader
        }

        private async Task loadPE()
        {
            try
            {
                label1.Text = "Setting up internet drivers";
                label1.Location = new Point(35, 10);
                await Task.Run(() => // loads PENetwork automatically
                {
                    Process process = new Process();
                    process.StartInfo.FileName = "PENetwork\\PENetwork.exe";
                    process.Start();
                    process.WaitForExit();
                });
            } catch (Exception ex)
            { // if it can't be found then the computer will reboot
                MessageBox.Show("Could not initialize the network drivers, here is the error:" + ex.Message);
                Process.Start(new ProcessStartInfo
                {
                    FileName = @"wpeutil.exe",
                    Arguments = $"reboot",
                    UseShellExecute = false,
                    CreateNoWindow = true
                });
            }
           
        }
        private void loadmainUI()
        { // just opens mainUI.exe
            Process.Start("mainUI.exe");
            this.Hide();
        }
        private void buttonStatus(bool status)
        { // sets the button Visibility in bulk
            button1.Visible = status;
            button2.Visible = status;
            button3.Visible = status;
            button4.Visible = status;
            button5.Visible = status;
            button6.Visible = status;
        }
        private bool checkNet()
        {
            try
            {
                string host = "github.com"; // try to make connection to GitHub

                using (Ping ping = new Ping())
                {
                    PingReply reply = ping.Send(host);
                    if (reply.Status == IPStatus.Success) // if response is 200
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }

            return false;
        }
        private void setNet()// recursive algorithms
        {
            if (checkNet()) // if the connection to GitHub worked
            {
                richTextBox1.Clear();
                richTextBox1.AppendText("Connection to internet was successful! Downloading the latest version of PortableISO!");
                Process.Start(new ProcessStartInfo
                {
                    FileName = @"powershell",
                    Arguments = "-Command \"[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12\"", // set TLS to 1.2
                    UseShellExecute = false,
                    CreateNoWindow = true
                });
             getLatestmainUI(); // download the latest mainUI
            }
            else
            {
                pictureBox1.Hide();
                pictureBox2.Show();
                richTextBox1.Clear();
                richTextBox1.AppendText("Connection failed! You can use Wi-Fi if your system supports it, configure PENetwork or close system. NOTE:Use PENetwork if Wi-Fi isn't connecting!");
                buttonStatus(true); // show the buttons so that the user can connect to Wi-Fi
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox2.Hide();
            pictureBox1.Show();
            setNet(); // try to connect again
        }

        private void getLatestmainUI()
        {
            try
            { 
                using (var client = new WebClient())
                {
                    client.DownloadFile("https://github.com/eliasailenei/PortableISO/releases/download/mainUI/Release.zip", "mainUI.zip"); // download the latest release from github // reading and writing from files
                }
                ZipFile.ExtractToDirectory("mainUI.zip", Directory.GetCurrentDirectory()); // save it in System32
                File.Delete("mainUI.zip"); // delete the zip file
                loadmainUI(); //open mainUI
            } catch (Exception ex) { // reboot if download was not possible
            MessageBox.Show(ex.Message);
                Process.Start(new ProcessStartInfo
                {
                    FileName = @"wpeutil.exe",
                    Arguments = $"reboot",
                    UseShellExecute = false,
                    CreateNoWindow = true
                });
            }
            
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            try
            {
                await loadPE(); // reopen PENetwork
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string bugs = await RunCommand("cmd.exe", "/c netsh wlan delete profile name=* i=*"); // delete any previous connections to Wi-Fi
            try
            {
                Process.Start(@"X:\Windows\System32\WinXShell\WinXShell.exe", "-ui -jcfg wxsUI\\UI_WIFI.zip"); // opens third party program to show Wi-Fi options
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            Environment.Exit(0); // close the system
        }
        string[] allConnections;
        Decryptors decryptors = new Decryptors();
        private async void button6_Click(object sender, EventArgs e)
        {
            Decryptors decryptors = new Decryptors();
            if (checkNet()) // if there is an internet connection
            {
                if (Directory.Exists(Directory.GetCurrentDirectory() + "\\wifiProfiles")) // delete any previous attempts
                {
                    Directory.Delete(Directory.GetCurrentDirectory() + "\\wifiProfiles",true);
                }
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\wifiProfiles"); // create a folder where to keep your profiles
                try
                {
                    string debug = await RunCommand("cmd.exe", $"/c netsh wlan export profile key=clear folder={Directory.GetCurrentDirectory() + "\\wifiProfiles"}"); // export all your wifi connections
                    button6.Enabled = false;
                    goProfile(); // create the SSID and password to then reattempt to download mainUI
                } catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message + "\n You can't auto connect the WiFi in post setup!");
                }
                
            }
            else
            {
                MessageBox.Show("You need some sort of internet for this!");
            }
        }
        private void goProfile() { // writing and reading from files
            string[] xmls = Directory.GetFiles(Directory.GetCurrentDirectory() + "\\wifiProfiles"); // get all the XML's
                        foreach (string files in xmls)
                        {
                            XDocument xml = XDocument.Load(files); // load the XML //pharring XML
                            XNamespace ns = "http://www.microsoft.com/networking/WLAN/profile/v1";
                            string name = xml.Root.Element(ns + "name").Value; // get the SSID
                            string keyMaterial = xml.Root
                                                     .Element(ns + "MSM")
                                                     .Element(ns + "security")
                                                     .Element(ns + "sharedKey")
                                                     .Element(ns + "keyMaterial")
                                                     .Value; // get the password
                                File.WriteAllText("SSID.txt", decryptors.Encrypt(name, "passforkey", 128)); // save the SSID in an encrypted txt file
                                File.WriteAllText("Password.txt", decryptors.Encrypt(keyMaterial, "passforkey", 128)); // save the password in an encrypted txt file
                                File.Copy(files, Directory.GetCurrentDirectory() + "\\profile.xml"); // place the profile outside for later use
                                pictureBox2.Hide();
                                pictureBox1.Show();
                                setNet(); // reattempt to connect to mainUI
                button6.Enabled = true;
                break;
                        }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            Process.Start("cmd.exe"); // opens cmd
        }
        static async Task<string> RunCommand(string program, string args)
        {
            string outs;
            try
            {// run a program and return its output as string
                Process process = new Process();
                process.StartInfo.FileName = program;
                process.StartInfo.Arguments = args;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();
                outs = await process.StandardOutput.ReadToEndAsync();
                Console.WriteLine(outs);
                process.WaitForExit();
                return outs;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return string.Empty;
            }

        }
    }
    public class Decryptors
    {
        public string DESDecrypt(string encryptedData, string key, int keySize)
        {
            using (TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider())
            {
                // set the key for decryption
                tripleDES.Key = GenerateValidKey(key, keySize);
                tripleDES.Mode = CipherMode.ECB;
                try
                {
                    // create a decryptor
                    using (ICryptoTransform decryptor = tripleDES.CreateDecryptor())
                    {
                        // convert the Base64 string to byte array
                        byte[] encryptedBytes = Convert.FromBase64String(encryptedData);
                        // decrypt the bytes
                        byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                        return Encoding.UTF8.GetString(decryptedBytes);
                    }
                }
                catch
                {
                    // if decryption fails, return error message
                    return "errPass";
                }
            }
        }

        public string Encrypt(string data, string key, int keySize)
        {
            using (TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider())
            {
                tripleDES.Key = GenerateValidKey(key, keySize);
                tripleDES.Mode = CipherMode.ECB;

                // create an encryptor
                using (ICryptoTransform encryptor = tripleDES.CreateEncryptor())
                {
                    // convert data to bytes
                    byte[] dataBytes = Encoding.UTF8.GetBytes(data);
                    // encrypt the data
                    byte[] encryptedBytes = encryptor.TransformFinalBlock(dataBytes, 0, dataBytes.Length);
                    return Convert.ToBase64String(encryptedBytes);
                }
            }
        }

        private byte[] GenerateValidKey(string key, int keySize)
        {
            try
            {
                // create an MD5 hash of the key
                using (var md5 = MD5.Create())
                {
                    byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(key));
                    // resize the hash to match the key size
                    Array.Resize(ref hash, keySize / 8);
                    return hash;
                }
            }
            catch
            {
                // if an exception occurs, return null
                Console.WriteLine("Credentials invalid! Try again later!");
                return null;
            }
        }
    }
}
