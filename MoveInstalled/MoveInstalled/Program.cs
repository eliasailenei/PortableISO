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
        if (!File.Exists("C:\\yes.txt")) // if this txt file does not exist, then this means that its the first time running the program
        {
            Console.WriteLine("Hey!");
            Console.WriteLine("");
            Console.WriteLine("We are getting ready! Please wait...");

            foreach (string drive in drives) // checks if the folder PortableDriver exists
            {
                if (Directory.Exists(drive + "contin\\PortableDriver")) // does it exist?
                {
                    continLoc = drive + "contin\\PortableDriver"; // assume that this is the location of PortableDriver
                    hasDrivers = true;
                    Copier.CopyDirectory(continLoc,"C:\\Windows\\Setup\\Scripts\\PortableDriver",false); // copy the folders to the setup Folder
                    Console.WriteLine("It looks like you have PortabelDriver setup! Lets import that...");
                    break;
                }
            }
            Console.WriteLine("Erasing your temp drive");
            await RunCommand("cmd.exe", "/c C:\\tempdelete.bat"); // erase the temp file
            Console.WriteLine("We are now going to continue the setup after you have logged on!");
            await RunCommand("cmd.exe", "/creg add \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\RunOnce\" /v PortableISO /t REG_SZ /d \"C:\\Windows\\Setup\\Scripts\\MoveInstalled.exe\" /f"); // add a new reg key in RunOnce to make this program run after the user has logged in
            File.WriteAllText("C:\\yes.txt", "begin");
        }
        else
        {
            Console.WriteLine("Welcome back! Please don't close me!");
           await doDrivers(); // run the driver setup if possible, this is so we can run the pre downloaded drivers like Wi-Fi drivers in order for setup to run properly
           await doNet(); // auto connect the user to Wi-Fi if possible
            while (!net(skipNet))
            { // put the user in a loop unill they have internet
                Console.Clear();
                Console.WriteLine("No internet! Please insert the ethernet cable back or connect to WiFi! You can type \"skip\" if you want to continue anyways... Press enter to rescan");
                string inp = Console.ReadLine();
                if (inp == "skip") // the user can also skip this if they have the drivers in PortableDriver
                {
                    skipNet = true;
                }
            }
            if (File.Exists("C:\\donotreopen.txt")) // tell the software to run the drivers again
            {
                File.Delete("C:\\donotreopen.txt");
            }
            else
            {
              await  doDrivers(); // start the actual driver setup
            }
            if (File.Exists("C:\\Windows\\Setup\\Scripts\\windowskey.txt")) //if a key is present then activate it 
            {
                Console.WriteLine("Activating Windows...");
                string key = File.ReadAllText("C:\\Windows\\Setup\\Scripts\\windowskey.txt");
                key = Decryptors.DESDecrypt(key, "passforkey", 128); // decrypt the windows key
                try
                {
                    await RunCommand(@"powershell.exe", $"-Command \"Set-WindowsProductKey -Path 'C:\\' -ProductKey '{key}'\"");// actual PS code to add the key to windows
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message + ". Please activate manually!"); // sometimes the user gives us a bad key, in this case we tell them to activate manually
                }

            }
            if (File.Exists("C:\\Windows\\Setup\\Scripts\\Ninite.exe") && !hasDrivers) // Some users choose to skip PortableDriver but still have chosen to pre install programs 
            {
                Console.WriteLine("Let's first get your programs");
                await RunCommand("C:\\Windows\\Setup\\Scripts\\autorun.exe", "C:\\Windows\\Setup\\Scripts\\autorun.au3"); // run the setup
            }
            string[] allFiles = Directory.GetFiles("C:\\", "*.bin"); // get all bin files in the root directory
            List<string> files = new List<string>();    
            foreach (string file in allFiles)
            {
                if (File.Exists(file) && file.Contains("PB")) // add all files that contain PB in them
                {
                    files.Add(file);
                }
            }
            if (files.Count > 1 || files.Count == 0 ) // if there is more than one backup file, we don't know which one to target
            {
                Console.WriteLine("Sorry, either there is nothing to backup or you don't meet the requirements");
            }
            else
            {
                Process.Start("https://github.com/eliasailenei/PortableBackup/releases/download/V1/Release.zip"); // if its found, start by downloading the latest verion of PortableBackup
            }
            string toWrite = "@echo off\necho Please wait, doing cleanup\ndel C:\\yes.txt\necho Unfortunatly, Windows does not know when the setup is done. Press any key to delete the setup folder once done.\npause\npowershell -Command \"$folderPath = 'C:\\Windows\\Setup\\Scripts'; $retryDurationSeconds = 5; $maxRetryDurationSeconds = 60; $startTime = Get-Date; $endTime = $startTime.AddSeconds($maxRetryDurationSeconds); while ((Test-Path $folderPath) -and ((Get-Date) -lt $endTime)) { try { Remove-Item -Path $folderPath -Recurse -Force -ErrorAction Stop } catch { Start-Sleep -Seconds $retryDurationSeconds } }; if (Test-Path $folderPath) { Write-Host 'Failed to delete the folder. File still exists: $folderPath' }\"\ndel %0\n"; // it firsts deletes the yes.txt from the root drive, then runs a PS code which states that try to delete everything from the location give, wait 60 seconds for program to finish and have 5 tries before there is a time out, lastly self delete
            File.WriteAllText("C:\\allDelete.bat",toWrite); // creates a cleanup script
            Process.Start("cmd.exe", "/c C:\\allDelete.bat"); // runs it at the end
        }
    }
    static async private Task doNet()
    {
        if (File.Exists("C:\\Windows\\Setup\\Scripts\\SSID.txt") && File.Exists("C:\\Windows\\Setup\\Scripts\\Password.txt") && File.Exists("C:\\Windows\\Setup\\Scripts\\profile.xml")) // makes sure we have the nessesary data like the SSID, password and the profile
        {
            Wifi wifi = new Wifi(); // init a new Wi-Fi class
            Console.WriteLine("Connecting to Wifi...");
            string SSID = File.ReadAllText("C:\\Windows\\Setup\\Scripts\\SSID.txt");
            SSID = Decryptors.DESDecrypt(SSID, "passforkey", 128); // read and decrypt the SSID
            string Pass = File.ReadAllText("C:\\Windows\\Setup\\Scripts\\Password.txt");
            Pass = Decryptors.DESDecrypt(Pass, "passforkey", 128); // read and decrypt the password
            try
            {
                await RunCommand("cmd.exe", "/c netsh wlan add profile filename=\"C:\\Windows\\Setup\\Scripts\\profile.xml\""); // add the profile so that Windows can be able to connect to the Wi-Fi
                AccessPoint ap = wifi.GetAccessPoints().FirstOrDefault(p => p.Name == SSID); // see if the SSID even exists
                if (ap != null) // if so continue
                {
                    bool isConnected = apRequest(ap, Pass); // try to connect to wifi

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
        if (Directory.Exists("C:\\Windows\\Setup\\Scripts\\PortableDriver")) // if folder exists then run the PortableDriver in Installer mode
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
        if (skip) // if the user wants to skip internet check say that there is internet
        {
            return true;
        } else
        {
            try
            {
                using (var ping = new Ping())
                {
                    var reply = ping.Send("8.8.8.8", 2000); // ping google
                    return (reply.Status == IPStatus.Success); // if the code is 200 then return true
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
        AuthRequest req = new AuthRequest(ap); // makes a new request

        req.Password = password; // puts password
        return ap.Connect(req); // returns if the connection was successfil or not
    }
    static async Task RunCommand(string program, string args)
    {
        try
        { // run a program and return its output as string
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

    public static string Encrypt(string data, string key, int keySize)
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

    private static byte[] GenerateValidKey(string key, int keySize)
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

public static class Copier
{
    static public void CopyDirectory(string sourceDir, string targetDir, bool replace)
    {
        try
        {
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir); // create the folder if needed
            }

            // Copy files from source to target directory
            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string targetFile = Path.Combine(targetDir, Path.GetFileName(file));

                if (File.Exists(targetFile))
                {
                    // If file already exists in target directory
                    if (replace)
                    {
                        File.Delete(targetFile); // replace existing file
                    }
                    else
                    {
                        // Rename the file to avoid overwriting
                        string fileName = Path.GetFileNameWithoutExtension(file);
                        string fileExtension = Path.GetExtension(file);
                        string newFileName = $"{fileName} - pb{fileExtension}";
                        targetFile = Path.Combine(targetDir, newFileName);
                    }
                }

                File.Copy(file, targetFile); // copy the file to target directory
            }

            // Recursively copy subdirectories
            foreach (string subdir in Directory.GetDirectories(sourceDir))
            {
                string targetSubDir = Path.Combine(targetDir, Path.GetFileName(subdir));

                if (Directory.Exists(targetSubDir))
                {
                    // If subdirectory exists in target directory
                    if (replace)
                    {
                        Directory.Delete(targetSubDir, true); // delete existing subdirectory
                        CopyDirectory(subdir, targetSubDir, replace); // copy with replacement
                    }
                    else
                    {
                        // Rename the subdirectory to avoid overwriting
                        string newSubDir = $"{targetSubDir} - pb";
                        CopyDirectory(subdir, newSubDir, replace); // copy to new renamed subdirectory
                    }

                }
                else
                {
                    // If subdirectory does not exist, copy recursively
                    CopyDirectory(subdir, targetSubDir, replace);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString()); // log any exceptions
        }
    }

}



