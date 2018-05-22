using System;
using System.Collections.Generic;
using System.Text;

namespace EzDbSchema.Core.Interfaces
{
    public interface IJson<T>
    {
        string AsJson();
        T FromJson(string jsonToConvert);
    }
}
