using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO.Compression;
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

        private static Publisher LoadFromJson()
        {
            JsonWrappers.VersionJson versionJson = new JsonWrappers.VersionJson().LoadJsonAndReturn();

            return new Publisher(versionJson.folderId, versionJson.versionCode, versionJson.versionName,
                versionJson.softwareName);
        }


        public static Publisher CreateUpdatePublisher()
        {
            return Publisher.LoadFromJson();
        }

        //creats new folder in drive and creates a json file in out
        public static Publisher CreateNewPublisher(Service service, string softwareName, string versionName)
        {
            var configJson = new JsonWrappers.ConfigJson().LoadJsonAndReturn();

            if (DriveUtils.FindFolderInRoot(service, configJson.RootFolderId, softwareName))
            {
                Console.WriteLine("Project already exisits can not create new.");
                return null;
            }


            System.IO.Directory.CreateDirectory(FilePath.ConfigDir);

            Publisher publisher = new Publisher(DriveUtils.CreateFolder(service, softwareName, configJson.RootFolderId), 1, versionName, softwareName);
            new JsonWrappers.VersionJson(publisher.folderId, publisher.versionCode, publisher.versionName, publisher.softwareName).SaveJson();
            
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

            new JsonWrappers.InstallConfigJson(publisher.folderId).SaveJson();
            
            new JsonWrappers.CoreFilesJson().SaveJson();

            Console.WriteLine("Created a publisher");
            return publisher;
        }

        public bool AddFileToCoreFilesJson(string filePath)
        {
            JsonWrappers.CoreFilesJson coreFilesJson = new JsonWrappers.CoreFilesJson().LoadJsonAndReturn();

            if (coreFilesJson.FileList.Any(fp => filePath == fp))
            {
                Console.Write("File is already included in core_files.json .");
                return false;
            }
            
            coreFilesJson.FileList.Add(filePath);

            coreFilesJson.SaveJson();

            return true;
        }

        public bool RemoveFileFromCoreFilesJson(string filePath)
        {
            JsonWrappers.CoreFilesJson coreFilesJson = new JsonWrappers.CoreFilesJson().LoadJsonAndReturn();

            if (coreFilesJson.FileList.Any(fp => filePath == fp))
            {
                coreFilesJson.FileList.Remove(filePath);

                coreFilesJson.SaveJson();
                return true;
            }

            Console.WriteLine("File is not included in core_files.");



            return false;
        }

        public void AddAllFilesToCoreFilesJson()
        {

            List<string> files =
                new List<string>(Directory.GetFiles(FilePath.RootDir, "*", SearchOption.AllDirectories));

            files.RemoveAll(elem => elem.Contains(".publish"));

            List<string> newFiles = new List<string>();

            foreach (var file in files)
            {
                newFiles.Add(DriveUtils.GetRelativePath(file));
            }

            new JsonWrappers.CoreFilesJson(newFiles).SaveJson();

        }

        public void RemoveAllFilesFromCoreFIlesJson()
        {
            new JsonWrappers.CoreFilesJson(new List<string>()).SaveJson();
        }

        private static void CreateZiip()
        {
            JsonWrappers.CoreFilesJson coreFilesJson = new JsonWrappers.CoreFilesJson().LoadJsonAndReturn();

            foreach (var filePath in coreFilesJson.FileList)
            {
                System.IO.FileInfo file = new System.IO.FileInfo(FilePath.TempDir+filePath);
                file.Directory?.Create(); // If the directory already exists, this method does nothing.
                System.IO.File.WriteAllText(file.FullName, File.ReadAllText(filePath));
            }

            File.Delete(FilePath.TempZipFile);
            ZipFile.CreateFromDirectory(FilePath.TempDir, FilePath.TempZipFile);
        }

        

        

        public string PublishUpdate(Service service)
        {
            int currentVersion;
            DriveUtils.GetLatestVersionCode(service, folderId, out currentVersion);
            if(currentVersion >= versionCode)
            {
                Console.WriteLine("Version is not new, Version Available: "+currentVersion);
                return null;
            }

            CreateZiip();

            Console.WriteLine($"Version {versionName} ({versionCode}) published successfully.");

            return DriveUtils.UploadFileToCloud(service, versionCode+".zip", folderId, FilePath.TempZipFile);

        }

        public string CreateUpdater(Service service)
        {

            new JsonWrappers.InstallConfigJson(folderId).SaveJson();

            File.Delete(FilePath.TempZipFile);
            ZipFile.CreateFromDirectory(FilePath.UpdaterDir, FilePath.TempZipFile);

            return DriveUtils.UploadFileToCloud(service, "updater.zip", folderId, FilePath.TempZipFile);
        }

        public void IncreaseVersion(string newVersionName)
        {
            Console.WriteLine($"Incrementing version from {versionName} ({versionCode}) to {newVersionName} ({versionCode+1})");

            versionCode++;
            versionName = newVersionName;

            new JsonWrappers.VersionJson(folderId, versionCode, versionName, softwareName).SaveJson();
        }        

        


        
    }
}
