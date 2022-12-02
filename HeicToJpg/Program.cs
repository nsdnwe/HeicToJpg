using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

// MIT license
// First install: https://imagemagick.org/script/download.php#windows

namespace HeicToJpg {
    internal class Program {
        private static int filesConverted = 0;

        static void Main(string[] args) {
            if(args.Any(z => z == "-h") || args.Any(z => z == "-?")) helpText();

            Console.WriteLine("\r\nCommand line HEIC to JPG converter");
            Console.WriteLine("\r\nHelp: HeicToJpg -h \r\n");

            bool subFolders = args.Any(z => z == "-s");
            bool overwriteJpgs = args.Any(z => z == "-o");
            bool deleteHeics = args.Any(z => z == "-d");
            bool deleteAae = args.Any(z => z == "-daae");

            string baseFolder = System.IO.Directory.GetCurrentDirectory();

            // First current directory
            processDirectory(baseFolder, overwriteJpgs, deleteHeics, deleteAae);

            // Then all subdirectories
            if (subFolders) {
                foreach (var directory in Directory.GetDirectories(baseFolder)) {
                    processDirectory(directory, overwriteJpgs, deleteHeics, deleteAae);
                }
            }

            Console.WriteLine("\r\n" + filesConverted.ToString() + " files converted");
        }

        // Process single directory
        private static void processDirectory(string directory, bool overwriteJpgs, bool deleteHeic, bool deleteAae) {
            Console.WriteLine("Processing folder " + directory);

            DirectoryInfo di = new DirectoryInfo(directory.ToString());
            FileInfo[] files = di.GetFiles("*.HEIC");
            bool oneFileFound = false;

            // Loop all the files in this directory
            foreach (FileInfo file in files) {
                string fileName = file.Name.Replace(".HEIC", ".heic");
                string newFileName = fileName.Replace(".heic", ".jpg");
                bool jpgExists = File.Exists(directory.ToString() + "\\" + newFileName);

                // Convert if no jpg already exist or overwrite attribute
                if (!jpgExists || overwriteJpgs) {
                    string command = "magick \"" + directory + "\\" + file.Name + "\" \"" + directory + "\\" + newFileName + "\"";
                    runCommand(command);
                    Console.Write(fileName + " converted ");

                    if (deleteHeic) {
                        file.Delete();
                        Console.Write("and deleted");
                    }
                    oneFileFound = true;
                    filesConverted++;  
                }
            }

            if (oneFileFound) Console.WriteLine("\r\n");

            // Special delete for iPhone 16:9 images
            if (deleteAae) {
                Console.WriteLine("Deleting AAE and IMG_E files");

                FileInfo[] files2 = di.GetFiles("*.AAE");
                foreach (FileInfo file in files2) file.Delete(); 

                FileInfo[] files3 = di.GetFiles("*.JPG");
                foreach (FileInfo file in files3) if (file.Name.StartsWith("IMG_E")) file.Delete();

                Console.WriteLine("");
            }
        }

        // Run magick with parameters
        public static void runCommand(string command) {
            ProcessStartInfo procStartInfo = new ProcessStartInfo("cmd", "/c " + command);
            procStartInfo.RedirectStandardInput = true;
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.RedirectStandardError = true;
            procStartInfo.CreateNoWindow = true;
            procStartInfo.UseShellExecute = false;

            Process proc = new System.Diagnostics.Process();
            proc.StartInfo = procStartInfo;
            proc.Start();
            proc.StandardInput.Flush();
            proc.StandardInput.Close();
            proc.WaitForExit();

            string result = proc.StandardError.ReadToEnd().ToString();
            Console.WriteLine(result);

            if (result != "") {
                Console.WriteLine("Check that imagemagic is installed and available in path.");
                Console.WriteLine("Download from https://imagemagick.org/script/download.php#windows");
                Environment.Exit(0);
            }
        }

        // Help and available attributes
        private static void helpText() {
            string helpText = @"
Command line HEIC to JPG converter

-s      Process subfolders
-o      Overwrite existing JPG files
-d      Delete HEIC files after converting to JPG
-daae   Delete AAD and IMG_E files

Sample: heictojpg -s -o -d

GitHub: https://github.com/nsdnwe/HeicToJpg
";
            Console.WriteLine(helpText);
            Environment.Exit(0);
        }
    }
}
    