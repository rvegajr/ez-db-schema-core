using System;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[assembly: InternalsVisibleTo("EzDbSchema.MsSql")]
[assembly: InternalsVisibleTo("EzDbCodeGen.Core")]

namespace EzDbSchema.Core.Extentions.Json
{
    public static class JsonExtensions
    {
        public static string AsString(this object obj)
        {
            if (obj == null) return string.Empty;
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }

        public static T FromJson<T>(this string json) where T : class
        {
            if (string.IsNullOrEmpty(json)) return null;
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static JObject ToJObject(this string json)
        {
            if (string.IsNullOrEmpty(json)) return null;
            return JObject.Parse(json);
        }

        public static JArray ToJArray(this string json)
        {
            if (string.IsNullOrEmpty(json)) return null;
            return JArray.Parse(json);
        }
    }
}