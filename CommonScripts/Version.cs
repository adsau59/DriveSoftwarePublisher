using System;
using System.Collections.Generic;
using System.Text;

namespace CommonScripts
{
    /// <summary>
    /// Encapsulates the information of versions (update files) from drive
    /// </summary>
    public class Version
    {
        /// <summary>
        /// Integer defining the version number, of an update file,
        /// VersionCode starts with 1 and increments with each new updated
        /// </summary>
        public int VersionCode;

        /// <summary>
        /// String naming the update with a name,
        /// Could be any string without "--" substring or space, and 
        /// Should be unique for each version.
        /// </summary>
        public string VersionName;

        /// <summary>
        /// FileId of the update file of the version in the Drive.
        /// </summary>
        public string FileId;

        public Version(int versionCode, string versionName, string fileId)
        {
            this.VersionCode = versionCode;
            this.VersionName = versionName;
            this.FileId = fileId;
        }

        public override string ToString()
        {
            return $"v{VersionCode}:{VersionName}";
        }
    }
}
