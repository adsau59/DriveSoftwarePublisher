using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ClassLibrary1
{
    /// <summary>
    /// InstallConfiguration json file that is used by updater
    /// </summary>
    public abstract class InstallConfigJson : ClassLibrary1.IJsonWrapper.IBaseJsonWrapper
    {
        public string FolderId;

        public InstallConfigJson()
        {
        }

        public InstallConfigJson(string folderId)
        {
            FolderId = folderId;
        }

        public abstract string GetPath();

        public void LoadJson()
        {
            JObject rss = ClassLibrary1.IJsonWrapper.LoadJObject(GetPath());

            FolderId = (string)rss["folder_id"];
        }

        public void SaveJson()
        {
            JObject rss = new JObject(
                new JProperty("folder_id", FolderId));

            ClassLibrary1.IJsonWrapper.SaveJObject(GetPath(), rss);
        }

        

    }
}
