using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ClassLibrary1
{
    /// <summary>
    /// VersionJson abstract class used in Publisher and Updater
    /// </summary>
    public abstract class VersionJson : IJsonWrapper.IBaseJsonWrapper
    {
        private string folderId;
        private int versionCode;
        private string versionName;
        private string softwareName;

        public VersionJson()
        {
        }

        public VersionJson(string folderId, int versionCode, string versionName, string softwareName)
        {
            this.folderId = folderId;
            this.versionCode = versionCode;
            this.versionName = versionName;
            this.softwareName = softwareName;
        }

        public VersionJson(VersionDetailStruct versionDetail)
        {
            this.folderId = versionDetail.folderId;
            this.versionCode = versionDetail.versionCode;
            this.versionName = versionDetail.versionName;
            this.softwareName = versionDetail.softwareName;
        }

        public string FolderId
        {
            get => folderId;
            set => folderId = value;
        }

        public int VersionCode
        {
            get => versionCode;
            set => versionCode = value;
        }

        public string VersionName
        {
            get => versionName;
            set => versionName = value;
        }

        public string SoftwareName
        {
            get => softwareName;
            set => softwareName = value;
        }


        public abstract string GetPath();

        public void LoadJson()
        {
            JObject rss = IJsonWrapper.LoadJObject(GetPath());

            if (rss == null)
            {
                Console.WriteLine("version.json was not found, please initialize the publisher first or contact the developer.");
                return;
            }

            folderId = (string)rss["folder_id"];
            versionCode = (int)rss["version_code"];
            versionName = (string)rss["version_name"];
            softwareName = (string)rss["software_name"];
        }

        public void SaveJson()
        {
            JObject rss = new JObject(
                new JProperty("folder_id", folderId),
                new JProperty("version_code", versionCode),
                new JProperty("version_name", versionName),
                new JProperty("software_name", softwareName));

            IJsonWrapper.SaveJObject(GetPath(), rss);
        }

    }

    /// <summary>
    /// VersionDetail Struct can be used to create VersionJson
    /// </summary>
    public struct VersionDetailStruct
    {
        public string folderId;
        public int versionCode;
        public string versionName;
        public string softwareName;


        public VersionDetailStruct(string folderId, int versionCode, string versionName, string softwareName)
        {
            this.folderId = folderId;
            this.versionCode = versionCode;
            this.versionName = versionName;
            this.softwareName = softwareName;
        }
    }
}
