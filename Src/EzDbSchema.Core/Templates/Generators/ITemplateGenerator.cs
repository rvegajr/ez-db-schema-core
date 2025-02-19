using System.Collections.Generic;
using EzDbSchema.Core.Interfaces;

namespace EzDbSchema.Core.Templates.Generators
{
    public interface ITemplateGenerator
    {
        string Name { get; }
        string Description { get; }
        IDictionary<string, string> GenerateFiles(IDatabase database, GeneratorOptions options);
    }

    public class GeneratorOptions
    {
        public string Language { get; set; } = "C#";
        public string Namespace { get; set; }
        public string OutputPath { get; set; }
        public bool GenerateInterfaces { get; set; } = true;
        public bool GenerateValidation { get; set; } = true;
        public bool GenerateDocumentation { get; set; } = true;
        public bool GenerateTests { get; set; } = true;
        public Dictionary<string, object> AdditionalOptions { get; set; } = new();
    }
}
