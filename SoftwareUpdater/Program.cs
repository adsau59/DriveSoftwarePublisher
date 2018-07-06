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
      VERSIONCODE    Version to install/update.

    ";

        static void Main(string[] args)
        {
            Service service = new Service(FilePath.SecretJsonFile, FilePath.CredentialsJsonFile);
            var arguments = new Docopt().Apply(usage, args, exit: true);
#if false
            foreach (var argument in arguments)
            {
                Console.WriteLine("{0} = {1}", argument.Key, argument.Value);
            }
#endif

            if (GetValue(arguments, "install").IsTrue)
            {
                if (GetValue(arguments, "VERSIONCODE") == null)
                    Updater.CreateInstaller(service).doUpdate(service, false);
                else if (GetValue(arguments, "VERSIONCODE").IsInt)
                    Updater.CreateInstaller(service, GetValue(arguments, "VERSIONCODE").AsInt).doUpdate(service,false);
                else
                    Console.WriteLine("Invalid arguments.");
            }
            else if (GetValue(arguments, "update").IsTrue)
            {
                if (GetValue(arguments, "VERSIONCODE") == null)
                    Updater.CreateUpdater(service).doUpdate(service, true);
                else if (GetValue(arguments, "VERSIONCODE").IsInt)
                    Updater.CreateUpdater(service, GetValue(arguments, "VERSIONCODE").AsInt).doUpdate(service,true);
                else
                    Console.WriteLine("Invalid arguments.");
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
