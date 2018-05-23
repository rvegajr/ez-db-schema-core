﻿using System;
using System.Collections.Generic;
using EzDbSchema.Core.Extentions;
using EzDbSchema.Core.Interfaces;

namespace EzDbSchema.Core.Objects
{
	public class CustomAttributes : Dictionary<string, object>, ICustomAttributes
    {
        public CustomAttributes()
        {
            this.Id = this.GetId();
        }
        public int Id { get; set; }
    }
}
