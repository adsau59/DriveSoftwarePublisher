using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using File = Google.Apis.Drive.v3.Data.File;

namespace ClassLibrary1
{
    /// <summary>
    /// Contains utils functions that communicate with Drive api
    /// </summary>
    public class DriveUtils
    {
        /// <summary>
        /// use version code to get drive file id
        /// </summary>
        /// <param name="service">Service object</param>
        /// <param name="folderId">Parent folder id</param>
        /// <param name="versionCode">Version code to find</param>
        /// <param name="versionDetail">gets version details that can be used to create json</param>
        /// <returns>drive file id of the update zip</returns>
        public static string GetFileIdWithVersionCode(Service service, string folderId, int versionCode,
            out VersionDetailStruct versionDetail)
        {
            var versionList = new List<int>();
            string pageToken = null;
            FileList result;
            do
            {
                var request = service.DriveService.Files.List();
                request.Q = $"\'{folderId}\' in parents and trashed=false and name contains \'{versionCode}--\'";
                request.Spaces = "drive";
                request.Fields = "nextPageToken, files(id, name)";
                request.PageToken = pageToken;
                result = request.Execute();

                if (result.Files.Count > 0)
                {
                    var nameParts = Utils.GetVersionDetailFromName(result.Files[0].Name);
                    versionDetail = new VersionDetailStruct(folderId, int.Parse(nameParts[0]), nameParts[2], nameParts[1]);
                    return result.Files[0].Id;
                }

                pageToken = result.NextPageToken;
            } while (pageToken != null);

            versionDetail = new VersionDetailStruct();
            return null;
        }

        /// <summary>
        /// Get VersionJson object of the latest version
        /// </summary>
        /// <param name="service">driver service</param>
        /// <param name="folderId">folderid to search in</param>
        /// <returns>VersionDeatilStruct of the latest zip</returns>
        public static VersionDetailStruct GetLatestVersion(Service service, string folderId)
        {
            var versionList = new List<int>();
            string pageToken = null;
            File latestFile = null;
            FileList result;
            do
            {
                var request = service.DriveService.Files.List();
                request.Q = $"\'{folderId}\' in parents and trashed=false";
                request.Spaces = "drive";
                request.Fields = "nextPageToken, files(id, name)";
                request.PageToken = pageToken;
                result = request.Execute();


                var largestVersion = -1;
                foreach (var file in result.Files)
                {
                    if(file.Name == "updater.zip")
                        continue;

                    var version = Utils.GetVersionCodeFromName(file.Name);
                    if (largestVersion < version)
                    {
                        largestVersion = version;
                        latestFile = file;
                    }
                }


                pageToken = result.NextPageToken;
            } while (pageToken != null);

            if (latestFile == null)
                return new VersionDetailStruct();


            //versioncode--softwarename--versionname.zip
            var nameParts = Utils.GetVersionDetailFromName(latestFile.Name);
            return new VersionDetailStruct(folderId, int.Parse(nameParts[0]), nameParts[2], nameParts[1]);
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
        public static string UploadFileToCloud(Service service, string fileName, string parentFolderId,
            string localFilePath)
        {
            var fileMetadata = new File
            {
                Name = fileName,
                Parents = new List<string>
                {
                    parentFolderId
                }
            };
            FilesResource.CreateMediaUpload request;
            using (var stream = new FileStream(localFilePath,
                FileMode.Open))
            {
                request = service.DriveService.Files.Create(
                    fileMetadata, stream, "application/zip");
                request.Fields = "id";
                request.Upload();
            }

            var file = request.ResponseBody;
            return file.Id;
        }

        /// <summary>
        /// Create a folder in a drive
        /// </summary>
        /// <param name="service">driver service</param>
        /// <param name="name">name of the folder to be created</param>
        /// <param name="parentFolderId">drive folder id of the parent</param>
        /// <returns>drive folder id of the folder created</returns>
        public static string CreateFolder(Service service, string name, string parentFolderId)
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
        /// <param name="service">driver service</param>
        /// <param name="folderId">Drive Folder id of the parent</param>
        /// <param name="driveLink">drive file id of the updater.zip file</param>
        /// <returns>true if updater.zip is found</returns>
        public static bool GetUpdaterLink(Service service, string folderId, out string driveLink)
        {
            string pageToken = null;
            do
            {
                var request = service.DriveService.Files.List();
                request.Q = $"\'{folderId}\' in parents and trashed=false and name=\'updater.zip\'";
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
            driveLink = "";
            return false;
        }

        /// <summary>
        /// Find folder in the SoftwarePublisher folder in drive,
        /// used to find existing project project
        /// </summary>
        /// <param name="service">driver service</param>
        /// <param name="rootFolderId">SoftwarePublisher folder id</param>
        /// <param name="name">name of the folder</param>
        /// <param name="folderid">folder id of the found folder</param>
        /// <returns>true if folder is found</returns>
        public static bool FindFolderInRoot(Service service, string rootFolderId, string name, out string folderid)
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

            folderid = "";
            return false;
        }

        /// <summary>
        /// overload of above function
        /// </summary>
        public static bool FindFolderInRoot(Service service, string rootFolderId, string name)
        {
            return FindFolderInRoot(service, rootFolderId,name, out _);
        }


        /// <summary>
        /// Downloads the file from the drive
        /// </summary>
        /// <param name="service">driver service</param>
        /// <param name="filePath">path of the file where it is to be saved</param>
        /// <param name="fileId">FileId of the file to be downloaded</param>
        /// <returns></returns>
        public static bool DownloadFile(Service service, string filePath, string fileId)
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
                            Console.WriteLine(progress.BytesDownloaded);
                            break;
                        }
                        case DownloadStatus.Completed:
                        {
                            Console.WriteLine("Download complete.");
                            break;
                        }
                        case DownloadStatus.Failed:
                        {
                            Console.WriteLine("Download failed.");
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


    }
}