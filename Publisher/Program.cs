using System;
using System.Collections.Generic;
using System.IO;
using CommonScripts;
using DocoptNet;

namespace Publisher
{
    internal class Program
    {

        private const string Usage = @"Software Publisher.

Usage:
    Publisher init SOFTWARENAME
    Publisher upversion [VERSIONNAME]
    Publisher setversion VERSIONCODE VERSIONNAME [-f | --force]
    Publisher push [-f | --force]
    Publisher pushnow [VERSIONNAME]
    Publisher getupdater (--win|--linux)
    Publisher drop (--code=<vc> | --name=<vn>)
    Publisher showversions

Options:
    -h --help      Show this screen.
    -f --force     Delete the old update if exists.
    --code=<vc>    VersionCode of the version (integer).
    --name=<vn>    VersionName of the version (string).
    SOFTWARENAME   Name of the software.
    VERSIONNAME    Version name of the software.";

        private static void Main(string[] args)
        {
            DocoptWrapper docopt = new DocoptWrapper(Usage, args);
            var service = new Service(FilePath.SecretJsonFile, FilePath.CredentialsJsonFile);
            DriveUtils.SetService(service);

            
            if (docopt.Get("init").IsTrue)
            {
                //initialize publisher
                var name = docopt.Get("SOFTWARENAME").ToString();
                Publisher.New(name).Init();
            }

            Publisher publisher = Publisher.Load();

            if (docopt.Get("push").IsTrue)
            {
                //upload files on drive
                publisher.Push(docopt.Get("--force").IsTrue);
            }
            else if (docopt.Get("getupdater").IsTrue)
            {
                //get updater
                publisher.GetUpdater(
                    docopt.Get("--win").IsTrue?"win":"linux"
                    );
            }
            else if (docopt.Get("upversion").IsTrue)
            {
                //increase version
                publisher.UpVersion(docopt.Get("VERSIONNAME").ToString());
            }
            else if (docopt.Get("pushnow").IsTrue)
            {
                //increase version and push
                publisher.PushNow(docopt.GetString("VERSIONNAME"));
            }
            else if (docopt.Get("drop").IsTrue)
            {
                //delete version
                publisher.Drop(docopt.GetInt("--code"), docopt.GetString("--name"));
            }
            else if(docopt.Get("setversion").IsTrue)
            {
                //set version to a specific code/name
                publisher.SetVersion(docopt.Get("VERSIONCODE").AsInt, docopt.GetString("VERSIONNAME"), docopt.Get("--force").IsTrue);
            }
            else if (docopt.Get("showversions").IsTrue)
            {
                //show all versions in the project
                publisher.ShowVersions();
            }

#if DEBUG
        Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
#endif
        }

        
    }
}