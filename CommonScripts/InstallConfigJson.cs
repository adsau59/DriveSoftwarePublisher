using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json.Linq;

namespace CommonScripts
{
    /// <summary>
    /// InstallConfiguration json file that is used by updater
    /// </summary>
    public class InstallConfigJson : CommonScripts.IJsonWrapper.IBaseJsonWrapper
    {
        public string RootFolderId;
        public string SoftwareName;

        private readonly string _filePath;

        public InstallConfigJson(string filePath)
        {
            _filePath = filePath;
        }

        public InstallConfigJson(string filePath, string rootFolderId, string softwareName)
        {
            RootFolderId = rootFolderId;
            SoftwareName = softwareName;
            _filePath = filePath;
        }

        public static InstallConfigJson Load(string filePath)
        {
            InstallConfigJson installConfigJson = new InstallConfigJson(filePath);
            installConfigJson.LoadJson();
            return installConfigJson;
        }

        private const string root_folder_id = "folder_id";
        private const string software_name = "software_name";

        public string GetPath()
        {
            return _filePath;
        }

        public void LoadJson()
        {
            JObject rss = CommonScripts.IJsonWrapper.LoadJObject(GetPath());

            RootFolderId = (string)rss[root_folder_id];
            SoftwareName = (string)rss[software_name];
        }

        public void SaveJson()
        {
            JObject rss = new JObject(
                new JProperty(root_folder_id, RootFolderId),
                new JProperty(software_name, SoftwareName)
                );

            CommonScripts.IJsonWrapper.SaveJObject(GetPath(), rss);
        }

        

    }
}
