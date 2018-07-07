using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary1;
using Newtonsoft.Json.Linq;

namespace SoftwareUpdater
{
    /// <summary>
    /// Json wrapper of json files used in this project
    /// </summary>
    class JsonWrapper
    {
        /// <summary>
        /// install_config.json
        /// saves the folder id of the software that is to be downloaded.
        /// </summary>
        public class UpdaterInstallConfigJson: InstallConfigJson
        {
            public UpdaterInstallConfigJson()
            {
            }

            public UpdaterInstallConfigJson(string folderId) : base(folderId)
            {
            }

            public override string GetPath()
            {
                return FilePath.InstallConfigFile;
            }

            public UpdaterInstallConfigJson LoadJsonAndReturn()
            {
                LoadJson();
                return this;
            }


        }

        /// <summary>
        /// updated_files.json
        /// Maintains the list of files that is copied in previous update.
        /// </summary>
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

        /// <summary>
        /// version.json
        /// saves the current version name and version code of the software.
        /// </summary>
        public class UpdaterVersionJson : VersionJson
        {
            public UpdaterVersionJson()
            {
            }

            public UpdaterVersionJson(VersionDetailStruct versionDetail) : base(versionDetail)
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
