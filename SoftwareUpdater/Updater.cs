using System;
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

        public static Updater CreateInstaller(Service service, int versionCode=-1)
        {
            Updater updater = new Updater(true);

            JsonWrapper.InstallConfigJson installConfigJson = new JsonWrapper.InstallConfigJson().LoadJsonAndReturn();
            
            updater.folderId = installConfigJson._folderId;

            if (versionCode == -1)
                DriveUtils.GetLatestVersionCode(service, updater.folderId, out updater.versionCode);
            else
                updater.versionCode = versionCode;

            return updater;
        }

        public static Updater CreateUpdater(Service service, int versionCode=-1)
        {
            JsonWrapper.VersionJson versionJson = new JsonWrapper.VersionJson().LoadJsonAndReturn();
            
            Updater updater = new Updater(true);

            if (versionCode == -1)
            {
                int availableLatestVerion;
                DriveUtils.GetLatestVersionCode(service, versionJson.folderId, out availableLatestVerion);

                if (versionJson.versionCode >= availableLatestVerion)
                {
                    Console.WriteLine($"Newer version {versionJson.versionName} ({versionCode}) already installed.");
                    return new Updater(false);
                }

                updater.versionCode = availableLatestVerion;
            }
            else
            {
                updater.versionCode = versionCode;
            }

            
            updater.folderId = versionJson.folderId;

            return updater;
        }
        
        private static void UnzipFiles(bool deleteOldFiles)
        {
            string destination = FilePath.RootDir;
            string zipPath = FilePath.TempZipFile;

            if (deleteOldFiles)
            {
                //todo delte the files in old core_file.json
                JsonWrapper.CoreFilesJson json = new JsonWrapper.CoreFilesJson().LoadJsonAndReturn();

                foreach (var fp in json.FileList)
                {
                    File.Delete(fp);
                }
            }

            ZipFile.ExtractToDirectory(zipPath, destination);
        }

        public bool doUpdate(Service service, bool isUpdate)
        {
            if (!valid)
                return false;

            string fileId = DriveUtils.GetIdWithVersionCode(service, folderId, versionCode);

            if(fileId == null)
            {
                Console.WriteLine("Could'nt find the version "+versionCode+".");
                return false;
            }


            if (DriveUtils.DownloadFile(service, FilePath.TempZipFile, fileId))
            {
                UnzipFiles(deleteOldFiles: isUpdate);

                Console.WriteLine($"Updated to version code {versionCode}");
                return true;
            }

            return false;
        }
    }
}
