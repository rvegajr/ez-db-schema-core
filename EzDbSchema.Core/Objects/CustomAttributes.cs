using System;
using System.Collections.Generic;
using System.Text;
using EzDbSchema.Core.Extentions;
using EzDbSchema.Core.Interfaces;

namespace EzDbSchema.Core.Objects
{
    public class CustomAttributes : Dictionary<string, object>, ICustomAttributes
    {
        public string AsJson()
        {
            var sb = new StringBuilder();
            sb.Append("[");
            foreach (var key in this.Keys)
            {
                sb.AppendJsonParm(key, this[key].ToString());
            }
            sb.Append("]");
            return sb.ToString();
        }

        public ICustomAttributes FromJson(string jsonToConvert)
        {
            throw new NotImplementedException();
        }
    }
}
