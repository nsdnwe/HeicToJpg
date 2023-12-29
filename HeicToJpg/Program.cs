using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

// NSD Oy Finland - MIT license
// First install: https://imagemagick.org/script/download.php#windows

namespace HeicToJpg {
    internal class Program {
        private static int filesConverted = 0;

        static void Main(string[] args) {
            if (args.Any(z => z == "-h") || args.Any(z => z == "-?")) helpText();

            Console.WriteLine("\r\nCommand-line HEIC to JPG converter");
            Console.WriteLine("\r\nHelp: HeicToJpg -h \r\n");

            bool subFolders = args.Any(z => z == "-s");
            bool overwriteJpgs = args.Any(z => z == "-o");
            bool deleteConvertedHeics = args.Any(z => z == "-d");
            bool deleteHeics = args.Any(z => z == "-dheic");
            bool deleteImgs = args.Any(z => z == "-dimg");
            bool deleteImges = args.Any(z => z == "-dimge");
            bool deleteAaes = args.Any(z => z == "-daae");
            bool moveFiles = args.Any(z => z == "-mbcd");
            string baseFolder = System.IO.Directory.GetCurrentDirectory();

            // If move, no other action is taken
            if (moveFiles) {
                moveFilesByCreateDate(baseFolder);
                Environment.Exit(0);
            }

            if (deleteImgs && deleteImges) {
                Console.Write("Don't delete both IMG_ and IMG_E files");
                Environment.Exit(0);
            }

            // First current directory
            processDirectory(baseFolder, overwriteJpgs, deleteConvertedHeics, deleteHeics, deleteImgs, deleteImges, deleteAaes);

            // Then all subdirectories
            if (subFolders) {
                foreach (var directory in Directory.GetDirectories(baseFolder)) {
                    processDirectory(directory, overwriteJpgs, deleteConvertedHeics, deleteHeics, deleteImgs, deleteImges, deleteAaes);
                }
            }

            Console.WriteLine("\r\n" + filesConverted.ToString() + " files converted");
        }

        // Process single directory
        private static void processDirectory(string directory, bool overwriteJpgs, bool deleteConvertedHeics, bool deleteHeics, bool deleteImgs, bool deleteImges, bool deleteAaes) {
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

                    // Set creation date by original file
                    DateTime creationTime = File.GetCreationTime(directory + "\\" + file.Name);
                    DateTime modificationTime = File.GetLastWriteTime(directory + "\\" + file.Name);
                    File.SetCreationTime(directory + "\\" + newFileName, creationTime);
                    File.SetLastWriteTime(directory + "\\" + newFileName, modificationTime);

                    // Delete heic after converting
                    if (deleteConvertedHeics) {
                        file.Delete();
                        Console.Write("and deleted");
                    }
                    oneFileFound = true;
                    filesConverted++;
                }
            }

            if (oneFileFound) Console.WriteLine("\r\n");

            // Delete IMG_ files if IMG_E exists
            if (deleteHeics) {
                Console.WriteLine("Deleting HEIC files if JPG file exists");
                FileInfo[] files3 = di.GetFiles("*.HEIC");
                foreach (FileInfo file in files3) {
                    string fileName = file.Name.Replace(".HEIC", ".heic");
                    string newFileName = fileName.Replace(".heic", ".jpg");
                    if (File.Exists(directory.ToString() + "\\" + newFileName)) file.Delete();
                }
            }

            // Delete IMG_ files if IMG_E exists
            if (deleteImgs) {
                Console.WriteLine("Deleting IMG_ files if IMG_E exists");
                FileInfo[] files3 = di.GetFiles("*.JPG");
                foreach (FileInfo file in files3) {
                    if (file.Name.StartsWith("IMG_") && !file.Name.StartsWith("IMG_E")) {
                        string checkFile = file.Name.Replace("IMG_", "IMG_E");
                        if (File.Exists(directory.ToString() + "\\" + checkFile)) file.Delete();
                    }
                }
            }

            // Delete IMG_E files if IMG_ exists
            if (deleteImges) {
                Console.WriteLine("Deleting IMG_E files if IMG_ exists");
                FileInfo[] files3 = di.GetFiles("*.JPG");
                foreach (FileInfo file in files3) {
                    if (file.Name.StartsWith("IMG_E")) {
                        string checkFile = file.Name.Replace("IMG_E", "IMG_");
                        if (File.Exists(directory.ToString() + "\\" + checkFile)) file.Delete();
                    }
                }
            }

            // Delete AAE files
            if (deleteAaes) {
                Console.WriteLine("Deleting AAE files");
                FileInfo[] files2 = di.GetFiles("*.AAE");
                foreach (FileInfo file in files2) file.Delete();
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

        private static void moveFilesByCreateDate(string directory) {
            Console.WriteLine("Processing folder " + directory);

            DirectoryInfo di = new DirectoryInfo(directory.ToString());
            FileInfo[] files = di.GetFiles("*.jpg");
            bool oneFileFound = false;

            // Loop all the files in this directory
            foreach (FileInfo file in files) {
                string fileName = file.Name;
                DateTime creationTime = File.GetCreationTime(directory + "\\" + file.Name);
                string targetDirectory = directory + "\\" + creationTime.ToString("yyyy-MM-dd");
                if (!Directory.Exists(targetDirectory)) Directory.CreateDirectory(targetDirectory);

                if (!File.Exists(targetDirectory + "\\" + fileName)) {
                    File.Move(directory + "\\" + fileName, targetDirectory + "\\" + fileName);
                    Console.WriteLine(fileName + " moved to \\" + creationTime.ToString("yyyy-MM-dd"));
                } else {
                    Console.WriteLine(fileName + " already exists in target directory \\" + creationTime.ToString("yyyy-MM-dd"));
                }
            }
        }

        // Help and available attributes
        private static void helpText() {
            string helpText = @"
---------------------------------- 
Command-line HEIC to JPG converter
----------------------------------

-s      Process subfolders
-o      Overwrite existing JPG files
-d      Delete HEIC files after converting to JPG

Sample: heictojpg -s -o -d

-------------------------
Patch deletion attributes
-------------------------

-dheic  Delete HEIC files if JPG file exists
-dimg   Delete IMG_ files (4:3) if IMG_E file (16:9) exists
-dimge  Delete IMG_E files (16:9) files if IMG_ file (4:3) exists
-daae   Delete AAE files

Sample: heictojpg -s -dheic -dimg -daae

--------------------
Patch move JPG files
--------------------

-mbcd   Move JPG files to folders by JPG file creation date. Create folder if needed

Sample: heictojpg -mbcd

GitHub: https://github.com/nsdnwe/HeicToJpg
";
            Console.WriteLine(helpText);
            Environment.Exit(0);
        }
    }
}
    