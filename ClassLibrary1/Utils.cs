using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class Utils
    {
        /// <summary>
        /// Deletes everything in a directory
        /// </summary>
        /// <param name="directory">directory of the folder to be emptied</param>
        public static void Empty(System.IO.DirectoryInfo directory)
        {
            foreach (System.IO.FileInfo file in directory.GetFiles()) file.Delete();
            foreach (System.IO.DirectoryInfo subDirectory in directory.GetDirectories()) subDirectory.Delete(true);
        }

        /// <summary>
        /// Parse the file name into versioncode
        /// versioncode--softwarename--versionname.zip
        /// </summary>
        /// <param name="name">name to be passed</param>
        /// <returns>version code</returns>
        public static int GetVersionCodeFromName(string name)
        {
            return int.Parse(name.Split(new[] {"--"}, StringSplitOptions.None)[0]);
        }

        /// <summary>
        /// Parse the file name into name parts
        /// versioncode--softwarename--versionname.zip
        /// </summary>
        /// <param name="name">name to be passed</param>
        /// <returns>{"versioncode","softwarename","versionname"}</returns>
        public static string[] GetVersionDetailFromName(string name)
        {
            return name.Split(new[] { ".zip" }, StringSplitOptions.None)[0]
                .Split(new[] { "--" }, StringSplitOptions.None);
        }
    }
}
