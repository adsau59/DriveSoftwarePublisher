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
    /// <summary>
    /// Json Helper class
    /// </summary>
    public class IJsonWrapper
    {
        /// <summary>
        /// Interface for json files
        /// </summary>
        public interface IBaseJsonWrapper
        {
            string GetPath();
            void LoadJson();
            void SaveJson();

        }

        /// <summary>
        /// Loads json file
        /// </summary>
        /// <param name="path">Path of the json file</param>
        /// <returns>JObject Object</returns>
        public static JObject LoadJObject(string path)
        {
            JObject rss = null;
            try
            {
                string jsonString = File.ReadAllText(path);
                rss = JObject.Parse(jsonString);
            }
            catch (System.IO.FileNotFoundException e)
            {
                Console.WriteLine($"{path} not found");
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine($"{path} not found");
            }

            return rss;
        }

        /// <summary>
        /// Save the json file.
        /// </summary>
        /// <param name="path">Path of the json file to be saved</param>
        /// <param name="rss">JObject object</param>
        public static void SaveJObject(string path, JObject rss)
        {
            //todo create directory if not exist
            System.IO.Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, rss.ToString());
        }


    }
}
