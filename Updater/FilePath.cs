using System;

namespace Updater
{
    /// <summary>
    /// File Path of all the files used in the project
    /// </summary>
    public class FilePath
    {
        public static string RootDir = Environment.CurrentDirectory+"/";

        public static string ExeDir = AppDomain.CurrentDomain.BaseDirectory;
        public static string VersionJsonFile = ExeDir + "/version.json";
        public static string SecretJsonFile = ExeDir + "/client_secret.json";
        public static string InstallConfigFile = ExeDir + "/install_config.json";
        public static string ConfigFile = ExeDir + "/config.json";
        public static string TempZipFile = ExeDir + "/temp.zip";
        public static string CredentialsJsonFile = ExeDir + "/.credentials/drive-credentials.json";
        public static string UpdatedFiles = ExeDir + "/updated_files.json";
    }
}
