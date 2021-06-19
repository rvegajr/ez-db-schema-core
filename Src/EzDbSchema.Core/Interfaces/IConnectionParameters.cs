using System;
using System.Collections.Generic;

namespace EzDbSchema.Core.Interfaces
{
    public interface IConnectionParameters 
    {
		string Database { get; set; }
		string Server { get; set; }
        string UserName { get; set; }
        string Password { get; set; }
        bool Trusted { get; set; }
        string ConnectionString { get; set; }
        bool IsValid();
    }
}
