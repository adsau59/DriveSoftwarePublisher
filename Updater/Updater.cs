using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using CommonScripts;
using Version = CommonScripts.Version;

namespace Updater
{
    /// <summary>
    /// contains all the function needed by Updater.
    /// </summary>
    public class Updater
    {
        private Project _project;

        public Updater()
        {
            _project = Project.ByInstallConfig(
                    InstallConfigJson.Load(FilePath.InstallConfigFile)
                );
        }

        /// <summary>
        /// Install the version specified by code or name,
        /// if both are null latest version is installed
        /// </summary>
        /// <param name="code">Target version code to be installed.</param>
        /// <param name="name">Target version name to be installed.</param>
        public void Install(int code, string name)
        {
            Version version = null;
            if (code > 0)
                version = _project.GetVersion(code);
            else if (!string.IsNullOrEmpty(name))
                version = _project.GetVersion(name);
            else
                version = _project.GetLatestVersion();

            if (version == null)
            {
                Console.WriteLine("Version could not be found.");
                return;
            }

            if (!DriveUtils.DownloadFile(FilePath.TempZipFile, version.FileId))
            {
                Console.WriteLine("Downloding updated failed.");
                return;
            }

            UnzipFiles();

            var configJson = new ConfigJson(_project.SoftwareName, version.VersionCode);
            configJson.SaveJson();

            Console.WriteLine($"Updated to version code {version.VersionName}");
        }

        /// <summary>
        /// Checks for newer version and prints response accordingly.
        /// </summary>
        public void Checkupdate()
        {
            var configJson = ConfigJson.Load();

            Console.WriteLine(configJson.CurrentVersionCode < _project.GetLatestVersion().VersionCode
                ? $"New version {_project.GetLatestVersion()} is available."
                : "Version is up to date.");
        }

        public void ShowVersions()
        {
            Console.WriteLine($"{_project.SoftwareName}\nvVersionCode:VersionName\n");

            var list = new List<Version>(_project.Versions);
            list.Reverse();

            foreach (var version in list)
                Console.WriteLine(version);
        }


        /// <summary>
        /// delete files from previous update
        /// Unzip and copy files
        /// </summary>
        private static void UnzipFiles()
        {
            var destination = FilePath.RootDir;
            var zipPath = FilePath.TempZipFile;
            var archive = ZipFile.Open(zipPath, ZipArchiveMode.Read);

            //check the list of files if exists
            if (File.Exists(FilePath.UpdatedFiles))
            {

                //then delete all those files
                var updatedFilesJson = UpdatedFilesJson.Load();
                updatedFilesJson.UpdatedFiles.ForEach(File.Delete);

            }

            var updatedFiles = new List<string>();
            foreach (var file in archive.Entries)
            {
                var completeFileName = Path.Combine(destination, file.FullName);
                if (file.Name == "")
                {// Assuming Empty for Directory
                    Directory.CreateDirectory(Path.GetDirectoryName(completeFileName));
                    continue;
                }

                System.IO.Directory.CreateDirectory(Path.GetDirectoryName(completeFileName));

                updatedFiles.Add(file.FullName);
                file.ExtractToFile(completeFileName, true);
            }
            //save the list of files extracted
            new UpdatedFilesJson(updatedFiles).SaveJson();

        }
        
    }
}
