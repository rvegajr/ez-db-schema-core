using System;
using System.Collections.Generic;
using System.Text;

namespace EzDbSchema.Core.Interfaces
{
    public interface IEzObject
    {
        string DatabaseObjectName { get; }
        int _id { get; set; }
        bool IsEnabled { get; set; }
        ICustomAttributes CustomAttributes { get; set; }
    }
}
