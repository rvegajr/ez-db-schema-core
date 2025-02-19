using System;
using System.Collections.Generic;
using System.Text;

namespace EzDbSchema.Core.Interfaces
{
    public interface IJsonRenderable
    {
        string AsJson();
        void ToJsonFile(string fileName);
    }
}
