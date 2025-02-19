using System.Collections.Generic;
using System.Linq;

namespace EzDbSchema.Core.CodeGen.Templates
{
    public static class ORMTemplates
    {
        public static IDictionary<string, string> GetEntityFrameworkMappings(EntityCodeGenInfo entity)
        {
            return new Dictionary<string, string>
            {
                ["EntityConfiguration"] = $@"
public class {entity.ClassName}Configuration : IEntityTypeConfiguration<{entity.ClassName}>
{{
    public void Configure(EntityTypeBuilder<{entity.ClassName}> builder)
    {{
        builder.ToTable(""{entity.TableName}"", ""{entity.SchemaName}"");

        {string.Join("\n        ", entity.Properties.Select(p => GetEFPropertyConfiguration(p)))}

        {string.Join("\n        ", entity.OneToManyRelationships.Select(r => GetEFRelationshipConfiguration(r)))}
    }}
}}",

                ["DbContext"] = $@"
public DbSet<{entity.ClassName}> {entity.ClassNamePlural} {{ get; set; }}

protected override void OnModelCreating(ModelBuilder modelBuilder)
{{
    modelBuilder.ApplyConfiguration(new {entity.ClassName}Configuration());
}}"
            };
        }

        public static IDictionary<string, string> GetTypeORMMappings(EntityCodeGenInfo entity)
        {
            return new Dictionary<string, string>
            {
                ["EntityDecorators"] = $@"
@Entity({{ name: '{entity.TableName}', schema: '{entity.SchemaName}' }})
export class {entity.ClassName} {{
    {string.Join("\n    ", entity.Properties.Select(p => GetTypeORMPropertyDecorator(p)))}

    {string.Join("\n    ", entity.OneToManyRelationships.Select(r => GetTypeORMRelationshipDecorator(r)))}
}}",

                ["Module"] = $@"
@Module({{
    imports: [TypeOrmModule.forFeature([{entity.ClassName}])],
    controllers: [{entity.ClassName}Controller],
    providers: [{entity.ClassName}Service],
}})
export class {entity.ClassName}Module {{}}"
            };
        }

        public static IDictionary<string, string> GetSQLAlchemyMappings(EntityCodeGenInfo entity)
        {
            return new Dictionary<string, string>
            {
                ["Model"] = $@"
class {entity.ClassName}(Base):
    __tablename__ = '{entity.TableName}'
    __table_args__ = {{'schema': '{entity.SchemaName}'}}

    {string.Join("\n    ", entity.Properties.Select(p => GetSQLAlchemyPropertyDefinition(p)))}

    {string.Join("\n    ", entity.OneToManyRelationships.Select(r => GetSQLAlchemyRelationshipDefinition(r)))}"
            };
        }

        private static string GetEFPropertyConfiguration(PropertyCodeGenInfo prop)
        {
            var config = new List<string>();

            if (prop.IsPrimaryKey)
                config.Add(".HasKey(e => e." + prop.PropertyName + ")");

            if (prop.IsIdentity)
                config.Add(".ValueGeneratedOnAdd()");

            if (prop.MaxLength > 0)
                config.Add($".HasMaxLength({prop.MaxLength})");

            if (!prop.IsNullable)
                config.Add(".IsRequired()");

            if (prop.IsText && prop.MaxLength == -1)
                config.Add(".IsUnicode()");

            if (prop.Precision > 0)
                config.Add($".HasPrecision({prop.Precision}, {prop.Scale})");

            return $"builder.Property(e => e.{prop.PropertyName})" + string.Join("", config) + ";";
        }

        private static string GetEFRelationshipConfiguration(RelationshipInfo rel)
        {
            if (rel.IsOneToMany)
            {
                return $@"builder.HasMany(e => e.{rel.NavigationPropertyName})
                   .WithOne(e => e.{rel.InverseNavigationPropertyName})
                   .HasForeignKey(e => e.{rel.ForeignKeyPropertyName})
                   .OnDelete(DeleteBehavior.{rel.CascadeOption});";
            }
            
            return $@"builder.HasOne(e => e.{rel.NavigationPropertyName})
                   .WithMany(e => e.{rel.InverseNavigationPropertyName})
                   .HasForeignKey(e => e.{rel.ForeignKeyPropertyName})
                   .OnDelete(DeleteBehavior.{rel.CascadeOption});";
        }

        private static string GetTypeORMPropertyDecorator(PropertyCodeGenInfo prop)
        {
            var decorators = new List<string>();

            if (prop.IsPrimaryKey)
                decorators.Add("@PrimaryGeneratedColumn()");
            else
                decorators.Add($@"@Column({{
        name: '{prop.ColumnName}',
        type: '{GetTypeORMColumnType(prop)}',
        length: {prop.MaxLength},
        nullable: {prop.IsNullable.ToString().ToLower()},
        precision: {prop.Precision},
        scale: {prop.Scale}
    }})");

            return string.Join("\n    ", decorators) + $"\n    {prop.VariableName}: {prop.LanguageType};";
        }

        private static string GetTypeORMColumnType(PropertyCodeGenInfo prop)
        {
            return prop.DataType.ToLowerInvariant() switch
            {
                "int" => "int",
                "bigint" => "bigint",
                "decimal" => "decimal",
                "float" => "float",
                "bit" => "boolean",
                "datetime" => "timestamp",
                "varchar" => "varchar",
                "text" => "text",
                _ => "varchar"
            };
        }

        private static string GetTypeORMRelationshipDecorator(RelationshipInfo rel)
        {
            if (rel.IsOneToMany)
            {
                return $@"@OneToMany(() => {rel.ToClassName}, {rel.VariableName} => {rel.VariableName}.{rel.InverseNavigationPropertyName})
    {rel.NavigationPropertyName}: {rel.ToClassName}[];";
            }
            
            return $@"@ManyToOne(() => {rel.ToClassName}, {rel.VariableName} => {rel.VariableName}.{rel.InverseNavigationPropertyName})
    @JoinColumn({{ name: '{rel.ForeignKeyPropertyName}' }})
    {rel.NavigationPropertyName}: {rel.ToClassName};";
        }

        private static string GetSQLAlchemyPropertyDefinition(PropertyCodeGenInfo prop)
        {
            var type = GetSQLAlchemyColumnType(prop);
            var constraints = new List<string>();

            if (prop.IsPrimaryKey)
                constraints.Add("primary_key=True");
            if (!prop.IsNullable)
                constraints.Add("nullable=False");
            if (prop.MaxLength > 0)
                constraints.Add($"length={prop.MaxLength}");
            if (prop.IsIdentity)
                type = "Integer, Sequence('{prop.ColumnName}_seq')";

            var constraintsStr = constraints.Any() ? ", " + string.Join(", ", constraints) : "";
            return $"{prop.VariableName} = Column({type}{constraintsStr})";
        }

        private static string GetSQLAlchemyColumnType(PropertyCodeGenInfo prop)
        {
            return prop.DataType.ToLowerInvariant() switch
            {
                "int" => "Integer",
                "bigint" => "BigInteger",
                "decimal" => $"Numeric(precision={prop.Precision}, scale={prop.Scale})",
                "float" => "Float",
                "bit" => "Boolean",
                "datetime" => "DateTime",
                "varchar" => "String",
                "text" => "Text",
                _ => "String"
            };
        }

        private static string GetSQLAlchemyRelationshipDefinition(RelationshipInfo rel)
        {
            if (rel.IsOneToMany)
            {
                return $"{rel.NavigationPropertyName} = relationship('{rel.ToClassName}', back_populates='{rel.InverseNavigationPropertyName}')";
            }
            
            return $"{rel.NavigationPropertyName} = relationship('{rel.ToClassName}', back_populates='{rel.InverseNavigationPropertyName}', uselist=False)";
        }
    }
}
