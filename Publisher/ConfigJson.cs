using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using Publisher;

namespace Publisher
{
    /// <inheritdoc />
    /// <summary>
    /// Json file that contains the configuration needed by Publisher.
    /// </summary>
    internal class ConfigJson : CommonScripts.IJsonWrapper.IBaseJsonWrapper
    {
        public string SoftwareName;
        public int CurrentVersionCode;
        public string VersionName;

        private const string software_name = "software_name";
        private const string current_version = "current_version";
        private const string version_name = "version_name";

        public ConfigJson(string softwareName)
        {
            SoftwareName = softwareName;
            CurrentVersionCode = 0;
            VersionName = "none";
        }

        private ConfigJson()
        {

        }

        public static ConfigJson Load()
        {
            var configjson = new ConfigJson();
            configjson.LoadJson();
            return configjson;
        }

        public string GetPath()
        {
            return FilePath.ConfigJsonFile;
        }

        public void LoadJson()
        {
            JObject rss = CommonScripts.IJsonWrapper.LoadJObject(GetPath());
            
            SoftwareName = (string)rss[software_name];
            CurrentVersionCode = (int) rss[current_version];
            VersionName = (string) rss[version_name];
        }

        public void SaveJson()
        {
            JObject rss = new JObject(
                new JProperty(software_name, SoftwareName),
                new JProperty(current_version, CurrentVersionCode),
                new JProperty(version_name, VersionName)
                );

            CommonScripts.IJsonWrapper.SaveJObject(GetPath(), rss);
        }
    }
}
