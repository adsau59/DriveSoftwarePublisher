using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwarePublisher
{
    public class FilePath
    {

        public static string RootDir = Environment.CurrentDirectory+"\\";
        public static string CoreFilesJsonFile = RootDir + "core_file.json";
        public static string VersionJsonFile = RootDir + "version.json";

        public static string ConfigDir = RootDir+ ".publisher\\";
        public static string TempDir = ConfigDir + "temp\\";
        public static string TempZipFile = ConfigDir + "temp.zip";

        public static string UpdaterDir = ConfigDir + "updater\\.updater\\";
        public static string InstallConfigJsonFile = UpdaterDir + "install_config.json";

        public static string ExeRootDir = AppDomain.CurrentDomain.BaseDirectory;
        public static string ConfigJsonFile = ExeRootDir + "config.json";
        public static string SecretJsonFile = ExeRootDir + "client_secret.json";
        public static string BaseUpdaterDir = ExeRootDir + "updater\\";
        public static string CredentialsJsonFile = ExeRootDir + "\\.credentials/drive-credentials.json";
    }
}
