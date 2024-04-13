using System;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using SimpleWifi;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Net.NetworkInformation;
using Microsoft.SqlServer.Server;
using System.Collections.Generic;
using System.Xml;
using System.Net.Sockets;

public class MoveInstalled
{
    static string[] drives = Environment.GetLogicalDrives();
    static string continLoc;
    static bool hasDrivers, skipNet;

    static async Task Main()
    {
        if (!File.Exists("C:\\yes.txt"))
        {
            Console.WriteLine("Hey!");
            Console.WriteLine("");
            Console.WriteLine("We are getting ready! Please wait...");

            foreach (string drive in drives)
            {
                if (Directory.Exists(drive + "contin\\PortableDriver"))
                {
                    continLoc = drive + "contin\\PortableDriver";
                    hasDrivers = true;
                    Copier.CopyDirectory(continLoc,"C:\\Windows\\Setup\\Scripts\\PortableDriver",false);
                    Console.WriteLine("It looks like you have PortabelDriver setup! Lets import that...");
                    break;
                }
            }
            Console.WriteLine("Erasing your temp drive");
            await RunCommand("cmd.exe", "/c C:\\tempdelete.bat");
            Console.WriteLine("We are now going to continue the setup after you have logged on!");
            await RunCommand("cmd.exe", "/creg add \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\RunOnce\" /v PortableISO /t REG_SZ /d \"C:\\Windows\\Setup\\Scripts\\MoveInstalled.exe\" /f");
            File.WriteAllText("C:\\yes.txt", "begin");
        }
        else
        {
            Console.WriteLine("Welcome back! Please don't close me!");
           await doDrivers();
           await doNet();
            while (!net(skipNet))
            {
                Console.Clear();
                Console.WriteLine("No internet! Please insert the ethernet cable back or connect to WiFi! You can type \"skip\" if you want to continue anyways... Press enter to rescan");
                string inp = Console.ReadLine();
                if (inp == "skip")
                {
                    skipNet = true;
                }
            }
            if (File.Exists("C:\\donotreopen.txt"))
            {
                File.Delete("C:\\donotreopen.txt");
            }
            else
            {
              await  doDrivers();
            }
            if (File.Exists("C:\\Windows\\Setup\\Scripts\\windowskey.txt"))
            {
                Console.WriteLine("Activating Windows...");
                string key = File.ReadAllText("C:\\Windows\\Setup\\Scripts\\windowskey.txt");
                key = Decryptors.DESDecrypt(key, "passforkey", 128);
                try
                {
                    await RunCommand(@"powershell.exe", $"-Command \"Set-WindowsProductKey -Path 'C:\\' -ProductKey '{key}'\"");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message + ". Please activate manually!");
                }

            }
            if (File.Exists("C:\\Windows\\Setup\\Scripts\\Ninite.exe") && !hasDrivers)
            {
                Console.WriteLine("Let's first get your programs");
                await RunCommand("C:\\Windows\\Setup\\Scripts\\autorun.exe", "C:\\Windows\\Setup\\Scripts\\autorun.au3");
            }
            string[] allFiles = Directory.GetFiles("C:\\", "*.bin");
            List<string> files = new List<string>();    
            foreach (string file in allFiles)
            {
                if (File.Exists(file) && file.Contains("PB"))
                {
                    files.Add(file);
                }
            }
            if (files.Count > 1 || files.Count == 0 )
            {
                Console.WriteLine("Sorry, either there is nothing to backup or you don't meet the requirements");
            }
            else
            {
                Process.Start("https://github.com/eliasailenei/PortableBackup/releases/download/V1/Release.zip");
            }
            string toWrite = "@echo off\necho Please wait, doing cleanup\ndel C:\\yes.txt\necho Unfortunatly, Windows does not know when the setup is done. Press any key to delete the setup folder once done.\npause\npowershell -Command \"$folderPath = 'C:\\Windows\\Setup\\Scripts'; $retryDurationSeconds = 5; $maxRetryDurationSeconds = 60; $startTime = Get-Date; $endTime = $startTime.AddSeconds($maxRetryDurationSeconds); while ((Test-Path $folderPath) -and ((Get-Date) -lt $endTime)) { try { Remove-Item -Path $folderPath -Recurse -Force -ErrorAction Stop } catch { Start-Sleep -Seconds $retryDurationSeconds } }; if (Test-Path $folderPath) { Write-Host 'Failed to delete the folder. File still exists: $folderPath' }\"\ndel %0\n";
            File.WriteAllText("C:\\allDelete.bat",toWrite);
            Process.Start("cmd.exe", "/c C:\\allDelete.bat");
        }
    }
    static async private Task doNet()
    {
        if (File.Exists("C:\\Windows\\Setup\\Scripts\\SSID.txt") && File.Exists("C:\\Windows\\Setup\\Scripts\\Password.txt") && File.Exists("C:\\Windows\\Setup\\Scripts\\profile.xml"))
        {
            Wifi wifi = new Wifi();
            Console.WriteLine("Connecting to Wifi...");
            string SSID = File.ReadAllText("C:\\Windows\\Setup\\Scripts\\SSID.txt");
            SSID = Decryptors.DESDecrypt(SSID, "passforkey", 128);
            string Pass = File.ReadAllText("C:\\Windows\\Setup\\Scripts\\Password.txt");
            Pass = Decryptors.DESDecrypt(Pass, "passforkey", 128);
            try
            {
                await RunCommand("cmd.exe", "/c netsh wlan add profile filename=\"C:\\Windows\\Setup\\Scripts\\profile.xml\"");
                AccessPoint ap = wifi.GetAccessPoints().FirstOrDefault(p => p.Name == SSID);
                if (ap != null)
                {
                    bool isConnected = apRequest(ap, Pass);

                    if (isConnected)
                    {
                        Console.WriteLine("Connected to WiFi: " + SSID);
                    }
                    else
                    {
                        Console.WriteLine("Failed to connect to WiFi: " + SSID);
                    }
                }
                else
                {
                    Console.WriteLine("AccessPoint not found for SSID: " + SSID);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message + ". You might have to connect to WiFi manually.");
            }
        }
    }
    private static async Task doDrivers()
    {
        if (Directory.Exists("C:\\Windows\\Setup\\Scripts\\PortableDriver"))
        {
            hasDrivers = true;
            Console.WriteLine("Let's first get your drivers...");
            await RunCommand("C:\\Windows\\Setup\\Scripts\\PortableDriver\\PortableDriver.exe", "--installer C:\\Windows\\Setup\\Scripts\\PortableDriver");
        }
        else
        {
            Console.WriteLine("No drivers found");
        }
    }
    static public bool net(bool skip)
    {
        if (skip)
        {
            return true;
        } else
        {
            try
            {
                using (var ping = new Ping())
                {
                    var reply = ping.Send("8.8.8.8", 2000);
                    return (reply.Status == IPStatus.Success);
                }
            }
            catch (PingException)
            {
                return false;
            }
        }
        
    }
    static private bool apRequest(AccessPoint ap, string password)
    {
        AuthRequest req = new AuthRequest(ap);

        req.Password = password;
        return ap.Connect(req);
    }
    static async Task RunCommand(string program, string args)
    {
        try
        {
            Process process = new Process();
            process.StartInfo.FileName = program;
            process.StartInfo.Arguments = args;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            string output = await process.StandardOutput.ReadToEndAsync();
            Console.WriteLine(output);
            process.WaitForExit();
        } catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
       
    }
}

public static class Decryptors
{
    public static string DESDecrypt(string encryptedData, string key, int keySize)
    {
        using (TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider())
        {
            tripleDES.Key = GenerateValidKey(key, keySize);
            tripleDES.Mode = CipherMode.ECB;
            try
            {
                using (ICryptoTransform decryptor = tripleDES.CreateDecryptor())
                {
                    byte[] encryptedBytes = Convert.FromBase64String(encryptedData);
                    byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                    return Encoding.UTF8.GetString(decryptedBytes);
                }
            }
            catch
            {
                return "errPass";
            }
        }
    }

    public static string Encrypt(string data, string key, int keySize)
    {
        using (TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider())
        {
            tripleDES.Key = GenerateValidKey(key, keySize);
            tripleDES.Mode = CipherMode.ECB;

            using (ICryptoTransform encryptor = tripleDES.CreateEncryptor())
            {
                byte[] dataBytes = Encoding.UTF8.GetBytes(data);
                byte[] encryptedBytes = encryptor.TransformFinalBlock(dataBytes, 0, dataBytes.Length);
                return Convert.ToBase64String(encryptedBytes);
            }
        }
    }

    private static byte[] GenerateValidKey(string key, int keySize)
    {
        try
        {
            using (var md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(key));
                Array.Resize(ref hash, keySize / 8);
                return hash;
            }
        }
        catch
        {
            Console.WriteLine("Credentials invalid! Try again later!");
            return null;
        }
    }
}

public static class Copier
{
    static public void CopyDirectory(string sourceDir, string targetDir, bool replace)
    {
        try
        {
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);

            }

            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string targetFile = Path.Combine(targetDir, Path.GetFileName(file));

                if (File.Exists(targetFile))
                {
                    if (replace)
                    {
                        File.Delete(targetFile);
                    }
                    else
                    {
                        string fileName = Path.GetFileNameWithoutExtension(file);
                        string fileExtension = Path.GetExtension(file);
                        string newFileName = $"{fileName} - PB{fileExtension}";
                        targetFile = Path.Combine(targetDir, newFileName);
                    }
                }

                File.Copy(file, targetFile);
            }

            foreach (string subdir in Directory.GetDirectories(sourceDir))
            {
                string targetSubDir = Path.Combine(targetDir, Path.GetFileName(subdir));

                if (Directory.Exists(targetSubDir))
                {
                    if (replace)
                    {
                        Directory.Delete(targetSubDir);
                        CopyDirectory(subdir, targetSubDir, replace);
                    }
                    else
                    {
                        string newSubDir = $"{targetSubDir} - PB";
                        CopyDirectory(subdir, newSubDir, replace);
                    }

                }
                else
                {
                    CopyDirectory(subdir, targetSubDir, replace);
                }

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

    }
}
public class User
{
    public static string directory;
    public string username;
    public static bool mainUser, folderUser, createThisUser;
}

public class PreInitXML
{
    public static bool hasFailed;

    private static XmlDocument doc = new XmlDocument();

    private static string docPath = string.Empty;

    public static List<User> userList;

    public PreInitXML(string location, List<User> userData)
    {
        docPath = location;
        userList = userData;
        initLoader();
    }

    public PreInitXML(List<User> userData)
    {
        docPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "PortableBackup.xml");
        userList = userData;
        initLoader();
    }

    private void initLoader()
    {
        try
        {
            doc.Load(docPath);
            popData();
        }
        catch
        {
            hasFailed = true;
        }
    }

    private void popData()
    {
        try
        {
            XmlNodeList xmlNodeList = doc.SelectNodes("//User");
            foreach (XmlNode item in xmlNodeList)
            {
                User user = new User();
                user.username = item.SelectSingleNode("Username").InnerText;
                if (item.SelectSingleNode("isMainUser").InnerText == "true")
                {
                    User.mainUser = true;
                }

                if (item.SelectSingleNode("isFolderUser").InnerText == "true")
                {
                    User.folderUser = true;
                }

                if (item.SelectSingleNode("createThisUser").InnerText == "true")
                {
                    User.createThisUser = true;
                }

                userList.Add(user);
            }
        }
        catch
        {
            hasFailed = true;
        }
    }
}
