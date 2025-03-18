using System;
using Newtonsoft.Json;

namespace EzDbSchema.Core.Interfaces
{
    /// <summary>
    /// Interface for objects that can be rendered to JSON
    /// </summary>
    public interface IJsonRenderable
    {
        /// <summary>
        /// Renders the object as a JSON string
        /// </summary>
        string AsJson();
    }
}
