using System;
using System.Collections.Generic;
using System.Text;
using DocoptNet;

namespace CommonScripts
{

    /// <summary>
    /// Wrapper class to make using docopt a bit easier.
    /// </summary>
    public class DocoptWrapper
    {
        /// <summary>
        /// HashMap of ValueObjects returned by the docopt api
        /// </summary>
        private readonly IDictionary<string, ValueObject> _args;
        
        /// <summary>
        /// Runs docopt and saves the result.
        /// </summary>
        /// <param name="usage"></param>
        /// <param name="args"></param>
        public DocoptWrapper(string usage, string[] args)
        {
            this._args = new Docopt().Apply(usage, args, exit: true);
        }

        /// <summary>
        /// Helps keep the code clean for using docopt
        /// </summary>
        /// <param name="arguments">docopt idictionary obeject</param>
        /// <param name="key">key to search</param>
        /// <returns></returns>
        public ValueObject Get(string key)
        {
            ValueObject obj;
            _args.TryGetValue(key, out obj);
            return obj;
        }

        /// <summary>
        /// Helps avoid using ValueObject when a string is expected,
        /// returns null string if the target ValueObject is null.
        /// </summary>
        /// <param name="key">Key to search in the hashmap</param>
        /// <returns>null if ValueObject is null, else string</returns>
        public string GetString(string key)
        {
            return Get(key) == null? null : Get(key).ToString();
        }

        /// <summary>
        /// Helps avoid using ValueObject when an int is expected,
        /// returns -1 if the target ValueObject is null.
        /// </summary>
        /// <param name="key">Key to search in hashmap</param>
        /// <returns>-1 if the ValueObject is null, else integer</returns>
        public int GetInt(string key)
        {
            return Get(key) == null ? -1 : Get(key).AsInt;
        }
    }
}
