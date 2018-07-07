using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwarePublisher
{
    /// <summary>
    /// File Path of all the files used in the project
    /// </summary>
    public class FilePath
    {

        public static string RootDir = Environment.CurrentDirectory+"\\";

        public static string IgnoreFile = RootDir + ".pubignore";

        public static string ConfigDir = RootDir+ ".publisher\\";

        public static string VersionJsonFile = ConfigDir + "version.json";
        public static string TempDir = ConfigDir + "temp\\";
        public static string TempZipFile = ConfigDir + "temp.zip";

        public static string UpdaterDir = ConfigDir + "updater\\.updater\\";
        public static string UpdaterExeDir = UpdaterDir + "updater\\";
        public static string InstallConfigJsonFile = UpdaterExeDir + "install_config.json";

        public static string ExeRootDir = AppDomain.CurrentDomain.BaseDirectory;
        public static string ConfigJsonFile = ExeRootDir + "config.json";
        public static string SecretJsonFile = ExeRootDir + "client_secret.json";
        public static string BaseUpdaterDir = ExeRootDir + "updater\\";
        public static string CredentialsJsonFile = ExeRootDir + "\\.credentials/drive-credentials.json";
    }
}
