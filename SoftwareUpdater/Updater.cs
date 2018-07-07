using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using ClassLibrary1;

namespace SoftwareUpdater
{
    public class Updater
    {
        int versionCode;
        string folderId;
        private bool valid = true;

        private Updater(bool valid)
        {
            this.versionCode = -1;
            this.folderId = "";
            this.valid = valid;
        }

        /**
         * Initializes the Attributes using intall config json.
         */
        public static Updater CreateInstaller(Service service, int versionCode=-1)
        {
            Updater updater = new Updater(true);

            JsonWrapper.InstallConfigJson installConfigJson = new JsonWrapper.InstallConfigJson().LoadJsonAndReturn();
            
            updater.folderId = installConfigJson._folderId;

            if (versionCode == -1)
            {
                updater.versionCode = DriveUtils.GetLatestVersion(service, updater.folderId).versionCode;
                //Console.WriteLine($"{versionCode}");
            }
            else
                updater.versionCode = versionCode;

            return updater;
        }

        /*
         * Initializing the attributes using version json
         */
        public static Updater CreateUpdater(Service service, int versionCode=-1)
        {
            JsonWrapper.UpdaterVersionJson versionJson = new JsonWrapper.UpdaterVersionJson();
            versionJson.LoadJson();
            
            Updater updater = new Updater(true);

            if (versionCode == -1)
            {
                int availableLatestVerion = DriveUtils.GetLatestVersion(service, versionJson.FolderId).versionCode;

                if (versionJson.VersionCode >= availableLatestVerion)
                {
                    Console.WriteLine($"Newer version {versionJson.VersionName} ({versionJson.VersionCode}) already installed.");
                    return new Updater(false);
                }

                updater.versionCode = availableLatestVerion;
            }
            else
            {
                updater.versionCode = versionCode;
            }

            
            updater.folderId = versionJson.FolderId;

            return updater;
        }
        
        /*
         * Unzip and copy files
         */
        private static void UnzipFiles()
        {
            string destination = FilePath.RootDir;
            string zipPath = FilePath.TempZipFile;
            ZipArchive archive = ZipFile.Open(zipPath, ZipArchiveMode.Read);

            //todo check the list of files if exists
            if (File.Exists(FilePath.UpdatedFiles))
            {

                //todo then delete all those files
                JsonWrapper.UpdatedFilesJson updatedFilesJson = new JsonWrapper.UpdatedFilesJson();
                updatedFilesJson.LoadJson();
                updatedFilesJson.updatedFiles.ForEach(s => File.Delete(s));

            }

            List<string> updatedFiles = new List<string>();
            foreach (ZipArchiveEntry file in archive.Entries)
            {
                string completeFileName = Path.Combine(destination, file.FullName);
                if (file.Name == "")
                {// Assuming Empty for Directory
                    Directory.CreateDirectory(Path.GetDirectoryName(completeFileName));
                    continue;
                }

                System.IO.Directory.CreateDirectory(Path.GetDirectoryName(completeFileName));

                //todo save the list of files extracted
                updatedFiles.Add(file.FullName);
                file.ExtractToFile(completeFileName, true);
            }
            new JsonWrapper.UpdatedFilesJson(updatedFiles).SaveJson();

        }

        /*
         * check drive for new version,
         * if there is download and run unzip
         */
        public bool doUpdate(Service service, bool isUpdate)
        {
            if (!valid)
                return false;

            VersionStruct versionStruct;
            string fileId = DriveUtils.GetFileIdWithVersionCode(service, folderId, versionCode, out versionStruct);

            if(fileId == null)
            {
                Console.WriteLine($"Could\'nt find the version {versionCode}.");
                return false;
            }


            if (DriveUtils.DownloadFile(service, FilePath.TempZipFile, fileId))
            {
                UnzipFiles();


                Console.WriteLine($"Updated to version code {versionCode}");

                new JsonWrapper.UpdaterVersionJson(versionStruct).SaveJson();             
                return true;
            }

            return false;
        }
    }
}
