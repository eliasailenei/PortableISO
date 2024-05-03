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
using System.Net;
using System.Text.RegularExpressions;

namespace CustomConfig
{
    public class Loader
    {
        protected string scriptLoc, xmlLoc;

        public Loader() // simple OOP model
        {
            scriptLoc = Environment.SystemDirectory;
            xmlLoc = scriptLoc + "\\config.xml"; // we assume that the script is located at System32 where mainUI is based at
        }

        public Loader(string customLoc)
        {
            scriptLoc = customLoc;
            xmlLoc = scriptLoc + "\\config.xml"; // set it at a custom location initially 
        }

        public void setCustomLocation(string customLoc)
        {
            scriptLoc = customLoc; // the user can change the location of the XML after the class has been initialized
            xmlLoc = scriptLoc + "\\config.xml";
        }

        public string getScriptLoc()
        {
            return scriptLoc; // get location of script
        }

        public bool getScriptExistance()
        {
            return File.Exists(xmlLoc); // see if the file exists
        }
    }

    public class getExistingData : Loader // simple OOP model
    {
        public string serverUsername, serverPasswordEnc, serverKeyEnc, serverKeyPassEnc, applicationLine, CurrentVer, CurrentRel, CurrentLang, OSUsername, OSPassword, diskNumber, usingDomain, DomainLine;
        public bool isOnline;

        private void getText() // we only need the basics. Once we have this, we can use online mode as a backup.
        {
            XDocument doc = XDocument.Load(xmlLoc); // read the XML // writing and reading from files
            XElement setupData = doc.Descendants("setupData").FirstOrDefault();
            string onlineMode = (string)setupData.Element("IsOnline"); // see if its online
            if (onlineMode.ToLower() == "true")
            {
                isOnline = true;
            }
             serverUsername = (string)setupData.Element("Username"); // username for DB
            serverPasswordEnc = (string)setupData.Element("Password"); // password for DB
            serverKeyEnc = (string)setupData.Element("LoginKey"); // the string to unlock access to DB
            serverKeyPassEnc = (string)setupData.Element("KeyPass"); // to decrypt the key
        }

        public bool xmlStatus()
        {
            try
            {
                getText(); // get the text
                if (serverUsername == null || serverPasswordEnc == null || serverKeyEnc == null || serverKeyPassEnc == null)
                {
                    return false; // if any of the fields are empty then its not a valid XML
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return false; // if it can't read the XML then its probably not one made for this program
            }
        }
    }

    public class makeData : getExistingData
    {
        SQLCheck sqls; // get the credentials for the SQL DB
        public makeData(SQLCheck sql)
        {
        sqls = sql; // set the current variable of SQLCheck to the one already made (which has user data)
        }
        public void createNewXML()
        {
            using (XmlWriter writer = XmlWriter.Create(Path.Combine(scriptLoc, "config.xml"), new XmlWriterSettings { Indent = true })) // from here // writing and reading from file
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("EXMLE");
                writer.WriteStartElement("setupData");
                writer.WriteElementString("IsOnline", "true");
                sqls.isOnline = true;
                writer.WriteElementString("Username", sqls.serverUsername);
                writer.WriteElementString("Password", sqls.serverPasswordEnc);
                writer.WriteElementString("LoginKey", sqls.serverKeyEnc);
                writer.WriteElementString("KeyPass", sqls.serverKeyPassEnc);
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }
    } // to here, we create a new XML so that other programs have access to the data without asking the user for login again

    public class Decryptors // there is nothing about cryptography in the spec
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


    public class SQLCheck : getExistingData
    {
        public object sqlCC(string query) // this is to make queries to the SQL DB
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(serverCreds())) // we get the serverCreds by decrypting the key first into a usable string
            {
                try
                { // try to make a connection
                    connection.Open();
                    using (NpgsqlCommand selectCommand = new NpgsqlCommand(query, connection))
                    {
                        return selectCommand.ExecuteScalar(); // give back the user the input
                    }
                }
                catch (Exception ex)
                {
                    return ex.ToString(); // give back the user the error
                }
                finally { connection.Close(); } // close the connection even if it failed
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
                string serverPass = sqlCC("SELECT Password FROM Users WHERE Username = '" + serverUsername + "'").ToString(); // make a query to ask for the hashed password of said username // single table SQL
                if (serverPass != null && BCrypt.Net.BCrypt.Verify(givenPass, serverPass)) // hashing
                {
                    return true; // say that the password is correct if it matches the users one
                }
                else
                {
                    return false; // else say its not right
                }
            }
            catch
            {
                return false; // a failed attempt at hashing or at the query always means that its not the correct password
            }
        }

        private string serverCreds()
        {
            Decryptors decrypt = new Decryptors(); // start the new decryptors
            string pass = decrypt.DESDecrypt(serverKeyPassEnc, "unlock", 128); // decrypt the server key with password "unlock"
            return decrypt.DESDecrypt(serverKeyEnc, pass, 128); // return the result
        }
    }

    public  class remoteData
    {
        protected bool autoStatus;
        SQLCheck sqls; // make a new SQLCheck variable

        public remoteData(SQLCheck sql)
        {
            sqls = sql; // get the existing variables imported
            try
            {
                    collectOSSetup(); // set the data for OSSetup
                    collectOSSel(); // set the data for OSSelect
                    collectNinite(); // set the data for Ninite
                makeData make = new makeData(sqls); // import the data
                make.createNewXML(); // create a new XML
                    autoStatus = true;
                
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
        // from here
        private void collectOSSetup()
        {
            try
            {
                object result = sqls.sqlCC("select p.DiskNo from setuppref p join users u on p.id = u.id where u.username = '" + sqls.serverUsername + "'"); // Cross-table parameterised SQL

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
                object result = sqls.sqlCC("select p.DomainCommand from setuppref p join users u on p.id = u.id where u.username = '" + sqls.serverUsername + "'");// Cross-table parameterised SQL

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
                object result = sqls.sqlCC("select p.OSUser from setuppref p join users u on p.id = u.id where u.username = '" + sqls.serverUsername + "'");// Cross-table parameterised SQL

                if (result != null)
                {
                    string inputsss = result.ToString();

                    if (!string.IsNullOrEmpty(inputsss))
                    {
                        sqls.OSUsername = inputsss;
                    }
                }
            }
            catch { }

            try
            {
                object result = sqls.sqlCC("select p.OSPassword from setuppref p join users u on p.id = u.id where u.username = '" + sqls.serverUsername + "'");// Cross-table parameterised SQL

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
                object result = sqls.sqlCC("select p.osversion from preference p join users u on p.id = u.id where u.username = '" + sqls.serverUsername + "'");// Cross-table parameterised SQL

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
                object result = sqls.sqlCC("select p.osrelease from preference p join users u on p.id = u.id where u.username = '" + sqls.serverUsername + "'");// Cross-table parameterised SQL

                if (result != null)
                {
                    string inputs = result.ToString();

                    if (!string.IsNullOrEmpty(inputs))
                    {
                        sqls.CurrentRel = inputs;
                    }
                }
            }
            catch { }
            try
            {
                object result = sqls.sqlCC("select p.oslanguage from preference p join users u on p.id = u.id where u.username = '" + sqls.serverUsername + "'");// Cross-table parameterised SQL

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
                object input = sqls.sqlCC("select p.niniteoptions from preference p join users u on p.id = u.id where u.username = '" + sqls.serverUsername + "'");// Cross-table parameterised SQL
                if (input != null)
                {
                    string result = input.ToString();
                    sqls.applicationLine = result;
                }
            }
            catch { }
        }
    }
    // to here, we do many queries to get the data needed and save them into variables that the programs can use
    public class localData : SQLCheck
    {
        public localData(SQLCheck sql)
        {
            if (xmlStatus() && !isOnline) // create a XML with offline data
            {
                XDocument doc = XDocument.Load(xmlLoc); // writing and reading from file
                XElement niniteOptions = doc.Descendants("niniteOptions").FirstOrDefault();
                if (niniteOptions != null)
                {
                    sql.applicationLine = (string)niniteOptions.Element("ApplicationLine");
                }
                XElement OSDownload = doc.Descendants("OSDownload").FirstOrDefault();
                if (OSDownload != null)
                {
                   sql.CurrentVer = (string)OSDownload.Element("CurrentVersion");
                    sql.CurrentRel = (string)OSDownload.Element("CurrentRelease");
                    sql.CurrentLang = (string)OSDownload.Element("CurrentLanguage");
                }
                XElement OSConfig = doc.Descendants("OSConfig").FirstOrDefault();
                if (OSConfig != null)
                {
                    sql.OSUsername = (string)OSConfig.Element("OSUsername");
                    sql.OSPassword = (string)OSConfig.Element("OSPassword");
                    sql.diskNumber = (string)OSConfig.Element("DiskNumber");
                    sql.usingDomain = (string)OSConfig.Element("IsUsingDomain");
                    sql.DomainLine = (string)OSConfig.Element("DomainLine");
                }
            }
        }
    }
    public class DriveLetters
    {
        public char CLetter, TLetter;
        public DriveLetters() // by default we have C as our C letter and T as our T letter
        {
            CLetter = 'C';
            TLetter = 'T';
        }
        public DriveLetters(char c, char t) // the C and T letter is user defined
        {
            CLetter = c;
            TLetter = t;
        }
        public DriveLetters(string fileLoc) // we use this method when the driverLetters were made previously by setup
        {
            if (File.Exists(Environment.SystemDirectory + "\\driveLetters.txt")) // read text file
            {
                string[] letters = File.ReadAllLines(Environment.SystemDirectory + "\\driveLetters.txt"); // writing and reading from files
                if (letters.Length >= 2) // set the variables
                {
                    CLetter = letters[0][0]; 
                    TLetter = letters[1][0]; 
                }
            } else // sometimes there is no file or its corrupted, we rely on the defaults at this point
            {
                CLetter = 'C';
                TLetter = 'T';
            }
        }
        public async Task<char[]> GetLettersAsync() // gets all the drive letters that are on the system
        {
            return await Task.Run(() =>
            {
                DriveInfo[] drives = DriveInfo.GetDrives(); // get all the drives
                HashSet<char> takenLetters = new HashSet<char>(drives.Select(d => char.ToUpper(d.Name[0]))); // only keep the drive letters and they must be unique 
                char[] letters = new char[takenLetters.Count];

                int pointer = 0;
                foreach (char item in takenLetters)
                {
                    letters[pointer] = item; // set the array with the letters
                    pointer++;
                }

                return letters;
            });
        }
        public async Task<bool> LetterCollision()
        {
            char[] letters = await GetLettersAsync(); // get the letters
            foreach (char item in letters) // check if there is at least one collision, this means that setup was done without success or there is already a Windows drive present
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
    public class activationLib
    {
        public Dictionary<string,bool> getBlacklist() // make a dictionary for easy search
        {
            Dictionary<string, bool> blacklist = new Dictionary<string, bool>(); // declear a new dictionary
            try
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile("https://raw.githubusercontent.com/Ja7ad/PIDChecker/master/blockedKey", "blacklist.txt"); // download the blacklist from a users repo
                }
                string[] lines = File.ReadAllLines("blacklist.txt");
                foreach (string line in lines)
                {
                    blacklist[line] = true; // add the data to the dictionary 
                }
                return blacklist;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public bool fitPattern(string key)
        {
            Regex regex = new Regex(@"\b([a-zA-Z1-9]{5}-){4}[a-zA-Z1-9]{5}\b"); // this regex states that there must be charaters for each entry for key followed by a dash so 12345-ABCDE....
            if (regex.IsMatch(key))
            {
                return true;
            } else
            {
                return false;
            }
        }
    }
    public class globalStrings
    {
        public static string windowsKey { get; set; }
    }

}
