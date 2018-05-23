using System;
using System.Collections.Generic;
using EzDbSchema.Core.Extentions;
using EzDbSchema.Core.Interfaces;

namespace EzDbSchema.Core.Objects
{
    /// <summary></summary>
    public class EzObject : IEzObject
    {
        public EzObject()
        {
            this.Id = this.GetId();
        }
        public int Id { get; set; }
    }
}
