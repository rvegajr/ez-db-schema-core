namespace EzDbSchema.Core.Templates.Languages
{
    public interface ILanguageDefinition
    {
        string Name { get; }
        string FileExtension { get; }
        string GetDataType(string dbType, int? length = null, int? precision = null, int? scale = null);
        string GetDefaultValue(string dbType);
        string GetNullableType(string type);
        string GetCollectionType(string type);
        string GetImportStatement(string type);
        string GetClassDeclaration(string className, string baseClass = null, string[] interfaces = null);
        string GetPropertyDeclaration(string type, string name, bool isNullable, bool isReadOnly = false);
        string GetMethodDeclaration(string returnType, string name, MethodParameter[] parameters);
        string GetConstructorDeclaration(string className, MethodParameter[] parameters);
        string GetInterfaceDeclaration(string name, string[] baseInterfaces = null);
        string GetEnumDeclaration(string name);
        string GetEnumMember(string name, string value = null);
        string GetComment(string text);
        string GetRegionStart(string name);
        string GetRegionEnd();
        string GetNamespace(string name);
    }

    public class MethodParameter
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string DefaultValue { get; set; }
        public bool IsOptional => DefaultValue != null;
    }
}
