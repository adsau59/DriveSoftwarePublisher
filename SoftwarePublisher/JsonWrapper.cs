using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary1;
using Newtonsoft.Json.Linq;

namespace SoftwarePublisher
{
    /// <summary>
    /// Json wrapper of json files used in this project
    /// </summary>
    class JsonWrapper
    {
        /// <summary>
        /// Config file containing root folder id (SoftwarePublisher) in drive
        /// </summary>
        public class ConfigJson : ClassLibrary1.IJsonWrapper.IBaseJsonWrapper
        {
            public string RootFolderId;

            public ConfigJson()
            {
            }

            public ConfigJson(string rootFolderId)
            {
                RootFolderId = rootFolderId;
            }

            public string GetPath()
            {
                return FilePath.ConfigJsonFile;
            }

            public void LoadJson()
            {
                JObject rss = ClassLibrary1.IJsonWrapper.LoadJObject(GetPath());

                if(rss == null)
                    return;

                RootFolderId = (string)rss["root_folder_id"];
            }

            public void SaveJson()
            {
                JObject rss = new JObject(
                    new JProperty("root_folder_id", RootFolderId));

                ClassLibrary1.IJsonWrapper.SaveJObject(GetPath(), rss);
            }

            public ConfigJson LoadJsonAndReturn()
            {
                LoadJson();
                return this;
            }
        }

        /// <summary>
        /// InstallConfiguration json file that is used by updater
        /// </summary>
        public class PublisherInstallConfigJson : InstallConfigJson
        {
            public PublisherInstallConfigJson()
            {
            }

            public PublisherInstallConfigJson(string folderId) : base(folderId)
            {
            }

            public override string GetPath()
            {
                return FilePath.InstallConfigJsonFile;
            }
        }

        /// <summary>
        /// Implementation of VersionJson
        /// </summary>
        public class PublisherVersionJson : VersionJson
        {
            public PublisherVersionJson()
            {
            }

            public PublisherVersionJson(string folderId, int versionCode, string versionName, string softwareName) : base(folderId, versionCode, versionName, softwareName)
            {
            }

            public override string GetPath()
            {
                return FilePath.VersionJsonFile;
            }
        }

    }
}
