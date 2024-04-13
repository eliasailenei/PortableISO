using contin;
using System;
using System.IO.Compression;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace contin
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1()); 
        }
    }

    public static class ZipFileExtensions // not my code taken from https://tinyurl.com/overwritezipcode
    {
        public static void ExtractToDirectoryWithOverwrite(string sourceArchiveFileName, string destinationDirectoryName, Encoding entryNameEncoding = null)
        {
            using (ZipArchive archive = ZipFile.Open(sourceArchiveFileName, ZipArchiveMode.Read, entryNameEncoding))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    try
                    {
                        string destinationPath = Path.Combine(destinationDirectoryName, entry.FullName);
                        string destinationDirectory = Path.GetDirectoryName(destinationPath);

                        if (!Directory.Exists(destinationDirectory))
                        {
                            Directory.CreateDirectory(destinationDirectory);
                        }

                        if (!string.IsNullOrEmpty(Path.GetFileName(destinationPath))) // Skip directories
                        {
                            entry.ExtractToFile(destinationPath, overwrite: true);
                        }
                    } catch (Exception ex) 
                    {
                        
                    }
                    
                }
            }
        }
    }

}
