using System;
using System.Collections.Generic;
using System.Text;

namespace EzDbSchema.Core.Interfaces
{
    public interface IEzObject
    {
        int Id { get; set; }
    }

    public interface IEzObjectJson<T>
    {
        string AsJson();
        T FromJson(string Json);
    }
}
