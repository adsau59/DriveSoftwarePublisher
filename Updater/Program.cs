using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonScripts;
using DocoptNet;
using Updater;

namespace SoftwareUpdater
{
    internal class Program
    {


        private const string Usage = @"Software Updater.

Usage:
    Updater install [--code=<vc> | --name=<vn>]
    Updater checkupdate
    Updater showversions

Options:
    -h --help      Show this screen.
    VERSIONCODE    Version to install/update.
    --code=<vc>    VersionCode of the version (integer).
    --name=<vn>    VersionName of the version (string).";


        private static void Main(string[] args)
        {
            var docopt = new DocoptWrapper(Usage, args);
            var service = new Service(FilePath.SecretJsonFile, FilePath.CredentialsJsonFile);
            DriveUtils.SetService(service);

            var updater = new Updater.Updater();

            if (docopt.Get("install").IsTrue)
            {
                updater.Install(docopt.GetInt("--code"), docopt.GetString("--name"));
            }
            else if (docopt.Get("checkupdate").IsTrue)
            {
                updater.Checkupdate();
            }
            else if (docopt.Get("showversions").IsTrue)
            {
                //show all versions in the project
                updater.ShowVersions();
            }



#if DEBUG
            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
#endif
        }

    }
}
