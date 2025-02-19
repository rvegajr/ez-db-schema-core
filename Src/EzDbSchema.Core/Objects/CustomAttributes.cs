using System;
using System.Collections.Generic;
using EzDbSchema.Core.Extentions;
using EzDbSchema.Core.Extentions.Objects;
using EzDbSchema.Core.Interfaces;

namespace EzDbSchema.Core.Objects
{
	public class CustomAttributes : Dictionary<string, object>, ICustomAttributes
    {
        internal static string ALIAS = "CustomAttributes";

        public CustomAttributes()
        {
            this._id = this.GetId();
        }
        public int _id { get; set; }
    }

    [System.AttributeUsage(System.AttributeTargets.Interface |
                           System.AttributeTargets.Property,
                           AllowMultiple = false)  // multiuse attribute  
    ]
    public class AsRef : System.Attribute
    {
        public AsRef(string referenceFieldName)
        {
            ReferenceFieldName = referenceFieldName;
        }
        public string ReferenceFieldName { get; set; }
    }
}
