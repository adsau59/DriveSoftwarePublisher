using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using CommonScripts;

namespace CommonScripts
{
    /// <summary>
    /// Project is an encapsulation of for a software that has to be versioned using DriveSP,
    /// The project file contains alot of methods that is used by both Publisher and Updater.
    /// </summary>
    public class Project
    {
        /// <summary>
        /// Name of the software.
        /// </summary>
        public string SoftwareName;

        /// <summary>
        /// FolderId of the project on Drive.
        /// </summary>
        public string FolderId;

        /// <summary>
        /// List of versions (that have been uploaded) on the drive.
        /// </summary>
        public List<Version> Versions;


        /// <summary>
        /// Create project by using just name of the software.
        /// Used only by Publisher.
        /// </summary>
        /// <param name="softwareName">Name of the software in the project.</param>
        /// <returns>Project object</returns>
        public static Project BySoftwareName(string softwareName)
        {
            var project = new Project
            {
                SoftwareName = softwareName,
                FolderId = DriveUtils.GetProjectFolderId(softwareName, DriveUtils.GetRootFoderIdFromMyDrive()),
                Versions = new List<Version>()
            };


            if (project.IsOnMyDrive())
            {
                project.Versions = DriveUtils.GetVersions(project.FolderId);
            }

            return project;
        }

        /// <summary>
        /// Create project using InstallConfiguration,
        /// used only by Updater
        /// </summary>
        /// <param name="installConfigJson">InstallConfigJson Object</param>
        /// <returns>Project object</returns>
        public static Project ByInstallConfig(InstallConfigJson installConfigJson)
        {
            var project = new Project
            {
                FolderId =
                    DriveUtils.GetProjectFolderId(installConfigJson.SoftwareName, installConfigJson.RootFolderId),
                SoftwareName = installConfigJson.SoftwareName
            };

            project.Versions = DriveUtils.GetVersions(project.FolderId);

            return project;
        }
        
        /// <summary>
        /// Adds a version to the list.
        /// </summary>
        /// <param name="version"></param>
        public void AddVersion(Version version)
        {
            this.Versions.Add(version);
        }
        
        /// <summary>
        /// Checks if the project is on drive,
        /// used only by Publisher.
        /// </summary>
        /// <returns></returns>
        public bool IsOnMyDrive()
        {
            var rootFolderId = DriveUtils.GetRootFoderIdFromMyDrive();
            return DriveUtils.FindFolderInRoot(rootFolderId, SoftwareName);
        }

        /// <summary>
        /// Find the project folder with softwareName in the rootFolder (DefineXSoftwarePublisher)
        /// Used only by Publisher.
        /// </summary>
        /// <param name="softwareName">SoftwareName to find</param>
        /// <returns>FolderId of the project folder on drive.</returns>
        public static string GetFolderIdFromSoftwarName(string softwareName)
        {
            var rootFolderId = DriveUtils.GetRootFoderIdFromMyDrive();
            DriveUtils.FindFolderInRoot(rootFolderId, softwareName, out var folderId);
            return folderId;
        }

        /// <summary>
        /// Creates the project folder on drive if it doesn't exsists,
        /// Used only by Publisher.
        /// </summary>
        public void CreateOnMyDrive()
        {
            if(!IsOnMyDrive())
                DriveUtils.CreateFolder(SoftwareName, DriveUtils.GetRootFoderIdFromMyDrive());
        }

        /// <summary>
        /// Get the latest version from the versions list.
        /// </summary>
        /// <returns>latest version.</returns>
        public Version GetLatestVersion()
        {
            if (Versions.Count <= 0)
                return null;

            var tempVersion = Versions[0];

            foreach (var version in Versions)
            {
                if (tempVersion.VersionCode < version.VersionCode)
                    tempVersion.VersionCode = version.VersionCode;
            }

            return tempVersion;
        }

        /// <summary>
        /// Get version with versionCode.
        /// </summary>
        /// <param name="versionCode">Target versionCode.</param>
        /// <returns>version with versionCode</returns>
        public Version GetVersion(int versionCode)
        {
            foreach (var version in Versions)
            {
                if (version.VersionCode == versionCode)
                    return version;
            }

            return null;
        }

        /// <summary>
        /// Get version with versionName,
        /// If duplicate exists, returns the first versionName found (usually searches in the desending order of versionCode).
        /// </summary>
        /// <param name="versionName">Target VersionName.</param>
        /// <returns>version with versionName</returns>
        public Version GetVersion(string versionName)
        {
            foreach (var version in Versions)
            {
                if (version.VersionName == versionName)
                {
                    return version;
                }
            }
            return null;
        }

        /// <summary>
        /// Delete a version from drive.
        /// </summary>
        /// <param name="version">Version to be deleted</param>
        public void DeleteVersionFromDrive(Version version)
        {
            DriveUtils.DeleteFileFromDrive(version.FileId);
            Versions.Remove(version);
        }

        /// <summary>
        /// Check if a versionName being used by a version on drive.
        /// </summary>
        /// <param name="name">VersionName to search.</param>
        /// <returns>true if a version is found with versionName.</returns>
        public bool DoesVersionNameExists(string name)
        {
            foreach (var version in Versions)
            {
                if (version.VersionName == name)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Check if a versionCode is being used by a version on drive.
        /// </summary>
        /// <param name="code">VersionCode to search.</param>
        /// <returns>true if a version is found with versionCode.</returns>
        public bool DoesVersionCodeExists(int code)
        {
            foreach (var version in Versions)
            {
                if (version.VersionCode == code)
                {
                    return true;
                }
            }

            return false;
        }

    }
}
