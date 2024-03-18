using System;
using System.Collections;
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
        bool TrustServerCertificate { get; set; }
        ICollection Values { get; }
        string ConnectionString { get; set; }
        bool IsValid();
    }
}
