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

        public class InstallConfigJson: IJsonWrapper.IBaseJsonWrapper
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

        public class UpdatedFilesJson : IJsonWrapper.IBaseJsonWrapper
        {

            public List<string> updatedFiles = new List<string>();


            public UpdatedFilesJson()
            {
            }

            public UpdatedFilesJson(List<string> updatedFiles)
            {
                this.updatedFiles = updatedFiles;
            }

            public string GetPath()
            {
                return FilePath.UpdatedFiles;
            }

            public void LoadJson()
            {
                JObject rss = ClassLibrary1.IJsonWrapper.LoadJObject(GetPath());
                JArray jArray = (JArray)rss["updated_files"];
                updatedFiles = jArray.Select(c => (string)c).ToList();

            }

            public void SaveJson()
            {
                JObject rss = new JObject(
                    new JProperty("updated_files", new JArray(
                        from f in updatedFiles select new JValue(f)
                    )));

                ClassLibrary1.IJsonWrapper.SaveJObject(GetPath(), rss);

            }
        }

        public class UpdaterVersionJson : VersionJson
        {
            public UpdaterVersionJson()
            {
            }

            public UpdaterVersionJson(VersionStruct version) : base(version)
            {
            }

            public UpdaterVersionJson(string folderId, int versionCode, string versionName, string softwareName) : base(folderId, versionCode, versionName, softwareName)
            {
            }

            public override string GetPath()
            {
                return FilePath.VersionJsonFile;
            }
        }

    }
}
