using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary1;
using DocoptNet;

namespace SoftwareUpdater
{

    class Program
    {


        private const string usage = @"Software Updater.

    Usage:
      SU.exe install [VERSIONCODE]
      SU.exe update [VERSIONCODE]

    Options:
      -h --help      Show this screen.
      VERSIONCODE    Version to install/update.";

        static void Main(string[] args)
        {
            var arguments = new Docopt().Apply(usage, args, exit: true);
            Service service = new Service(FilePath.SecretJsonFile, FilePath.CredentialsJsonFile);


            //install command
            if (GetValue(arguments, "install").IsTrue)
            {
                //version is not specified, use latest version
                if (GetValue(arguments, "VERSIONCODE") == null)
                    Updater.CreateInstaller(service).doUpdate(service);
                //version is specified, use specified version
                else if (GetValue(arguments, "VERSIONCODE").IsInt)
                    Updater.CreateInstaller(service, GetValue(arguments, "VERSIONCODE").AsInt).doUpdate(service);
                else
                    Console.WriteLine("Invalid arguments.");
            }

            //update command
            else if (GetValue(arguments, "update").IsTrue)
            {
                //version is not specified, download latest version
                if (GetValue(arguments, "VERSIONCODE") == null)
                    Updater.CreateUpdater(service).doUpdate(service);
                //version is speified, use specified version
                else if (GetValue(arguments, "VERSIONCODE").IsInt)
                    Updater.CreateUpdater(service, GetValue(arguments, "VERSIONCODE").AsInt).doUpdate(service);
                else
                    Console.WriteLine("Invalid arguments.");
            }


#if DEBUG
            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
#endif
        }

        /// <summary>
        /// Helps keep the code clean for using docopt
        /// </summary>
        /// <param name="arguments">docopt idictionary obeject</param>
        /// <param name="key">key to search</param>
        /// <returns></returns>
        static ValueObject GetValue(IDictionary<string, ValueObject> arguments, string key)
        {
            ValueObject obj;
            arguments.TryGetValue(key, out obj);
            return obj;
        }
    }
}
