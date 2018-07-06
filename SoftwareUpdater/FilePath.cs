using System;

namespace SoftwareUpdater
{
    public class FilePath
    {
        public static string RootDir = Environment.CurrentDirectory+"\\";
        public static string VersionJsonFile = RootDir + "\\version.json";
        public static string CoreFilesJsonFile = RootDir + "\\core_file.json";

        public static string ExeDir = AppDomain.CurrentDomain.BaseDirectory;
        public static string SecretJsonFile = ExeDir + "\\client_secret.json";
        public static string InstallConfigFile = ExeDir + "\\install_config.json";
        public static string TempZipFile = ExeDir + "\\temp.zip";
        public static string CredentialsJsonFile = ExeDir + "\\.credentials/drive-credentials.json";
    }
}
