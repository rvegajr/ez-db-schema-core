﻿using System;
using EzDbSchema.Core.Interfaces;

namespace EzDbSchema.Core.Objects
{
	public class DatabaseObjectUpdates : IDatabaseObjectUpdates
	{
        public DateTime? LastCreated { get; set; } = null;
        public DateTime? LastModified { get; set; } = null;
        public string LastItemCreated { get; set; } = "";
        public string LastItemModified { get; set; } = "";
    }
}