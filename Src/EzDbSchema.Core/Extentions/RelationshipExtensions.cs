using System;
using System.Linq;
using System.Text;
using EzDbSchema.Core.Interfaces;
using EzDbSchema.Core.Objects;

public static class RelationshipExtensions
{
    public static string RebuildObjectPointers(this Relationship relationship, IDatabase database, bool verbose)
    {
        StringBuilder debug = new StringBuilder();
        if (verbose)
        {
            debug.AppendLine($"Starting to rebuild pointers for relationship: {relationship.ConstraintName}");

            // Find FromEntity
            debug.AppendLine($"Looking for FromEntity with key: {relationship.FromTableName}");
        }
        var fromEntityKey = relationship.FromTableName;
        if (!database.Entities.TryGetValue(fromEntityKey, out var fromEntity))
        {
            debug.AppendLine($"Error: FromEntity not found for key {fromEntityKey}");
            throw new Exception(debug.ToString());
        }
        relationship.FromEntity = fromEntity;
        if (verbose) debug.AppendLine($"FromEntity found: {fromEntity.TableName}");

        // Find ToEntity
        if (verbose) debug.AppendLine($"Looking for ToEntity with key: {relationship.ToTableName}");
        var toEntityKey = relationship.ToTableName;
        if (!database.Entities.TryGetValue(toEntityKey, out var toEntity))
        {
            debug.AppendLine($"Error: ToEntity not found for key {toEntityKey}");
            throw new Exception(debug.ToString());
        }
        relationship.ToEntity = toEntity;
        if (verbose) debug.AppendLine($"ToEntity found: {toEntity.TableName}");

        // Find FromProperty
        if (verbose) debug.AppendLine($"Looking for FromProperty with name: {relationship.FromColumnName} in entity {fromEntityKey}");
        var fromProperty = fromEntity.Properties.Values.FirstOrDefault(p => p.ColumnName == relationship.FromColumnName);
        if (fromProperty == null)
        {
            debug.AppendLine($"Error: FromProperty not found for column {relationship.FromColumnName} in entity {fromEntityKey}");
            if (verbose)
            {
                debug.AppendLine("Available properties:");
                foreach (var prop in fromEntity.Properties.Values)
                {
                    debug.AppendLine($"- {prop.ColumnName}");
                }
            }
            throw new Exception(debug.ToString());
        }
        relationship.FromProperty = fromProperty;
        if (verbose) debug.AppendLine($"FromProperty found: {fromProperty.ColumnName}");

        // Find ToProperty
        if (verbose) debug.AppendLine($"Looking for ToProperty with name: {relationship.ToColumnName} in entity {toEntityKey}");
        var toProperty = toEntity.Properties.Values.FirstOrDefault(p => p.ColumnName == relationship.ToColumnName);
        if (toProperty == null)
        {
            debug.AppendLine($"Error: ToProperty not found for column {relationship.ToColumnName} in entity {toEntityKey}");
            if (verbose)
            {
                debug.AppendLine("Available properties:");
                foreach (var prop in toEntity.Properties.Values)
                {
                    debug.AppendLine($"- {prop.ColumnName}");
                }
            }
            throw new Exception(debug.ToString());
        }
        relationship.ToProperty = toProperty;
        if (verbose) debug.AppendLine($"ToProperty found: {toProperty.ColumnName}");

        if (verbose) debug.AppendLine("Relationship pointers rebuilt successfully.");
        return debug.ToString();
    }

    public static IDatabase RebuildAllRelationshipPointers(this Database database, bool verbose = false)
    {
        return ((IDatabase)database).RebuildAllRelationshipPointers(verbose);
    }

    public static IDatabase RebuildAllRelationshipPointers(this IDatabase database, bool verbose = false)
    {
        if (database == null) return null;
        if (verbose) Console.WriteLine("Starting to rebuild all relationship pointers.");
        else Console.WriteLine("Rebuilding relationships...");
        
        int totalRelationships = 0;
        int successfulRebuilds = 0;
        int failedRebuilds = 0;

        foreach (var entity in database.Entities.Values)
        {
            if (verbose) Console.WriteLine($"Processing entity: {entity.TableName}");
            foreach (var relationshipGroup in entity.RelationshipGroups.Values)
            {
                foreach (Relationship relationship in relationshipGroup)
                {
                    totalRelationships++;
                    try
                    {
                        string debugInfo = relationship.RebuildObjectPointers(database, verbose);
                        successfulRebuilds++;
                        if (verbose) Console.WriteLine(debugInfo);
                    }
                    catch (Exception ex)
                    {
                        failedRebuilds++;
                        if (verbose) Console.WriteLine($"Error rebuilding relationship: {ex.Message}");
                    }
                }
            }
        }

        Console.WriteLine($"Rebuild process completed.");
        Console.WriteLine($"Total relationships processed: {totalRelationships}");
        Console.WriteLine($"Successful rebuilds: {successfulRebuilds}");
        Console.WriteLine($"Failed rebuilds: {failedRebuilds}");
        return database;
    }
}