using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using EzDbSchema.Core.Interfaces;
using EzDbSchema.Core.Objects;
using EzDbSchema.Core.Extentions;
using EzDbSchema.Core.Extentions.Objects;
using EzDbSchema.Core.Extentions.Strings;
using EzDbSchema.Core.Enums;

namespace EzDbSchema.MsSql
{
	public class Database : EzDbSchema.Core.Objects.Database, IDatabase
    {
		public override IDatabase Render(string entityName, string ConnectionString)
        {
			IDatabase schema = this;
			schema.Name = entityName;

            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    var OriginalCompatabilityLevel = 0;
                    if (connection.State == ConnectionState.Closed) connection.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT compatibility_level FROM sys.databases WHERE name = DB_NAME()", connection))
                    {
                        OriginalCompatabilityLevel = int.Parse(cmd.ExecuteScalar().ToString());
                    }
                    /* For some reason that I have not investigated,  calling schema queries takes MUCH more time on any COMPATIBILITY_LEVEL > 110,  this will temporarily obtain the 
                     * current COMPATIBILITY_LEVEL and save it,  then restore it after this query has run
                     */
                    if (OriginalCompatabilityLevel > 110)
                    {
                        SqlCommand cmdCompat11 = new SqlCommand("DECLARE @sql NVARCHAR(1000) = 'ALTER DATABASE ' + DB_NAME() + ' SET COMPATIBILITY_LEVEL = 110'; EXECUTE sp_executesql @sql", connection);
                        cmdCompat11.CommandType = CommandType.Text;
                        cmdCompat11.ExecuteNonQuery();
                    }
                    using (SqlCommand cmd = new SqlCommand(FKSQL, connection))
                    {
                        cmd.CommandType = CommandType.Text;

                        var ds = new DataSet();
                        var adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(ds);
                        var tableColumnData = ds.Tables[0];
                        var currentSchemaObjectName = "";
                        var schemaObjectName = "";
                        IEntity entityType = null;
                        PrimaryKeyProperties primaryKeyList = null;
                        foreach (DataRow row in tableColumnData.Rows)
                        {
                            schemaObjectName = string.Format("{0}.{1}", row["SCHEMANAME"], row["TABLENAME"]);
                            if (currentSchemaObjectName != schemaObjectName)
                            {
                                if (currentSchemaObjectName.Length > 0)
                                {
                                    //Temporal views are handled below
                                    if ((!entityType.HasPrimaryKeys() && (!entityType.IsTemporalView) && (entityType.TemporalType != "HISTORY_TABLE")))
                                    {
                                        Console.WriteLine("Warning... no primary keys for " + schemaObjectName + ".. adding all as primary keys");
                                        int order = 0;
                                        foreach (var prop in entityType.Properties.Values)
                                        {
                                            order++;
                                            prop.IsKey = true;
                                            prop.KeyOrder = order;
                                            entityType.PrimaryKeys.Add(prop);
                                        }
                                    }
                                    schema.Add(currentSchemaObjectName, entityType);
                                }
								entityType = new Entity()
                                {
                                    Name = row["TABLENAME"].ToString()
                                    , Alias = schemaObjectName
                                    , Type = row["OBJECT_TYPE"].ToString()
                                    , Schema = row["SCHEMANAME"].ToString()
                                    , IsTemporalView = ((schemaObjectName.EndsWith("TemporalView", StringComparison.Ordinal)) && (row["OBJECT_TYPE"].ToString() == "VIEW"))
                                    , TemporalType = (row["TEMPORAL_TYPE_DESC"] == DBNull.Value ? "" : row["TEMPORAL_TYPE_DESC"].ToString())
                                    , Parent = schema
                                };
                                primaryKeyList = new PrimaryKeyProperties(entityType);
                                entityType.PrimaryKeys = primaryKeyList;
                                currentSchemaObjectName = schemaObjectName;
                            }
                            Property property = new Property() { IsNullable = (bool)row["IS_NULLABLE"] };
                            property.Name = (row["COLUMNNAME"] == DBNull.Value ? "" : row["COLUMNNAME"].ToString());
                            property.Alias = property.Name + (property.Name.Equals(schemaObjectName) ? "_Text" : "");
                            property.MaxLength = (int)(row["CHARACTER_MAXIMUM_LENGTH"] == DBNull.Value ? 0 : row["CHARACTER_MAXIMUM_LENGTH"]);
                            property.Precision = (int)(row["NUMERIC_PRECISION"] == DBNull.Value ? 0 : Convert.ToInt32(row["NUMERIC_PRECISION"]));
                            property.Scale = (int)(row["NUMERIC_SCALE"] == DBNull.Value ? 0 : Convert.ToInt32(row["NUMERIC_SCALE"]));
                            property.IsIdentity = (bool)(row["IS_IDENTITY"] == DBNull.Value ? false : row["IS_IDENTITY"]);
                            property.IsKey = (bool)(row["PRIMARY_KEY_ORDER"] == DBNull.Value ? false : true);
                            property.KeyOrder = (int)(row["PRIMARY_KEY_ORDER"] == DBNull.Value ? 0 : Convert.ToInt32(row["PRIMARY_KEY_ORDER"]));
                            if ((entityType.IsTemporalView) && ((property.Name == "SysEndTime") || (property.Name == "SysStartTime")))
                            {
                                property.IsKey = true;
                                property.KeyOrder = ((property.Name == "SysStartTime") ? 1 : 2);
                            }
                            property.Type = (row["DATA_TYPE"] == DBNull.Value ? "" : row["DATA_TYPE"].ToString());
                            if (property.IsKey) entityType.PrimaryKeys.Add(property);
                            property.Parent = entityType;
                            entityType.Properties.Add(property.Name, property);
                        }
                        if (currentSchemaObjectName.Length > 0)
                        {
                            //Temporal views are handled abolve
                            if ((!entityType.HasPrimaryKeys() && (!entityType.IsTemporalView) && (entityType.TemporalType != "HISTORY_TABLE")))
                            {
                                if (ShowWarnings) Console.WriteLine("Warning... no primary keys for " + currentSchemaObjectName + ".. adding all as primary keys");
                                int order = 0;
                                foreach (var prop in entityType.Properties.Values)
                                {
                                    order++;
                                    prop.IsKey = true;
                                    prop.KeyOrder = order;
                                    entityType.PrimaryKeys.Add(prop);
                                }
                            }
                            schema.Add(currentSchemaObjectName, entityType);
                        }

                        var tableFKeyData = ds.Tables[1];
                        foreach (DataRow row in tableFKeyData.Rows)
                        {
                            var entityKey = string.Format("{0}.{1}", row["EntitySchema"], row["EntityTable"]);
                            var toEntityKey = string.Format("{0}.{1}", row["RelatedSchema"], row["RelatedTable"]);
                            if (!schema.ContainsKey(entityKey))
                                throw new Exception(string.Format("Entity Key {0} was not found.", entityKey));
                            var relatedEntityKey = string.Format("{0}.{1}", row["RelatedSchema"], row["RelatedTable"]);
                            if (!schema.ContainsKey(relatedEntityKey))
                                throw new Exception(string.Format("Related Entity Key {0} was not found.", relatedEntityKey));

                            string fromEntity = (row["EntityTable"] == DBNull.Value ? "" : row["EntityTable"].ToString());
                            string fromEntityField = (row["EntityColumn"] == DBNull.Value ? "" : row["EntityColumn"].ToString());
                            string toEntity = (row["RelatedTable"] == DBNull.Value ? "" : row["RelatedTable"].ToString());
                            string toEntityField = (row["RelatedColumn"] == DBNull.Value ? "" : row["RelatedColumn"].ToString());
                            string toEntityColumnName = toEntityField.AsFormattedName();
                            string fromEntityColumnName = fromEntityField.AsFormattedName();
                            string multiplicity = (row["Multiplicity"] == DBNull.Value ? "" : row["Multiplicity"].ToString());
                            string primaryTableName = (row["PrimaryTableName"] == DBNull.Value ? "" : row["PrimaryTableName"].ToString());
                            int ordinalPosition = (row["FKOrdinalPosition"] == DBNull.Value ? 0 : row["FKOrdinalPosition"].AsInt(0));
                            try
                            {
                                var newRel = new Relationship()
                                {
                                    Name = row["FK_Name"] == DBNull.Value ? "" : row["FK_Name"].ToString(),
                                    FromTableName = entityKey,
                                    FromFieldName = fromEntityField,
                                    FromColumnName = fromEntityColumnName,
                                    FromEntity = schema[entityKey],
                                    FromProperty = schema[entityKey].Properties[fromEntityField],
                                    ToFieldName = toEntityField,
                                    ToTableName = relatedEntityKey,
                                    ToColumnName = toEntityColumnName,
                                    ToEntity = schema[toEntityKey],
                                    ToProperty = schema[toEntityKey].Properties[toEntityField],
                                    Type = multiplicity,
                                    PrimaryTableName = primaryTableName,
                                    FKOrdinalPosition = ordinalPosition,
                                    Parent = schema[entityKey],
                                    MultiplicityType = RelationshipMultiplicityType.Unknown
                                };
                                switch (newRel.Type.ToLower())
                                {
                                    case "one to one":
                                        newRel.MultiplicityType = RelationshipMultiplicityType.OneToOne;
                                        break;
                                    case "one to many":
                                        newRel.MultiplicityType = RelationshipMultiplicityType.OneToMany;
                                        break;
                                    case "zeroorone to many":
                                        newRel.MultiplicityType = RelationshipMultiplicityType.ZeroOrOneToMany;
                                        break;
                                    case "many to one":
                                        newRel.MultiplicityType = RelationshipMultiplicityType.ManyToOne;
                                        break;
                                    case "many to zeroorone":
                                        newRel.MultiplicityType = RelationshipMultiplicityType.ManyToZeroOrOne;
                                        break;
                                    case "one to zeroorone":
                                        newRel.MultiplicityType = RelationshipMultiplicityType.OneToZeroOrOne;
                                        break;
                                    case "zeroorone to one":
                                        newRel.MultiplicityType = RelationshipMultiplicityType.ZeroOrOneToOne;
                                        break;
                                }

                                schema[entityKey].Relationships.Add(newRel);
                                var fieldToMarkRelation = (entityKey.Equals(newRel.FromTableName) ? newRel.FromFieldName : newRel.ToFieldName);
                                if (schema[entityKey].Properties.ContainsKey(fieldToMarkRelation))
                                {
                                    schema[entityKey].Properties[fieldToMarkRelation].RelatedTo.Add(newRel);
                                }
                                if (string.IsNullOrEmpty(newRel.Name)) throw new Exception("FK Namne is missing from the relationship");
                                if (!schema[entityKey].RelationshipGroups.ContainsKey(newRel.Name))
                                    schema[entityKey].RelationshipGroups.Add(newRel.Name, new RelationshipList());
                                schema[entityKey].RelationshipGroups[newRel.Name].Add(newRel);

                            }
                            catch (Exception relEx)
                            {
                                throw new Exception(string.Format("Error while adding relationship {0}. {1}", 
                                    row["FK_Name"] == DBNull.Value ? "" : row["FK_Name"].ToString(), relEx.Message
                                ), relEx);
                            }

                        }

                        var tableLastDateTime = ds.Tables[2];
                        foreach (DataRow row in tableLastDateTime.Rows)
                        {
                            schema.LastUpdates.LastCreated = row["LastCreate"].AsDateTimeNullable(null);
                            schema.LastUpdates.LastModified = row["LastUpdate"].AsDateTimeNullable(null);
                            schema.LastUpdates.LastItemCreated = row["LastItemCreated"].ToSafeString();
                            schema.LastUpdates.LastItemModified = row["LastItemUpdate"].ToSafeString();
                        }

                    }

                    var itemCount = schema.Keys.Count();
                    if (OriginalCompatabilityLevel > 110)
                    {
                        SqlCommand cmdCompat13 = new SqlCommand("DECLARE @sql NVARCHAR(1000) = 'ALTER DATABASE ' + DB_NAME() + ' SET COMPATIBILITY_LEVEL = " + OriginalCompatabilityLevel.ToString() + "'; EXECUTE sp_executesql @sql", connection);
                        cmdCompat13.CommandType = CommandType.Text;
                        cmdCompat13.ExecuteNonQuery();
                    }
                    return schema;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        private static string FKSQL = @"
SET NOCOUNT ON
IF OBJECT_ID('tempdb..#IDX') IS NOT NULL DROP TABLE #IDX; 
IF OBJECT_ID('tempdb..#COL') IS NOT NULL DROP TABLE #COL ;
IF OBJECT_ID('tempdb..#FK') IS NOT NULL DROP TABLE #FK ;
IF OBJECT_ID('tempdb..#FKREL') IS NOT NULL DROP TABLE #FKREL ;
IF OBJECT_ID('tempdb..#KEYCOUNT') IS NOT NULL DROP TABLE #KEYCOUNT ;

SELECT DISTINCT
    s.name + '.' + t.name + '.' + c.name AS 'FULL_COLUMN_NAME',
    s.name as SCHEMANAME,
    t.name AS TABLENAME,
    c.name AS COLUMNNAME,
    ic.ORDINAL_POSITION,
    c.is_nullable AS IS_NULLABLE,
    c.is_identity AS IS_IDENTITY,
    (CASE WHEN cu4pk.CONSTRAINT_NAME IS NOT NULL THEN 
        ROW_NUMBER() over(PARTITION BY cu4pk.CONSTRAINT_NAME 
                          ORDER BY ic.ORDINAL_POSITION ASC) 
    ELSE NULL
    END) AS PRIMARY_KEY_ORDER, 
    cu4pk.CONSTRAINT_NAME AS PRIMARY_KEY_CONSTRAINT_NAME,
    ic.DATA_TYPE,
    ic.CHARACTER_MAXIMUM_LENGTH,
    ic.NUMERIC_PRECISION,
    ic.NUMERIC_PRECISION_RADIX,
    ic.NUMERIC_SCALE,
    ic.DATETIME_PRECISION,
    (CASE t.[type]
        WHEN 'U' THEN 'TABLE'
        WHEN 'V' THEN 'VIEW'
        ELSE t.[type]
    END) as OBJECT_TYPE,
	CAST(NULL AS VARCHAR(255)) AS TEMPORAL_TYPE_DESC,
	t.object_id AS TABLE_OBJECTID
INTO
    #COL
FROM 
/*
    sys.tables as t INNER JOIN sys.schemas as s
        ON t.schema_id = s.schema_id */
    sys.objects as t INNER JOIN sys.schemas as s 
        ON t.schema_id = s.schema_id AND t.type IN ('U', 'V') 
    INNER JOIN sys.columns as c 
        ON  c.object_id = t.object_id
    INNER JOIN INFORMATION_SCHEMA.COLUMNS ic 
        ON ic.TABLE_SCHEMA = s.name AND ic.TABLE_NAME = t.name AND ic.COLUMN_NAME = c.name
    LEFT OUTER JOIN  information_schema.table_constraints tc4pk 
        ON tc4pk.TABLE_SCHEMA = s.name AND tc4pk.TABLE_NAME = t.name AND tc4pk.constraint_type = 'PRIMARY KEY '
    LEFT OUTER JOIN  information_schema.constraint_column_usage cu4pk 
        ON cu4pk.CONSTRAINT_NAME = tc4pk.CONSTRAINT_NAME AND cu4pk.COLUMN_NAME = c.name
ORDER BY 
    s.name, t.name, ic.ORDINAL_POSITION, c.name;

SELECT 
	SCHEMANAME, TABLENAME, COUNT(PRIMARY_KEY_ORDER) as KEYCOUNT
INTO
	#KEYCOUNT
FROM
	#COL
WHERE 
	PRIMARY_KEY_ORDER IS NOT NULL
GROUP BY 
	SCHEMANAME, TABLENAME;

DECLARE @SQLMajorVersion DECIMAL
SELECT @SQLMajorVersion = 
  CASE 
     WHEN CONVERT(VARCHAR(128), SERVERPROPERTY ('productversion')) like '8%' THEN 8
     WHEN CONVERT(VARCHAR(128), SERVERPROPERTY ('productversion')) like '9%' THEN 9
     WHEN CONVERT(VARCHAR(128), SERVERPROPERTY ('productversion')) like '10.0%' THEN 10
     WHEN CONVERT(VARCHAR(128), SERVERPROPERTY ('productversion')) like '10.5%' THEN 10.5
     WHEN CONVERT(VARCHAR(128), SERVERPROPERTY ('productversion')) like '11%' THEN 11
     WHEN CONVERT(VARCHAR(128), SERVERPROPERTY ('productversion')) like '12%' THEN 12
     WHEN CONVERT(VARCHAR(128), SERVERPROPERTY ('productversion')) like '13%' THEN 13     
     WHEN CONVERT(VARCHAR(128), SERVERPROPERTY ('productversion')) like '14%' THEN 14 
     ELSE NULL
  END 

IF ( @SQLMajorVersion > 12 ) BEGIN
	UPDATE #COL SET
		TEMPORAL_TYPE_DESC = (SELECT tb.temporal_type_desc FROM sys.tables tb WHERE tb.object_id = #COL.TABLE_OBJECTID) 
END ELSE BEGIN
	UPDATE #COL SET
		TEMPORAL_TYPE_DESC = 'NON_TEMPORAL_TABLE'
	WHERE OBJECT_TYPE <> 'VIEW'
END

SELECT
    SCHEMA_NAME(o.schema_id) + '.' + o.name + '.' + co.[name] AS 'FULL_COLUMN_NAME',
    i.name as IndexName, 
    o.name as TableName, 
    SCHEMA_NAME(o.schema_id) as SchemaName,
    i.is_unique,
    ic.key_ordinal as ColumnOrder,
    ic.is_included_column as IsIncluded, 
    co.[name] as ColumnName
INTO
    #IDX
from sys.indexes i 
    INNER JOIN sys.objects o 
        ON i.object_id = o.object_id
    INNER JOIN sys.index_columns ic 
        ON ic.object_id = i.object_id AND ic.index_id = i.index_id
    INNER JOIN sys.columns co 
        ON co.object_id = i.object_id AND co.column_id = ic.column_id
where 
i.[type] = 2 
--and i.is_unique = 0 
and i.is_primary_key = 0
and o.[type] IN ( 'U', 'V' )
--and ic.is_included_column = 0
order by o.[name], i.[name], ic.is_included_column, ic.key_ordinal
;

SELECT * INTO #FK FROM (
SELECT 
       EntitySchema = CONVERT(SYSNAME,SCHEMA_NAME(O1.SCHEMA_ID)), 
       EntityTable = CONVERT(SYSNAME,O1.NAME), 
       EntityColumn = CONVERT(SYSNAME,C1.NAME), 
	   EntityFullColumnName = CONVERT(SYSNAME,SCHEMA_NAME(O1.SCHEMA_ID)) + '.' + CONVERT(SYSNAME,O1.NAME) + '.' + CONVERT(SYSNAME,C1.NAME),
       RelatedSchema = CONVERT(SYSNAME,SCHEMA_NAME(O2.SCHEMA_ID)), 
       RelatedTable = CONVERT(SYSNAME,O2.NAME), 
       RelatedColumn = CONVERT(SYSNAME,C2.NAME) , 
	   RelatedFullColumnName = CONVERT(SYSNAME,SCHEMA_NAME(O2.SCHEMA_ID))  + '.' +  CONVERT(SYSNAME,O2.NAME) + '.' + CONVERT(SYSNAME,C2.NAME),
	   MatchOption = 'SIMPLE',
       FK_Name = CONVERT(SYSNAME,OBJECT_NAME(F.OBJECT_ID)), 
       PK_NAME = CONVERT(SYSNAME,I.NAME), 
	   PrimaryTableName = CONVERT(SYSNAME,O1.NAME),
       UpdateRule = CONVERT(VARCHAR(255),CASE OBJECTPROPERTY(F.OBJECT_ID,'CnstIsUpdateCascade')  
                                        WHEN 1 THEN 'CASCADE' 
                                        ELSE 'NO ACTION' 
                                      END), 
       DeleteRule = CONVERT(VARCHAR(255),CASE OBJECTPROPERTY(F.OBJECT_ID,'CnstIsDeleteCascade')  
                                        WHEN 1 THEN 'CASCADE' 
                                        ELSE 'NO ACTION' 
                                      END)
	   , FKOrdinalPosition = (SELECT ORDINAL_POSITION FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU WHERE KCU.CONSTRAINT_NAME = CONVERT(SYSNAME,OBJECT_NAME(F.OBJECT_ID)) AND KCU.COLUMN_NAME =CONVERT(SYSNAME,C2.NAME))
	   --, DEFERRABILITY = CONVERT(SMALLINT,7)   -- SQL_NOT_DEFERRABLE 
FROM   SYS.ALL_OBJECTS O1, 
       SYS.ALL_OBJECTS O2, 
       SYS.ALL_COLUMNS C1, 
       SYS.ALL_COLUMNS C2, 
       SYS.FOREIGN_KEYS F 
       INNER JOIN SYS.FOREIGN_KEY_COLUMNS K 
         ON (K.CONSTRAINT_OBJECT_ID = F.OBJECT_ID) 
       INNER JOIN SYS.INDEXES I 
         ON (F.REFERENCED_OBJECT_ID = I.OBJECT_ID 
             AND F.KEY_INDEX_ID = I.INDEX_ID) 
WHERE  O1.OBJECT_ID = F.REFERENCED_OBJECT_ID 
       AND O2.OBJECT_ID = F.PARENT_OBJECT_ID 
       AND C1.OBJECT_ID = F.REFERENCED_OBJECT_ID 
       AND C2.OBJECT_ID = F.PARENT_OBJECT_ID 
       AND C1.COLUMN_ID = K.REFERENCED_COLUMN_ID
       AND C2.COLUMN_ID = K.PARENT_COLUMN_ID
	   /*
ORDER BY 
		CONVERT(SYSNAME,SCHEMA_NAME(O1.SCHEMA_ID)) --PK_TABLEOWNER
		, CONVERT(SYSNAME,O1.NAME) -- PK TABLE NAME
		, CONVERT(SYSNAME,C1.NAME)
		, CONVERT(SYSNAME,O2.NAME)
		, CONVERT(SYSNAME,C2.NAME)*/
) t 
ORDER BY 
	EntitySchema, EntityTable, FKOrdinalPosition;

-- SELECT '' AS '#KEYCOUNT', * FROM #KEYCOUNT; --DEBUG
--SELECT '' AS '#IDX', * FROM #IDX; --DEBUG
-- SELECT '' AS '#COL', * FROM #COL; --DEBUG
SELECT 
    fk.*
	, entitykey.KEYCOUNT as EKEYCNT, entitycol.IS_NULLABLE AS ENULL, entitycol.PRIMARY_KEY_ORDER AS EPKORDER
	, relatedkey.KEYCOUNT as RKEYCNT, relatedcol.IS_NULLABLE AS RNULL, relatedcol.PRIMARY_KEY_ORDER AS RPKORDER
    , (CASE   --RelationMultiplicityEntity
        WHEN entitycol.IS_NULLABLE = 0
            THEN 'One'
        WHEN entitycol.IS_NULLABLE = 1
            THEN 'ZeroOrOne'
        ELSE
            'Unknown'
    END) as RelationMultiplicityEntity
    , (CASE --RelationMultiplicityRelated
        WHEN relatedcol.PRIMARY_KEY_ORDER IS NOT NULL AND relatedkey.KEYCOUNT = 1 AND relatedcol.IS_NULLABLE = 0
            THEN 'One'
        WHEN relatedcol.PRIMARY_KEY_ORDER IS NOT NULL AND relatedkey.KEYCOUNT = 1 AND relatedcol.IS_NULLABLE = 1
            THEN 'ZeroOrOne'
        WHEN relatedcol.IS_NULLABLE = 0
            THEN 'Many'
        ELSE
            'Many'
    END) as RelationMultiplicityRelated
    , (CASE -- InverseRelationMultiplicityEntity
        WHEN relatedcol.IS_NULLABLE = 0
            THEN 'One'
        WHEN relatedcol.IS_NULLABLE = 1
            THEN 'ZeroOrOne'
        ELSE
            'Unknown'
    END) as InverseRelationMultiplicityEntity
    , (CASE -- InverseRelationMultiplicityRelated
        WHEN entitycol.PRIMARY_KEY_ORDER IS NOT NULL AND entitykey.KEYCOUNT = 1 AND entitycol.IS_NULLABLE = 0
            THEN 'One'
        WHEN entitycol.PRIMARY_KEY_ORDER IS NOT NULL AND entitykey.KEYCOUNT = 1 AND entitycol.IS_NULLABLE = 1
            THEN 'ZeroOrOne'
        WHEN entitycol.IS_NULLABLE = 0
            THEN 'Many'
        ELSE
            'Many'
    END) as InverseRelationMultiplicityRelated
INTO 
    #FKREL
FROM 
    #FK fk LEFT OUTER JOIN #COL entitycol
        ON entitycol.FULL_COLUMN_NAME = fk.EntityFullColumnName
    LEFT OUTER JOIN #IDX entityidx
        ON entityidx.FULL_COLUMN_NAME = entitycol.FULL_COLUMN_NAME
	LEFT OUTER JOIN #KEYCOUNT entitykey
		ON entitykey.SCHEMANAME = entitycol.SCHEMANAME AND entitykey.TABLENAME = entitycol.TABLENAME
    LEFT OUTER JOIN #COL relatedcol
        ON relatedcol.FULL_COLUMN_NAME = fk.RelatedFullColumnName
    LEFT OUTER JOIN #IDX relatedidx
        ON relatedidx.FULL_COLUMN_NAME = relatedcol.FULL_COLUMN_NAME
	LEFT OUTER JOIN #KEYCOUNT relatedkey
		ON relatedkey.SCHEMANAME = relatedcol.SCHEMANAME AND relatedkey.TABLENAME = relatedcol.TABLENAME
ORDER BY
    EntitySchema, EntityTable, EntityColumn, RelatedTable, RelatedColumn
;

--SELECT '' AS '#FKREL', * FROM #FKREL; -- DEBUG

SELECT * FROM #COL 
--WHERE TABLENAME = 'DrillGroupsTemporal'
ORDER BY SCHEMANAME, TABLENAME, ORDINAL_POSITION;

SELECT * FROM (
SELECT f.EntitySchema AS SCHEMANAME, f.EntityTable AS TABLENAME,
    f.FK_Name
    , f.EntitySchema 
    , f.EntityTable
    , f.EntityColumn
	, f.RelatedSchema
    , f.RelatedTable
    , f.RelatedColumn
    , f.EntityFullColumnName
    , f.RelatedFullColumnName
    , (f.RelationMultiplicityEntity + ' to ' + f.RelationMultiplicityRelated) as Multiplicity
    , 1 as RelationGroupSort
    , f.PrimaryTableName
	, f.FKOrdinalPosition
FROM
    #FKREL f
UNION
SELECT frel.RelatedSchema AS SCHEMANAME, frel.RelatedTable AS TABLENAME,
    frel.FK_Name
    , frel.RelatedSchema
    , frel.RelatedTable
    , frel.RelatedColumn
    , frel.EntitySchema 
    , frel.EntityTable
    , frel.EntityColumn
    , frel.RelatedFullColumnName
    , frel.EntityFullColumnName
    , (frel.InverseRelationMultiplicityEntity + ' to ' + frel.InverseRelationMultiplicityRelated) as Multiplicity
    , 2 as RelationGroupSort
    , frel.PrimaryTableName
	, frel.FKOrdinalPosition
FROM
    #FKREL frel 
) e
WHERE 
--WHERE FK_Name = 'FK_Projects_PrimaryAssetTypes'
 NOT (  e.EntityTable = e.RelatedTable AND e.EntityColumn = e.RelatedColumn)
 
ORDER BY
    EntitySchema, EntityTable, RelationGroupSort, FK_Name, FKOrdinalPosition, EntityColumn, RelatedTable, RelatedColumn
;

DROP TABLE #IDX;
DROP TABLE #COL;
DROP TABLE #FK;
DROP TABLE #FKREL;
DROP TABLE #KEYCOUNT;


DECLARE @LastCreate DATETIME, @LastUpdate DATETIME
DECLARE @LastItemCreated VARCHAR(MAX), @LastItemUpdate VARCHAR(MAX)

SELECT TOP 1 @LastCreate =o.create_date, @LastItemCreated = o.name
    FROM sys.all_objects o
    LEFT OUTER JOIN sys.schemas s
    ON ( o.schema_id = s.schema_id)
    WHERE o.name <> 'tvfLastModifiedSchema'
    AND o.type_desc IN ('VIEW', 'SQL_STORED_PROCEDURE', 'USER_TABLE')
    ORDER BY create_date DESC;

SELECT TOP 1 @LastUpdate=o.modify_date, @LastItemUpdate = o.name
    FROM sys.all_objects o
    LEFT OUTER JOIN sys.schemas s
    ON ( o.schema_id = s.schema_id)
    WHERE o.name <> 'tvfLastModifiedSchema'
    AND o.type_desc IN ('VIEW', 'SQL_STORED_PROCEDURE', 'USER_TABLE')
    ORDER BY modify_date DESC;

-- Add the SELECT statement with parameter references here
SELECT @LastCreate as LastCreate, @LastUpdate as LastUpdate, @LastItemCreated as LastItemCreated, @LastItemUpdate as LastItemUpdate
";
    }
}
