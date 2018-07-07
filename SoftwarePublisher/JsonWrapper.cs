using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary1;
using Newtonsoft.Json.Linq;

namespace SoftwarePublisher
{
    class JsonWrapper
    {

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

                RootFolderId = (string)rss["folder_id"];
            }

            public void SaveJson()
            {
                JObject rss = new JObject(
                    new JProperty("folder_id", RootFolderId));

                ClassLibrary1.IJsonWrapper.SaveJObject(GetPath(), rss);
            }

            public ConfigJson LoadJsonAndReturn()
            {
                LoadJson();
                return this;
            }
        }

        public class InstallConfigJson : ClassLibrary1.IJsonWrapper.IBaseJsonWrapper
        {
            public string FolderId;

            public InstallConfigJson()
            {
            }

            public InstallConfigJson(string folderId)
            {
                FolderId = folderId;
            }

            public string GetPath()
            {
                return FilePath.InstallConfigJsonFile;
            }

            public void LoadJson()
            {
                JObject rss = ClassLibrary1.IJsonWrapper.LoadJObject(GetPath());

                FolderId = (string)rss["root_foder_id"];
            }

            public void SaveJson()
            {
                JObject rss = new JObject(
                    new JProperty("root_folder_id", FolderId));

                ClassLibrary1.IJsonWrapper.SaveJObject(GetPath(), rss);
            }

            public InstallConfigJson LoadJsonAndReturn()
            {
                LoadJson();
                return this;
            }

        }

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
