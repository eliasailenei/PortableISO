using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Diagnostics;
using System.Security.Cryptography;
using Npgsql;
using BCrypt;

namespace CustomConfig
{
    public class Loader
    {
        protected string scriptLoc, xmlLoc;

        public Loader()
        {
            // scriptLoc = Environment.SystemDirectory;
            scriptLoc = "C:\\Users\\remus\\Videos\\AnyDesk";
            xmlLoc = scriptLoc + "\\config.xml";
        }

        public Loader(string customLoc)
        {
            scriptLoc = customLoc;
            xmlLoc = scriptLoc + "\\config.xml";
        }

        public void setCustomLocation(string customLoc)
        {
            scriptLoc = customLoc;
            xmlLoc = scriptLoc + "\\config.xml";
        }

        public string getScriptLoc()
        {
            return scriptLoc;
        }

        public bool getScriptExistance()
        {
            return File.Exists(xmlLoc);
        }
    }

    public class getExistingData : Loader
    {
        public string serverUsername, serverPasswordEnc, serverKeyEnc, serverKeyPassEnc, applicationLine, CurrentVer, CurrentRel, CurrentLang, OSUsername, OSPassword, diskNumber, usingDomain, DomainLine;
        public bool isOnline;

        private void getText()
        {
            XDocument doc = XDocument.Load(xmlLoc);
            XElement setupData = doc.Descendants("setupData").FirstOrDefault();
            string onlineMode = (string)setupData.Element("IsOnline");
            if (onlineMode.ToLower() == "true")
            {
                isOnline = true;
            }
             serverUsername = (string)setupData.Element("Username");
            serverPasswordEnc = (string)setupData.Element("Password");
            serverKeyEnc = (string)setupData.Element("LoginKey");
            serverKeyPassEnc = (string)setupData.Element("KeyPass");
        }

        public bool xmlStatus()
        {
            try
            {
                getText();
                if (serverUsername == null || serverPasswordEnc == null || serverKeyEnc == null || serverKeyPassEnc == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }

    public class makeData : getExistingData
    {
        public void createNewXML()
        {
            using (XmlWriter writer = XmlWriter.Create(Path.Combine(scriptLoc, "config.xml"), new XmlWriterSettings { Indent = true }))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("EXMLE");
                writer.WriteStartElement("setupData");
                writer.WriteElementString("IsOnline", "true");
                writer.WriteElementString("Username", serverUsername);
                writer.WriteElementString("Password", serverPasswordEnc);
                writer.WriteElementString("LoginKey", serverKeyEnc);
                writer.WriteElementString("KeyPass", serverKeyPassEnc);
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }
    }

    public class Decryptors
    {
        public string DESDecrypt(string encryptedData, string key, int keySize)
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
        public string Encrypt(string data, string key, int keySize)
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

        private byte[] GenerateValidKey(string key, int keySize)
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

    public class SQLCheck : getExistingData
    {
        public object sqlCC(string query)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(serverCreds()))
            {
                try
                {
                    connection.Open();
                    using (NpgsqlCommand selectCommand = new NpgsqlCommand(query, connection))
                    {
                        return selectCommand.ExecuteScalar();
                    }
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }
                finally { connection.Close(); }
            }
        }

        public bool checkValidity()
        {
            return true;
        }

        public bool userLogin(string givenPass)
        {
            try
            {
                string serverPass = sqlCC("SELECT Password FROM Users WHERE Username = '" + serverUsername + "'").ToString();
                if (serverPass != null && BCrypt.Net.BCrypt.Verify(givenPass, serverPass))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        private string serverCreds()
        {
            Decryptors decrypt = new Decryptors();
            string pass = decrypt.DESDecrypt(serverKeyPassEnc, "unlock", 128);
            return decrypt.DESDecrypt(serverKeyEnc, pass, 128);
        }
    }

    public  class remoteData
    {
        protected bool autoStatus;
        SQLCheck sqls;

        public remoteData(SQLCheck sql)
        {
            sqls = sql;
            try
            {
                if (sqls.xmlStatus() && sqls.isOnline)
                {
                    collectOSSetup();
                    collectOSSel();
                    collectNinite();
                    autoStatus = true;
                }
            }
            catch
            {
                autoStatus = false;
            }
        }
        public bool getAutoStatus()
        {
            return autoStatus;
        }
        private void collectOSSetup()
        {
            try
            {
                object result = sqls.sqlCC("select p.DiskNo from setuppref p join users u on p.id = u.id where u.username = '" + sqls.serverUsername + "'");

                if (result != null)
                {
                    string input = result.ToString();
                    if (!string.IsNullOrEmpty(input))
                    {
                        try
                        {
                            sqls.diskNumber = input;
                        }
                        catch (FormatException)
                        {
                            sqls.diskNumber = "-1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            try
            {
                object result = sqls.sqlCC("select p.DomainCommand from setuppref p join users u on p.id = u.id where u.username = '" + sqls.serverUsername + "'");

                if (result != null)
                {
                    string inputs = result.ToString();

                    if (!string.IsNullOrEmpty(inputs))
                    {
                        sqls.DomainLine = inputs;
                        sqls.usingDomain = "true";
                    }
                }
            }
            catch { }

            try
            {
                object result = sqls.sqlCC("select p.OSUser from setuppref p join users u on p.id = u.id where u.username = '" + sqls.serverUsername + "'");

                if (result != null)
                {
                    string inputsss = result.ToString();

                    if (!string.IsNullOrEmpty(inputsss))
                    {
                        sqls.OSPassword = inputsss;
                    }
                }
            }
            catch { }

            try
            {
                object result = sqls.sqlCC("select p.OSPassword from setuppref p join users u on p.id = u.id where u.username = '" + sqls.serverUsername + "'");

                if (result != null)
                {
                    string inputssss = result.ToString();

                    if (!string.IsNullOrEmpty(inputssss))
                    {
                        sqls.OSPassword = inputssss;
                    }
                }
            }
            catch { }


        }
        private void collectOSSel()
        {
            try
            {
                object result = sqls.sqlCC("select p.osversion from preference p join users u on p.id = u.id where u.username = '" + sqls.serverUsername + "'");

                if (result != null)
                {
                    string input = result.ToString();
                    if (!string.IsNullOrEmpty(input))
                    {
                        try
                        {

                            sqls.CurrentVer = input;
                        }
                        catch (FormatException)
                        {
                            sqls.CurrentVer = "-1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            try
            {
                object result = sqls.sqlCC("select p.osrelease from preference p join users u on p.id = u.id where u.username = '" + sqls.serverUsername + "'");

                if (result != null)
                {
                    string inputs = result.ToString();

                    if (!string.IsNullOrEmpty(inputs))
                    {
                        sqls.CurrentLang = inputs;
                    }
                }
            }
            catch { }
            try
            {
                object result = sqls.sqlCC("select p.oslanguage from preference p join users u on p.id = u.id where u.username = '" + sqls.serverUsername + "'");

                if (result != null)
                {
                    string inputsss = result.ToString();

                    if (!string.IsNullOrEmpty(inputsss))
                    {
                        sqls.CurrentLang = inputsss;
                    }
                }
            }
            catch { }

        }

        private void collectNinite()
        {
            try
            {
                object input = sqls.sqlCC("select p.niniteoptions from preference p join users u on p.id = u.id where u.username = '" + sqls.serverUsername + "'");
                if (input != null)
                {
                    string result = input.ToString();
                    sqls.applicationLine = result;
                }
            }
            catch { }
        }
    }

    public class localData : SQLCheck
    {
        public localData()
        {
            if (xmlStatus() && !isOnline)
            {
                XDocument doc = XDocument.Load(xmlLoc);
                XElement niniteOptions = doc.Descendants("niniteOptions").FirstOrDefault();
                if (niniteOptions != null)
                {
                    applicationLine = (string)niniteOptions.Element("ApplicationLine");
                }
                XElement OSDownload = doc.Descendants("OSDownload").FirstOrDefault();
                if (OSDownload != null)
                {
                    CurrentVer = (string)OSDownload.Element("CurrentVersion");
                    CurrentRel = (string)OSDownload.Element("CurrentRelease");
                    CurrentLang = (string)OSDownload.Element("CurrentLanguage");
                }
                XElement OSConfig = doc.Descendants("OSConfig").FirstOrDefault();
                if (OSConfig != null)
                {
                    OSUsername = (string)OSConfig.Element("OSUsername");
                    OSPassword = (string)OSConfig.Element("OSPassword");
                    diskNumber = (string)OSConfig.Element("DiskNumber");
                    usingDomain = (string)OSConfig.Element("IsUsingDomain");
                    DomainLine = (string)OSConfig.Element("DomainLine");
                }
            }
        }
    }
    public class DriveLetters
    {
        public char CLetter, TLetter;
        public DriveLetters()
        {
            CLetter = 'C';
            TLetter = 'T';
        }
        public DriveLetters(char c, char t)
        {
            CLetter = c;
            TLetter = t;
        }
        public DriveLetters(string fileLoc)
        {
            if (File.Exists(Environment.SystemDirectory + "\\driveLetters.txt"))
            {
                string[] letters = File.ReadAllLines(Environment.SystemDirectory + "\\driveLetters.txt");
                if (letters.Length >= 2)
                {
                    CLetter = letters[0][0]; 
                    TLetter = letters[1][0]; 
                }
            }
        }
        public async Task<char[]> GetLettersAsync()
        {
            return await Task.Run(() =>
            {
                DriveInfo[] drives = DriveInfo.GetDrives();
                HashSet<char> takenLetters = new HashSet<char>(drives.Select(d => char.ToUpper(d.Name[0])));
                char[] letters = new char[takenLetters.Count];

                int pointer = 0;
                foreach (char item in takenLetters)
                {
                    letters[pointer] = item;
                    pointer++;
                }

                return letters;
            });
        }
        public async Task<bool> LetterCollision()
        {
            char[] letters = await GetLettersAsync();
            foreach (char item in letters)
            {
                if (item == 'C' || item == 'T')
                {
                    return true;
                }
                break;
            }
            return false;
        }
    }

}
