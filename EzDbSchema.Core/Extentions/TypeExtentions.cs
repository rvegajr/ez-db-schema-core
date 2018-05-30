using System;
using System.Collections.Generic;

namespace EzDbSchema.Core.Extentions.Types
{
    public static class TypeExtensions
    {

        //this dictionary will store the types that have been encountered already
        static Dictionary<string, List<string>> TypeCache = new Dictionary<string, List<string>>();
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns></returns>
        public static bool TypeExists(this Type t, string cacheName)
        {
            return TypeCache.ContainsKey(cacheName) && TypeCache[cacheName].Contains(t.Name);
        }

        /// <summary>
        /// Gets an identifier for an object
        /// </summary>
        /// <param name="o">EZObject- if the object already exists,  use its ide</param>
        /// <param name="valToCheck">Value to check.  If it is greater than 0,  then return it</param>
        /// <returns></returns>
        public static Type TypeTag(this Type t, string cacheName)
        {
            if (!TypeCache.ContainsKey(cacheName)) TypeCache.Add(cacheName, new List<string>());
            TypeCache[cacheName].Add(t.Name);
            return t;
        }
    }
}