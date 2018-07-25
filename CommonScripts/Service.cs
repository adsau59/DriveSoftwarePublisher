using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommonScripts
{
    /// <summary>
    /// Encapsulation of DriveService Object,
    /// Used in DriveUtils to perform api calls.
    /// </summary>
    public class Service
    {
        private DriveService driveService;

        public DriveService DriveService
        {
            get { return driveService; }
        }

        private static string ApplicationName = "SoftwarePublisher";
        private static string[] Scopes = new string[] { DriveService.Scope.Drive,
                           DriveService.Scope.DriveAppdata,
                           DriveService.Scope.DriveFile,
                           DriveService.Scope.DriveMetadataReadonly,
                           DriveService.Scope.DriveReadonly,
                           DriveService.Scope.DriveScripts };

        /// <summary>
        /// Loads credentials and permisions if exists
        /// if not then create new
        /// </summary>
        /// <param name="secretPath">Path to secrete file</param>
        /// <param name="credentialsJson">Path to credentialsJson file</param>
        public Service(string secretPath, string credentialsJson)
        {
            UserCredential credential;

            using (var stream =
                new FileStream(secretPath, FileMode.Open, FileAccess.Read))
            {
                string credPath = "";
                credPath = Path.Combine(credPath, credentialsJson);

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            // Create Drive API service.
            driveService = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }


    }
}
