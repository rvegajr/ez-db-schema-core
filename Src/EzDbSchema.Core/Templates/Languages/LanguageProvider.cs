using System;
using System.Collections.Generic;

namespace EzDbSchema.Core.Templates.Languages
{
    public class LanguageProvider
    {
        private static readonly Dictionary<string, ILanguageDefinition> _languages = new();

        static LanguageProvider()
        {
            RegisterLanguage(new CSharpLanguage());
            RegisterLanguage(new TypeScriptLanguage());
            RegisterLanguage(new JavaLanguage());
            RegisterLanguage(new PythonLanguage());
            RegisterLanguage(new GoLanguage());
            RegisterLanguage(new SqlLanguage());
        }

        public static void RegisterLanguage(ILanguageDefinition language)
        {
            _languages[language.Name.ToLowerInvariant()] = language;
        }

        public static ILanguageDefinition GetLanguage(string name)
        {
            return _languages.TryGetValue(name.ToLowerInvariant(), out var language) 
                ? language 
                : throw new ArgumentException($"Language '{name}' not supported");
        }

        public static bool IsLanguageSupported(string name)
        {
            return _languages.ContainsKey(name.ToLowerInvariant());
        }

        public static IEnumerable<string> GetSupportedLanguages()
        {
            return _languages.Keys;
        }
    }
}
