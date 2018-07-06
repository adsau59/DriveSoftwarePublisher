using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary1;
using Newtonsoft.Json.Linq;

namespace SoftwarePublisher
{
    class JsonWrappers
    {

        public class ConfigJson : ClassLibrary1.IJsonWrapper.IBaseJsonWrapper, ClassLibrary1.IJsonWrapper.IExtraMethodForJsonWrapper<ConfigJson>
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

                RootFolderId = (string)rss["root_foder_id"];
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

        public class CoreFilesJson : ClassLibrary1.IJsonWrapper.IBaseJsonWrapper, ClassLibrary1.IJsonWrapper.IExtraMethodForJsonWrapper<CoreFilesJson>
        {
            public List<string> FileList = new List<string>() { DriveUtils.GetRelativePath(FilePath.VersionJsonFile) , DriveUtils.GetRelativePath(FilePath.CoreFilesJsonFile) };

            public CoreFilesJson()
            {
            }

            public CoreFilesJson(List<string> fileList)
            {
                FileList.AddRange(fileList);
            }

            public string GetPath()
            {
                return FilePath.CoreFilesJsonFile;
            }

            public void LoadJson()
            {
                JObject rss = ClassLibrary1.IJsonWrapper.LoadJObject(GetPath());
                JArray jArray = (JArray)rss["core_files"];
                FileList = jArray.Select(c => (string)c).ToList();
            }

            public void SaveJson()
            {
                JObject rss = new JObject(
                    new JProperty("core_files", new JArray(
                        from f in FileList select new JValue(f)
                    )));

                ClassLibrary1.IJsonWrapper.SaveJObject(GetPath(), rss);
            }

            public CoreFilesJson LoadJsonAndReturn()
            {
                LoadJson();
                return this;
            }
        }

        public class InstallConfigJson : ClassLibrary1.IJsonWrapper.IBaseJsonWrapper, ClassLibrary1.IJsonWrapper.IExtraMethodForJsonWrapper<InstallConfigJson>
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

        public class VersionJson : ClassLibrary1.IJsonWrapper.IBaseJsonWrapper, ClassLibrary1.IJsonWrapper.IExtraMethodForJsonWrapper<VersionJson>
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
                JObject rss = ClassLibrary1.IJsonWrapper.LoadJObject(GetPath());

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

                ClassLibrary1.IJsonWrapper.SaveJObject(GetPath(), rss);
            }

            public VersionJson LoadJsonAndReturn()
            {
                LoadJson();
                return this;
            }

        }

    }
}
