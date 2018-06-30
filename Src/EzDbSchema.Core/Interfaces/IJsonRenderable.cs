using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace EzDbSchema.Core.Interfaces
{
    public interface IJsonRenderable
    {
        string AsJson();
        void ToJsonFile(string FileName);
    }
}
