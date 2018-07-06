using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace ClassLibrary1
{
    public class IJsonWrapper
    {
        public interface IBaseJsonWrapper
        {
            string GetPath();
            void LoadJson();
            void SaveJson();

        }

        public interface IExtraMethodForJsonWrapper<T> where T : IBaseJsonWrapper
        {
            T LoadJsonAndReturn();
        }


        public static JObject LoadJObject(string path)
        {
            string jsonString = File.ReadAllText(path);
            JObject rss = JObject.Parse(jsonString);

            return rss;
        }

        public static void SaveJObject(string path, JObject rss)
        {
            File.WriteAllText(path, rss.ToString());
        }


    }
}
