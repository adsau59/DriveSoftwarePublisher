using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using CommonScripts;
using Publisher;
using Version = CommonScripts.Version;

namespace Publisher
{
    /// <summary>
    /// contains all the function needed by Publisher.
    /// </summary>
    public class Publisher
    {

        private Project _project;

        private Publisher(string softwareName)
        {
            _project = Project.BySoftwareName(softwareName);
        }

        /// <summary>
        /// Create a new project to create Publisher using SoftwareName
        /// </summary>
        /// <param name="softwareName">Name if the project.</param>
        /// <returns>Publisher object</returns>
        public static Publisher New(string softwareName)
        {
            var publisher = new Publisher(softwareName);
            return publisher;
        }

        /// <summary>
        /// Use the existing project to create Publisher.
        /// </summary>
        /// <returns></returns>
        public static Publisher Load()
        {
            var configJson = ConfigJson.Load();
            var publisher = new Publisher(configJson.SoftwareName);
            return publisher;
        }

        /// <summary>
        /// Create the folder on drive if doesn't exists, and
        /// Create local config files
        /// </summary>
        public void Init()
        {
            _project.CreateOnMyDrive();

            Directory.CreateDirectory(FilePath.DotPublishDir);
            var configJson = new ConfigJson(_project.SoftwareName);
            configJson.SaveJson();

            Console.WriteLine("Project created.");
        }

        /// <summary>
        /// Increase the version in the config file
        /// </summary>
        /// <param name="versionName">Name of the version.</param>
        public void UpVersion(string versionName)
        {
            var configJson = ConfigJson.Load();
            configJson.CurrentVersionCode++;

            if (versionName == null)
                versionName = DateTime.Now.ToString("yyyymmddhhmmssfff");

            if (_project.DoesVersionNameExists(versionName))
            {
                Console.WriteLine("Version name already exists please specify a new one.");
                return;
            }
            
            configJson.VersionName = versionName;
            configJson.SaveJson();

            Console.WriteLine($"Project version increased to v{configJson.CurrentVersionCode}:{versionName}");
        }

        /// <summary>
        /// Upload the new version on the drive.
        /// </summary>
        /// <param name="force">true if want to delete existing version with same version code.</param>
        public void Push(bool force)
        {
            var configJson = ConfigJson.Load();
            var version = _project.GetVersion(configJson.CurrentVersionCode);

            //if version exists on drive
            //if force then delete else abort
            if (version != null)
            {
                if (!force)
                {
                    Console.WriteLine("Version aready exists");
                    return;
                }

                _project.DeleteVersionFromDrive(version);
            }
            
            if (configJson.CurrentVersionCode == 0)
            {
                Console.WriteLine("Run upversion first");
                return;
            }

            CreateZip();
            var name = $"{configJson.CurrentVersionCode}--{configJson.SoftwareName}--{configJson.VersionName}";
            DriveUtils.UploadFileToCloud(name + ".zip", _project.FolderId, FilePath.TempZipFile);

            Console.WriteLine($"Update {version} has been uploaded to drive.");
        }

        /// <summary>
        /// Set the version to target version code.
        /// </summary>
        /// <param name="versionCode">Target version code.</param>
        /// <param name="versionName">Version name to use.</param>
        /// <param name="force">true if want to shift to a version even if project has a version with same version code or version name </param>
        public void SetVersion(int versionCode, string versionName, bool force)
        {
            var configJson = ConfigJson.Load();

            if (_project.DoesVersionCodeExists(versionCode) || !force)
            {
                Console.WriteLine("Version code already exists please specify a new one.");
                return;
            }

            configJson.CurrentVersionCode = versionCode;

            if (_project.DoesVersionNameExists(versionName) || !force)
            {
                Console.WriteLine("Version name already exists please specify a new one.");
                return;
            }

            configJson.VersionName = versionName;
            configJson.SaveJson();

            Console.WriteLine($"Project version increased to v{versionCode}:{versionName}");
        }

        /// <summary>
        /// Upload updater to drive and return its link.
        /// </summary>
        public void GetUpdater(string os)
        {
            string fileId;
            if (DriveUtils.GetUpdaterLink(_project, os, out fileId))
            {
                Console.WriteLine(DriveUtils.IdToDirectDownloadLink(fileId));
                return;
            }


            //copy updater into temp
            var sourcePath = "";

            switch (os)
            {
                case "win":
                    sourcePath = FilePath.WinUpdaterDir;
                    break;

                case "linux":
                    sourcePath = FilePath.LinuxUpdaterDir;
                    break;

                default:
                    return;
            }

            var destinationPath = FilePath.TempDir;
            Directory.CreateDirectory(destinationPath);
            Utils.Empty(new DirectoryInfo(FilePath.TempDir));

            //Now Create all of the directories
            foreach (var dirPath in Directory.GetDirectories(sourcePath, "*",
                SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath));

            //Copy all the files & Replaces any files with the same name
            foreach (var newPath in Directory.GetFiles(sourcePath, "*.*",
                SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(sourcePath, destinationPath), true);

            //create json for updater

            var installConfigJson = new InstallConfigJson(
                FilePath.InstallConfigJsonInTemp, 
                DriveUtils.GetRootFoderIdFromMyDrive(), 
                _project.SoftwareName
                );

            installConfigJson.SaveJson();


            //zip and upload
            File.Delete(FilePath.TempZipFile);
            ZipFile.CreateFromDirectory(FilePath.TempDir, FilePath.TempZipFile);
            var updaterFileId = DriveUtils.UploadFileToCloud($"{_project.SoftwareName}--{os}.zip", _project.FolderId, FilePath.TempZipFile);

            Console.WriteLine(DriveUtils.IdToDirectDownloadLink(updaterFileId));
            
        }

        /// <summary>
        /// Automatically increase version and upload it to drive.
        /// </summary>
        /// <param name="versionName">Version name of the new version</param>
        public void PushNow(string versionName)
        {
            if (versionName == null)
                versionName = DateTime.Now.ToString("yyyyMMddHHmmssfff");

            if (_project.DoesVersionNameExists(versionName))
            {
                Console.WriteLine("Version name already exists please specify a new one.");
                return;
            }

            UpVersion(versionName);

            Push(false);
        }

        /// <summary>
        /// Delete a version with versionCode or versionName
        /// </summary>
        /// <param name="versionCode">Version to delete with versionCode</param>
        /// <param name="versionName">Version to delete with versionName</param>
        public void Drop(int versionCode, string versionName)
        {
            Version version = null;
            if (versionCode > 0)
                version = _project.GetVersion(versionCode);
            else if (versionName != null)
                version = _project.GetVersion(versionName);

            if (version == null)
            {
                Console.WriteLine("Version not found");
                return;
            }

            _project.DeleteVersionFromDrive(version);
            Console.WriteLine($"{version} deleted from drive.");
        }

        /// <summary>
        /// Shows all the versions on the drive.
        /// </summary>
        public void ShowVersions()
        {
            Console.WriteLine($"{_project.SoftwareName}\nvVersionCode:VersionName\n");

            var list = new List<Version>(_project.Versions);
            list.Reverse();

            foreach (var version in list)
                Console.WriteLine(version);
        }

        
        /// <summary>
        /// Copies the files to temp directory,
        /// and zips it
        /// </summary>
        private static void CreateZip()
        {
            var filePaths =
                new List<string>(Directory.GetFiles(FilePath.RootDir, "*", SearchOption.AllDirectories));

            for (var i = 0; i < filePaths.Count; i++)
                filePaths[i] = filePaths[i].Remove(filePaths[i].IndexOf(FilePath.RootDir), FilePath.RootDir.Length);

            filePaths.RemoveAll(elem => elem.Contains(".publish"));
            filePaths.RemoveAll(elem => elem.Contains(".pubignore"));


            //if .pubignore exists
            if (File.Exists(FilePath.IgnoreFile))
            {
                //itrate on each line
                var ignoreList = File.ReadAllLines(FilePath.IgnoreFile);

                foreach (var ignore in ignoreList)
                    //do regex on each filepath
                    //if regex is true remove the file
                    filePaths.RemoveAll(s => Regex.Match(s, ignore, RegexOptions.IgnoreCase).Success);
            }

            Utils.Empty(new DirectoryInfo(FilePath.TempDir));
            foreach (var filePath in filePaths)
            {
                Console.WriteLine($"Adding: {filePath}");
                var file = new FileInfo(FilePath.TempDir + filePath);
                file.Directory?.Create();
                File.WriteAllBytes(file.FullName, File.ReadAllBytes(filePath));
            }

            File.Delete(FilePath.TempZipFile);
            ZipFile.CreateFromDirectory(FilePath.TempDir, FilePath.TempZipFile);
        }

        
    }
}