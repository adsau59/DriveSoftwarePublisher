using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Publisher
{
    /// <summary>
    /// File Path of all the files used in the project
    /// </summary>
    public class FilePath
    {

        public static string RootDir = Environment.CurrentDirectory+"/";

        public static string IgnoreFile = RootDir + ".pubignore";

        public static string DotPublishDir = RootDir+ ".publisher/";

        public static string ExeRootDir = AppDomain.CurrentDomain.BaseDirectory;
        public static string SecretJsonFile = ExeRootDir + "client_secret.json";
        public static string BaseUpdaterDir = ExeRootDir + "updater/";
        public static string WinUpdaterDir = BaseUpdaterDir + "win/";
        public static string LinuxUpdaterDir = BaseUpdaterDir + "linux/";
        public static string CredentialsJsonFile = ExeRootDir + ".credentials/drive-credentials.json";

        public static string ConfigJsonFile = DotPublishDir + "config.json";
        public static string TempDir = DotPublishDir + "temp/";
        public static string TempZipFile = DotPublishDir + "temp.zip";
        public static string InstallConfigJsonInTemp = TempDir + ".updater/install_config.json";
    }
}
