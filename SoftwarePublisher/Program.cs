using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary1;
using DocoptNet;

namespace SoftwarePublisher
{
    class Program
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/drive-dotnet-quickstart.json


        private const string usage = @"Software Publisher.

    Usage:
      SP.exe init NAME VERSIONNAME
      SP.exe add (FILENAME | --all)
      SP.exe remove (FILENAME | --all)
      SP.exe increase_version VERSIONNAME
      SP.exe commit
      SP.exe get_updater

    Options:
      -h --help      Show this screen.
      NAME           Name of the software.
      VERSIONNAME    Version name of the software.  

    ";

        static void Main(string[] args)
        {

            var service = new Service(FilePath.SecretJsonFile, FilePath.CredentialsJsonFile);

            var arguments = new Docopt().Apply(usage, args, exit: true);

            if (GetValue(arguments, "init").IsTrue)
            {
                string name = GetValue(arguments, "NAME").ToString();
                string version = GetValue(arguments, "VERSIONNAME").ToString();
                Publisher.CreateNewPublisher(service, name, version);
            }
            else if (GetValue(arguments, "commit").IsTrue)
            {
                Publisher.CreateUpdatePublisher().PublishUpdate(service);
            }
            else if (GetValue(arguments, "get_updater").IsTrue)
            {
                var publisher = Publisher.CreateUpdatePublisher();

                string driveId;
                if (!DriveUtils.GetUpdaterLink(service, publisher.folderId, out driveId))
                {
                    publisher.CreateUpdater(service);
                    DriveUtils.GetUpdaterLink(service, publisher.folderId, out driveId);
                }

                Console.WriteLine(DriveUtils.IdToDirectDownloadLink(driveId));

            }
            else if (GetValue(arguments, "increase_version").IsTrue)
            {
                var versionName = GetValue(arguments, "VERSIONNAME").ToString();
                Publisher.CreateUpdatePublisher().IncreaseVersion(versionName);
            }
            else if (GetValue(arguments, "add").IsTrue)
            {
                if(GetValue(arguments, "--all").IsTrue)
                    Publisher.CreateUpdatePublisher().AddAllFilesToCoreFilesJson();
                else
                {
                    Publisher.CreateUpdatePublisher()
                        .AddFileToCoreFilesJson(GetValue(arguments, "FILENAME").ToString());
                }
            }
            else if (GetValue(arguments, "remove").IsTrue)
            {
                if (GetValue(arguments, "--all").IsTrue)
                    Publisher.CreateUpdatePublisher().RemoveAllFilesFromCoreFIlesJson();
                else
                {
                    Publisher.CreateUpdatePublisher()
                        .RemoveFileFromCoreFilesJson(GetValue(arguments, "FILENAME").ToString());
                }
            }


#if DEBUG
            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
#endif

        }

        static ValueObject GetValue(IDictionary<string, ValueObject> arguments, string key)
        {
            ValueObject obj;
            arguments.TryGetValue(key, out obj);
            return obj;
        }
    }
}