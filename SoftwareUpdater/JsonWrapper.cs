using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary1;
using Newtonsoft.Json.Linq;

namespace SoftwareUpdater
{
    class JsonWrapper
    {

        public class InstallConfigJson: IJsonWrapper.IBaseJsonWrapper, IJsonWrapper.IExtraMethodForJsonWrapper<InstallConfigJson>
        {
            public string _folderId;

            public InstallConfigJson()
            {
            }

            public InstallConfigJson(string folderId)
            {
                _folderId = folderId;
            }

            public string GetPath()
            {
                return FilePath.InstallConfigFile;
            }

            public void LoadJson()
            {
                JObject rss = IJsonWrapper.LoadJObject(GetPath());

                _folderId = (string) rss["folder_id"];
            }

            public void SaveJson()
            {
                JObject rss = new JObject(
                    new JProperty("folder_id", _folderId));

                IJsonWrapper.SaveJObject(GetPath(), rss);
            }

            public InstallConfigJson LoadJsonAndReturn()
            {
                LoadJson();
                return this;
            }

            
        }

        public class VersionJson : IJsonWrapper.IBaseJsonWrapper, IJsonWrapper.IExtraMethodForJsonWrapper<VersionJson>
        {
            public string folderId;
            public int versionCode;
            public string versionName;
            public string softwareName;

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

            public string GetPath()
            {
                return FilePath.VersionJsonFile;
            }

            public void LoadJson()
            {
                JObject rss = IJsonWrapper.LoadJObject(GetPath());

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

            public VersionJson LoadJsonAndReturn()
            {
                LoadJson();
                return this;
            }

        }

        public class CoreFilesJson : IJsonWrapper.IBaseJsonWrapper, IJsonWrapper.IExtraMethodForJsonWrapper<CoreFilesJson>
        {
            public List<string> FileList;

            public CoreFilesJson()
            {
            }

            public CoreFilesJson(List<string> fileList)
            {
                FileList = fileList;
            }

            public string GetPath()
            {
                return FilePath.CoreFilesJsonFile;
            }

            public void LoadJson()
            {
                JObject rss = IJsonWrapper.LoadJObject(GetPath());
                JArray jArray = (JArray)rss["core_files"];
                FileList = jArray.Select(c => (string)c).ToList();
            }

            public void SaveJson()
            {
                JObject rss = new JObject(
                    new JProperty("core_files", new JArray(
                        from f in FileList select new JValue(f)
                    )));

                IJsonWrapper.SaveJObject(GetPath(), rss);
            }

            public CoreFilesJson LoadJsonAndReturn()
            {
                LoadJson();
                return this;
            }
        }

    }
}
