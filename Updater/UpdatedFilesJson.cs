using System.Collections.Generic;
using System.Linq;
using CommonScripts;
using Newtonsoft.Json.Linq;

namespace Updater
{
    
    /// <summary>
    /// updated_files.json
    /// Maintains the list of files that is copied in previous update.
    /// </summary>
    public class UpdatedFilesJson : IJsonWrapper.IBaseJsonWrapper
    {

        public List<string> UpdatedFiles = new List<string>();


        private UpdatedFilesJson()
        {
        }

        public UpdatedFilesJson(List<string> updatedFiles)
        {
            this.UpdatedFiles = updatedFiles;
        }

        public static UpdatedFilesJson Load()
        {
            var updatedFilesJson = new UpdatedFilesJson();
            updatedFilesJson.LoadJson();
            return updatedFilesJson;
        }

        public string GetPath()
        {
            return FilePath.UpdatedFiles;
        }

        public void LoadJson()
        {
            JObject rss = CommonScripts.IJsonWrapper.LoadJObject(GetPath());
            JArray jArray = (JArray)rss["updated_files"];
            UpdatedFiles = jArray.Select(c => (string)c).ToList();

        }

        public void SaveJson()
        {
            JObject rss = new JObject(
                new JProperty("updated_files", new JArray(
                    from f in UpdatedFiles select new JValue(f)
                )));

            CommonScripts.IJsonWrapper.SaveJObject(GetPath(), rss);

        }
    }
}
