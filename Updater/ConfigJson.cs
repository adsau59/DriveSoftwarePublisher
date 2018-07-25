using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using SoftwareUpdater;

namespace Updater
{
    /// <inheritdoc />
    /// <summary>
    /// Json file that contains the configuration needed by updater.
    /// </summary>
    internal class ConfigJson : CommonScripts.IJsonWrapper.IBaseJsonWrapper
    {
        public int CurrentVersionCode;

        public ConfigJson(string softwareName, int currentVersionCode)
        {
            CurrentVersionCode = currentVersionCode;
        }

        private ConfigJson()
        {
        }

        public static ConfigJson Load()
        {
            var configJson = new ConfigJson();
            configJson.LoadJson();
            return configJson;
        }

        private string current_version = "current_version";

        public string GetPath()
        {
            return FilePath.ConfigFile;
        }

        public void LoadJson()
        {
            JObject rss = CommonScripts.IJsonWrapper.LoadJObject(GetPath());

            CurrentVersionCode = (int)rss[current_version];
        }

        public void SaveJson()
        {
            var rss = new JObject(
                new JProperty(current_version, CurrentVersionCode)
            );

            CommonScripts.IJsonWrapper.SaveJObject(GetPath(), rss);
        }
    }
}
