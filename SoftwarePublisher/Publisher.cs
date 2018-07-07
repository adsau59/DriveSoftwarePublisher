using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO.Compression;
using System.Text.RegularExpressions;
using ClassLibrary1;
using File = System.IO.File;

//ReSharper Disable All

namespace SoftwarePublisher
{
    public class Publisher
    {
        public string folderId;
        public int versionCode;
        public string versionName;
        public string softwareName;

        
        public Publisher(string folderId, int versionCode, string versionName, string softwareName)
        {
            this.folderId = folderId;
            this.versionCode = versionCode;
            this.versionName = versionName;
            this.softwareName = softwareName;
        }

        private Publisher()
        {
            folderId = string.Empty;
            versionCode = -1;
            versionName = string.Empty;
            softwareName = string.Empty;
        }

        /// <summary>
        /// Creates Publisher object using version json
        /// </summary>
        /// <returns>Publisher object</returns>
        public static Publisher LoadFromJson()
        {
            JsonWrapper.PublisherVersionJson versionJson = new JsonWrapper.PublisherVersionJson();
            versionJson.LoadJson();

            return new Publisher(versionJson.FolderId, versionJson.VersionCode, versionJson.VersionName,
                versionJson.SoftwareName);
        }


        /// <summary>
        /// creats new folder in drive
        /// and creates a json file in out</summary>
        /// <param name="service">driver service</param>
        /// <param name="softwareName">Name of the software</param>
        /// <param name="versionName">version name of the new software</param>
        /// <returns>Publisher object</returns>
        public static Publisher CreateNewPublisher(Service service, string softwareName, string versionName)
        {
            var configJson = new JsonWrapper.ConfigJson().LoadJsonAndReturn();


            //if configJson doesnt exists find for SoftwarePublisher folder, if it doesnt exists create new
            if (!File.Exists(FilePath.ConfigJsonFile))
            {
                string folderid;

                if (!DriveUtils.FindFolderInRoot(service, "root", "DefineXSoftwarePublisher", out folderid))
                    folderid = DriveUtils.CreateFolder(service, "DefineXSoftwarePublisher", "root");

                configJson.RootFolderId = folderid;

                Console.WriteLine("Creating config.json");
                configJson.SaveJson();

            }

            if (DriveUtils.FindFolderInRoot(service, configJson.RootFolderId, softwareName))
            {
                Console.WriteLine("Project already exisits can not create new.");
                return null;
            }


            System.IO.Directory.CreateDirectory(FilePath.ConfigDir);

            Publisher publisher = new Publisher(DriveUtils.CreateFolder(service, softwareName, configJson.RootFolderId), 1, versionName, softwareName);
            new JsonWrapper.PublisherVersionJson(publisher.folderId, publisher.versionCode, publisher.versionName, publisher.softwareName).SaveJson();
            
            var sourcePath = FilePath.BaseUpdaterDir;
            var destinationPath = FilePath.UpdaterDir;

            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*",
                SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath));

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*",
                SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(sourcePath, destinationPath), true);

            new JsonWrapper.PublisherInstallConfigJson(publisher.folderId).SaveJson();

            Console.WriteLine("Created a publisher");
            return publisher;
        }

        /// <summary>
        /// Copies the files to temp directory,
        /// and zips it
        /// </summary>
        private static void CreateZiip()
        {
            List<string> filePaths =
                new List<string>(Directory.GetFiles(FilePath.RootDir, "*", SearchOption.AllDirectories));

            for (int i = 0; i < filePaths.Count; i++)
                filePaths[i] = filePaths[i].Remove(filePaths[i].IndexOf(FilePath.RootDir), FilePath.RootDir.Length);

            filePaths.RemoveAll(elem => elem.Contains(".publish"));
            filePaths.RemoveAll(elem => elem.Contains(".pubignore"));

            //todo .pubignore
      
            //if .pubignore exists
            if (File.Exists(FilePath.IgnoreFile))
            {
                //itrate on each line
                String[] ignoreList = File.ReadAllLines(FilePath.IgnoreFile);

                foreach (var ignore in ignoreList)
                    //do regex on each filepath
                    //if regex is true remove the file
                    filePaths.RemoveAll(s => Regex.Match(s, ignore, RegexOptions.IgnoreCase).Success);
            }

            Utils.Empty(new DirectoryInfo(FilePath.TempDir));
            foreach (var filePath in filePaths)
            {
                Console.WriteLine($"Adding: {filePath}");
                System.IO.FileInfo file = new System.IO.FileInfo(FilePath.TempDir+ filePath);
                file.Directory?.Create();
                System.IO.File.WriteAllText(file.FullName, File.ReadAllText(filePath));
            }

            File.Delete(FilePath.TempZipFile);
            ZipFile.CreateFromDirectory(FilePath.TempDir, FilePath.TempZipFile);
        }



        /// <summary>
        /// Checks if version is latest,
        /// creates zip,
        /// </summary>
        /// uploads the file<param name="service">driver service</param>
        /// <returns>drive file id of the uploaded file</returns>
        public string PublishUpdate(Service service)
        {

            int currentVersion = DriveUtils.GetLatestVersion(service, folderId).versionCode;

            if(currentVersion >= versionCode)
            {
                Console.WriteLine("Version is not new, Version Available: "+currentVersion);
                return null;
            }

            CreateZiip();

            Console.WriteLine($"Version {versionName} ({versionCode}) published successfully.");

            string name = $"{versionCode.ToString()}--{softwareName}--{versionName}";

            return DriveUtils.UploadFileToCloud(service, name + ".zip", folderId, FilePath.TempZipFile);

        }


        /// <summary>
        /// Creates updater and uploads it to drive
        /// </summary>
        /// <param name="service">driver service</param>
        /// <returns>file id of the update.zip file</returns>
        public string CreateUpdater(Service service)
        {

            new JsonWrapper.PublisherInstallConfigJson(folderId).SaveJson();

            File.Delete(FilePath.TempZipFile);
            ZipFile.CreateFromDirectory(FilePath.UpdaterDir, FilePath.TempZipFile);

            return DriveUtils.UploadFileToCloud(service, "updater.zip", folderId, FilePath.TempZipFile);
        }


        /// <summary>
        /// Increases the version and updates the version name in the version.json
        /// </summary>
        /// <param name="newVersionName">version name to update</param>
        public void IncreaseVersion(string newVersionName)
        {
            Console.WriteLine($"Incrementing version from {versionName} ({versionCode}) to {newVersionName} ({versionCode+1})");

            versionCode++;
            versionName = newVersionName;

            new JsonWrapper.PublisherVersionJson(folderId, versionCode, versionName, softwareName).SaveJson();
        }        

        


        
    }
}
