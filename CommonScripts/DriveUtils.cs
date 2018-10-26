using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CommonScripts;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Upload;
using File = Google.Apis.Drive.v3.Data.File;
using Version = CommonScripts.Version;

namespace CommonScripts
{
    /// <summary>
    /// Contains utils functions that communicate with Drive api
    /// </summary>
    public class DriveUtils
    {

        private static Service service;
        private const string rootFolderName = "DefineXSoftwarePublisher";

        /// <summary>
        /// Must be executed before using any method from this classs.
        /// </summary>
        /// <param name="service">Service object</param>
        public static void SetService(Service service)
        {
            DriveUtils.service = service;
        }


        private static string _rootFolderId;
        /// <summary>
        /// Searches for "DefineXSoftwarePublisher" on drive,
        /// caches its folderId, and returns it.
        /// </summary>
        /// <returns>FolderId of "DefineXSoftwarePublisher" on drive</returns>
        public static string GetRootFoderIdFromMyDrive()
        {
            if (_rootFolderId == null)
            {
                DriveUtils.FindFolderInRoot("root", rootFolderName, out _rootFolderId);

                if(_rootFolderId == "")
                    _rootFolderId = CreateFolder(rootFolderName, "root");
                
            }
            return _rootFolderId;
        }

        private static string _projectFolderId;
        /// <summary>
        /// Searches for project folder on drive,
        /// caches its folderId, and returns it
        /// </summary>
        /// <param name="softwareName">Name of the software in the project.</param>
        /// <param name="rootFolderId">Folderid of "DefineXSoftwarePublisher" on drive</param>
        /// <returns>Folder id of the projecct folder on drive.</returns>
        public static string GetProjectFolderId(string softwareName, string rootFolderId)
        {
            if (_projectFolderId == null)
                DriveUtils.FindFolderInRoot(rootFolderId, softwareName, out _projectFolderId);

            return _projectFolderId;
        }


        /// <summary>
        /// Gets list of version of a project with folderId.
        /// </summary>
        /// <param name="folderid">folderId of the project.</param>
        /// <returns>List of versions in the project on drive.</returns>
        public static List<Version> GetVersions(string folderid)
        {
            var versions = new List<Version>();

            var versionList = new List<int>();
            string pageToken = null;
            File latestFile = null;
            FileList result;
            do
            {
                var request = service.DriveService.Files.List();
                request.Q = $"\'{folderid}\' in parents and trashed=false";
                request.Spaces = "drive";
                request.Fields = "nextPageToken, files(id, name)";
                request.PageToken = pageToken;
                result = request.Execute();


                var largestVersion = -1;

                var updaterRegex = new Regex(@"^[a-zA-Z0-9_.]*--(win|linux|mac)*\.zip$");
                var versionRegex = new Regex(@"^([a-zA-Z0-9_.]*)--([a-zA-Z0-9_.]*)--([a-zA-Z0-9_.]*)\.zip$");

                foreach (var file in result.Files)
                {
                    if (updaterRegex.IsMatch(file.Name))
                        continue;

                    var versionMatch = versionRegex.Match(file.Name);

                    if(versionMatch.Success)
                        versions.Add(new Version(int.Parse(versionMatch.Groups[1].Value), versionMatch.Groups[3].Value, file.Id));
                }


                pageToken = result.NextPageToken;
            } while (pageToken != null);

            return versions;
        }

        /// <summary>
        /// Returns the url to download the zip with fileid
        /// </summary>
        /// <param name="fileId">drive file id</param>
        /// <returns>direct download url</returns>
        public static string IdToDirectDownloadLink(string fileId)
        {
            return "https://drive.google.com/uc?export=download&id=" + fileId;
        }

        /// <summary>
        /// Uploads the file to the cloud
        /// </summary>
        /// <param name="service">driver service.</param>
        /// <param name="fileName">Name for the file to be saved as</param>
        /// <param name="parentFolderId">Drive folder id of the parent of the file to be saved</param>
        /// <param name="localFilePath">File path in the local drive to be saved</param>
        /// <returns>id of the file that is saved</returns>
        public static string UploadFileToCloud(string fileName, string parentFolderId, string localFilePath)
        {
            var uploadStream = new System.IO.FileStream(localFilePath,
                                                System.IO.FileMode.Open,
                                                System.IO.FileAccess.Read);

            sizeOfFile = new System.IO.FileInfo(localFilePath).Length;

            var fileMetadata = new File
            {
                Name = fileName,
                Parents = new List<string>
                {
                    parentFolderId
                }
            };

            // Get the media upload request object.
            var insertRequest = service.DriveService.Files.Create(
                fileMetadata,
                uploadStream,
                "application/zip");

            // Add handlers which will be notified on progress changes and upload completion.
            // Notification of progress changed will be invoked when the upload was started,
            // on each upload chunk, and on success or failure.
            insertRequest.ProgressChanged += Upload_ProgressChanged;
            insertRequest.ResponseReceived += Upload_ResponseReceived;

            var task = insertRequest.UploadAsync();
            task.ContinueWith(t =>
            {
                // Remeber to clean the stream.
                uploadStream.Dispose();
            });

            task.Wait();

            var file = insertRequest.ResponseBody;
            return file.Id;

        }

        static ProgressBar progressBar = null;
        static long sizeOfFile;

        static void Upload_ProgressChanged(IUploadProgress progress)
        {
            if(progress.Status == UploadStatus.Starting)
            {
                progressBar = new ProgressBar();
            }
            progressBar.Report((double) progress.BytesSent / sizeOfFile);
        }

        static void Upload_ResponseReceived(File file)
        {
            progressBar.Dispose();
        }

        /// <summary>
        /// Create a folder in a drive
        /// </summary>
        /// <param name="name">name of the folder to be created</param>
        /// <param name="parentFolderId">drive folder id of the parent</param>
        /// <returns>drive folder id of the folder created</returns>
        public static string CreateFolder(string name, string parentFolderId)
        {
            var fileMetadata = new File
            {
                Name = name,
                MimeType = "application/vnd.google-apps.folder",
                Parents = new List<string>
                {
                    parentFolderId
                }
            };
            var request = service.DriveService.Files.Create(fileMetadata);
            request.Fields = "id";
            var file = request.Execute();
            return file.Id;
        }

        /// <summary>
        /// Searches for updater.zip file in a folder
        /// </summary>
        /// <param name="folderId">Drive Folder id of the parent</param>
        /// <param name="driveLink">drive file id of the updater.zip file</param>
        /// <returns>true if updater.zip is found</returns>
        public static bool GetUpdaterLink(Project project, string os, out string driveLink)
        {
            string pageToken = null;
            do
            {
                var request = service.DriveService.Files.List();
                request.Q = $"\'{project.FolderId}\' in parents and trashed=false and name=\'{project.SoftwareName}--{os}.zip\'";
                request.Spaces = "drive";
                request.Fields = "nextPageToken, files(id, name)";
                request.PageToken = pageToken;
                var result = request.Execute();
                foreach (var file in result.Files)
                {
                    driveLink = file.Id;
                    return true;
                }

                pageToken = result.NextPageToken;
            } while (pageToken != null);

            //if cant find then
            driveLink = null;
            return false;
        }

        /// <summary>
        /// Find folder in the SoftwarePublisher folder in drive,
        /// used to find existing project project
        /// </summary>
        /// <param name="rootFolderId">SoftwarePublisher folder id</param>
        /// <param name="name">name of the folder</param>
        /// <param name="folderid">folder id of the found folder</param>
        /// <returns>true if folder is found</returns>
        public static bool FindFolderInRoot(string rootFolderId, string name, out string folderid)
        {
            string pageToken = null;
            do
            {
                var request = service.DriveService.Files.List();
                request.Q = $"\'{rootFolderId}\' in parents and trashed=false and name=\'{name}\'";
                request.Spaces = "drive";
                request.Fields = "nextPageToken, files(id, name)";
                request.PageToken = pageToken;
                var result = request.Execute();
                if (result.Files.Any())
                {
                    folderid = result.Files[0].Id;
                    return true;
                }
                pageToken = result.NextPageToken;
            } while (pageToken != null);

            folderid = null;
            return false;
        }

        /// <summary>
        /// overload of above function
        /// </summary>
        public static bool FindFolderInRoot(string rootFolderId, string name)
        {
            return FindFolderInRoot(rootFolderId,name, out _);
        }


        /// <summary>
        /// Downloads the file from the drive
        /// </summary>
        /// <param name="filePath">path of the file where it is to be saved</param>
        /// <param name="fileId">FileId of the file to be downloaded</param>
        /// <returns></returns>
        public static bool DownloadFile(string filePath, string fileId)
        {
            var request = service.DriveService.Files.Get(fileId);
            var stream = new MemoryStream();

            // Add a handler which will be notified on progress changes.
            // It will notify on each chunk download and when the
            // download is completed or failed.

            var downloadFailed = false;
            request.MediaDownloader.ProgressChanged +=
                progress =>
                {
                    switch (progress.Status)
                    {
                        case DownloadStatus.Downloading:
                        {
                            //Console.WriteLine(progress.BytesDownloaded);
                            break;
                        }
                        case DownloadStatus.Completed:
                        {
                            //Console.WriteLine("Download complete.");
                            break;
                        }
                        case DownloadStatus.Failed:
                        {
                            //Console.WriteLine("Download failed.");
                            downloadFailed = true;
                            break;
                        }
                    }
                };

            if (downloadFailed)
                return false;

            request.Download(stream);

            System.IO.File.Delete(filePath);
            using (var file = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                stream.WriteTo(file);
            }

            return true;
        }

        public static bool DeleteFileFromDrive(string fileId)
        {
            try
            {
                service.DriveService.Files.Delete(fileId).Execute();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }


    }
}