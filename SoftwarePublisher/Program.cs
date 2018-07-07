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
      SP.exe upversion VERSIONNAME
      SP.exe push
      SP.exe getupdater

    Options:
      -h --help      Show this screen.
      NAME           Name of the software.
      VERSIONNAME    Version name of the software.";

        /*
        SP.exe add (FILENAME | --all)
        SP.exe remove (FILENAME | --all)
        */

        static void Main(string[] args)
        {

            var service = new Service(FilePath.SecretJsonFile, FilePath.CredentialsJsonFile);

            var arguments = new Docopt().Apply(usage, args, exit: true);

            //initialize publisher
            if (GetValue(arguments, "init").IsTrue)
            {
                string name = GetValue(arguments, "NAME").ToString();
                string version = GetValue(arguments, "VERSIONNAME").ToString();
                Publisher.CreateNewPublisher(service, name, version);
            }

            //upload files on drive
            else if (GetValue(arguments, "push").IsTrue)
            {
                Publisher.LoadFromJson().PublishUpdate(service);
            }

            //get updater
            else if (GetValue(arguments, "getupdater").IsTrue)
            {
                var publisher = Publisher.LoadFromJson();

                string driveId;
                if (!DriveUtils.GetUpdaterLink(service, publisher.folderId, out driveId))
                {
                    publisher.CreateUpdater(service);
                    DriveUtils.GetUpdaterLink(service, publisher.folderId, out driveId);
                }

                Console.WriteLine(DriveUtils.IdToDirectDownloadLink(driveId));

            }

            //increase version
            else if (GetValue(arguments, "upversion").IsTrue)
            {
                var versionName = GetValue(arguments, "VERSIONNAME").ToString();
                Publisher.LoadFromJson().IncreaseVersion(versionName);
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