using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Download;
using File = System.IO.File;

namespace ClassLibrary1
{
    public class DriveUtils
    {

        public static string GetIdWithVersionCode(Service service, string folderId, int versionCode)
        {
            List<int> versionList = new List<int>();
            string pageToken = null;
            FileList result;
            do
            {
                var request = service.DriveService.Files.List();
                request.Q = "'" + folderId + "' in parents and trashed=false and name='"+versionCode+".zip'";
                request.Spaces = "drive";
                request.Fields = "nextPageToken, files(id, name)";
                request.PageToken = pageToken;
                result = request.Execute();
                foreach (var file in result.Files)
                {
                    return file.Id;
                }
                pageToken = result.NextPageToken;
            } while (pageToken != null);

            return null;
        }

        public static string GetLatestVersionCode(Service service, string folderId, out int versionCode)
        {

            List<int> versionList = new List<int>();
            string pageToken = null;
            FileList result;
            do
            {
                var request = service.DriveService.Files.List();
                request.Q = "'"+folderId+ "' in parents and trashed=false";
                request.Spaces = "drive";
                request.Fields = "nextPageToken, files(id, name)";
                request.PageToken = pageToken;
                result = request.Execute();
                foreach (var file in result.Files)
                {
                    //Console.WriteLine(String.Format(
                    //        "Found file: {0} ({1})", file.Name, file.Id));

                    var leftPart = file.Name.Split('.')[0];
                    int number;

                    if(int.TryParse(leftPart, out number))
                        versionList.Add(number);
                }
                pageToken = result.NextPageToken;
            } while (pageToken != null);
            
            if (versionList.Count == 0)
            {
                versionCode = -1;
                return "";
            }

            versionCode = versionList.Max();

            foreach (var file in result.Files)
            {
                if (file.Name == versionCode + ".zip")
                    return file.Id;
            }

            return "";
        }

        public static string GetLatestVersionCode(Service service, string folderId)
        {
            int temp;
            return GetLatestVersionCode(service, folderId, out temp);
        }

        public static string IdToDirectDownloadLink(String fileId)
        {
            return "https://drive.google.com/uc?export=download&id=" + fileId;
        }

        public static string UploadFileToCloud(Service service, string fileName, string parentFolderId, string localFilePath)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = fileName,
                Parents = new List<string>
                {
                    parentFolderId
                }
            };
            FilesResource.CreateMediaUpload request;
            using (var stream = new System.IO.FileStream(localFilePath,
                System.IO.FileMode.Open))
            {
                request = service.DriveService.Files.Create(
                    fileMetadata, stream, "application/zip");
                request.Fields = "id";
                request.Upload();
            }
            var file = request.ResponseBody;
            return file.Id;
        }

        public static string CreateFolder(Service service, string name, string folderId)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = name,
                MimeType = "application/vnd.google-apps.folder",
                Parents = new List<string>
                {
                    folderId
                }
            };
            var request = service.DriveService.Files.Create(fileMetadata);
            request.Fields = "id";
            var file = request.Execute();
            return file.Id;
        }

        public static bool GetUpdaterLink(Service service, string folderId, out string driveLink)
        {
            string pageToken = null;
            do
            {
                var request = service.DriveService.Files.List();
                request.Q = "'" + folderId + "' in parents and trashed=false and name='updater.zip'";
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

        public static bool FindFolderInRoot(Service service, string rootFolderId, string name)
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
                    return true;
                }
                pageToken = result.NextPageToken;
            } while (pageToken != null);

            return false;

        }

        public static bool DownloadFile(Service service, string filePath, string fileId)
        {
            var request = service.DriveService.Files.Get(fileId);
            var stream = new System.IO.MemoryStream();

            // Add a handler which will be notified on progress changes.
            // It will notify on each chunk download and when the
            // download is completed or failed.

            bool downloadFailed = false;
            request.MediaDownloader.ProgressChanged +=
                (IDownloadProgress progress) =>
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

            File.Delete(filePath);
            using (FileStream file = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                stream.WriteTo(file);
            }

            return true;
        }

        public static string GetRelativePath(string filespec)
        {
            var folder = Environment.CurrentDirectory;

            Uri pathUri = new Uri(filespec);
            // Folders must end in a slash
            if (!folder.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                folder += Path.DirectorySeparatorChar;
            }
            Uri folderUri = new Uri(folder);
            return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', '\\'));
        }



    }
}
