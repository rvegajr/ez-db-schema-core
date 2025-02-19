using System.Collections.Generic;
using System.Linq;

namespace EzDbSchema.Core.CodeGen.Templates
{
    public static class ApiTemplates
    {
        public static IDictionary<string, string> GetSwaggerAnnotations(EntityCodeGenInfo entity)
        {
            return new Dictionary<string, string>
            {
                ["OpenApi"] = $@"
    /// <summary>
    /// {entity.ApiDescription}
    /// </summary>
    /// <response code=""200"">Success</response>
    /// <response code=""400"">Bad Request</response>
    /// <response code=""404"">Not Found</response>
    [ApiController]
    [Route(""{entity.ApiEndpoint}"")]
    [Produces(""application/json"")]
    [Tags(""{entity.ApiTag}"")]",

                ["SwaggerOperation"] = $@"
    /// <summary>
    /// Get all {entity.ClassNamePlural}
    /// </summary>
    /// <returns>List of {entity.ClassNamePlural}</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<{entity.ClassName}>), 200)]",

                ["SwaggerParameter"] = $@"
    /// <param name=""id"">The {entity.ClassName} identifier</param>
    /// <returns>The {entity.ClassName}</returns>
    [HttpGet(""{{{entity.VariableName}Id}}"")]
    [ProducesResponseType(typeof({entity.ClassName}), 200)]
    [ProducesResponseType(404)]"
            };
        }

        public static IDictionary<string, string> GetGraphQLTypes(EntityCodeGenInfo entity)
        {
            return new Dictionary<string, string>
            {
                ["Type"] = $@"
type {entity.ClassName} @key(fields: ""{string.Join(" ", entity.PrimaryKeys.Select(pk => pk.PropertyName))}"") {{
    {string.Join("\n    ", entity.Properties.Select(p => $"{p.VariableName}: {GetGraphQLType(p)}"))}
}}",

                ["Input"] = $@"
input {entity.ClassName}Input {{
    {string.Join("\n    ", entity.Properties.Where(p => !p.IsIdentity).Select(p => $"{p.VariableName}: {GetGraphQLType(p)}"))}
}}",

                ["Query"] = $@"
extend type Query {{
    {entity.VariableName}(id: ID!): {entity.ClassName}
    {entity.VariableNamePlural}: [{entity.ClassName}!]!
}}",

                ["Mutation"] = $@"
extend type Mutation {{
    create{entity.ClassName}(input: {entity.ClassName}Input!): {entity.ClassName}!
    update{entity.ClassName}(id: ID!, input: {entity.ClassName}Input!): {entity.ClassName}!
    delete{entity.ClassName}(id: ID!): Boolean!
}}"
            };
        }

        public static IDictionary<string, string> GetRestEndpoints(EntityCodeGenInfo entity)
        {
            return new Dictionary<string, string>
            {
                ["GetAll"] = $@"
    [HttpGet]
    public async Task<ActionResult<IEnumerable<{entity.ClassName}>>> GetAll{entity.ClassNamePlural}()
    {{
        var items = await _{entity.VariableName}Service.GetAllAsync();
        return Ok(items);
    }}",

                ["GetById"] = $@"
    [HttpGet(""{{{entity.VariableName}Id}}"")]
    public async Task<ActionResult<{entity.ClassName}>> Get{entity.ClassName}(int id)
    {{
        var item = await _{entity.VariableName}Service.GetByIdAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }}",

                ["Create"] = $@"
    [HttpPost]
    public async Task<ActionResult<{entity.ClassName}>> Create{entity.ClassName}({entity.ClassName}Dto dto)
    {{
        var item = await _{entity.VariableName}Service.CreateAsync(dto);
        return CreatedAtAction(nameof(Get{entity.ClassName}), new {{ id = item.Id }}, item);
    }}",

                ["Update"] = $@"
    [HttpPut(""{{{entity.VariableName}Id}}"")]
    public async Task<IActionResult> Update{entity.ClassName}(int id, {entity.ClassName}Dto dto)
    {{
        await _{entity.VariableName}Service.UpdateAsync(id, dto);
        return NoContent();
    }}",

                ["Delete"] = $@"
    [HttpDelete(""{{{entity.VariableName}Id}}"")]
    public async Task<IActionResult> Delete{entity.ClassName}(int id)
    {{
        await _{entity.VariableName}Service.DeleteAsync(id);
        return NoContent();
    }}"
            };
        }

        private static string GetGraphQLType(PropertyCodeGenInfo prop)
        {
            var type = prop.DataType.ToLowerInvariant() switch
            {
                "int" => "Int",
                "bigint" => "Int",
                "smallint" => "Int",
                "decimal" => "Float",
                "float" => "Float",
                "bit" => "Boolean",
                "datetime" => "DateTime",
                _ => "String"
            };

            return prop.IsNullable ? type : $"{type}!";
        }
    }
}
