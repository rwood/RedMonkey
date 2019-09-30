namespace RedMonkey.SchemaAnalysis.SqlServer
{
    public class SqlServerQueries : IQueries
    {
        private readonly string DatabaseName = string.Empty;

        private readonly string ListColumnsQuery =
            "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_CATALOG = '{0}' AND TABLE_NAME = '{1}' ORDER BY ORDINAL_POSITION ASC;";

        private readonly string ListDatabasesQuery = "SELECT name As [Database] FROM sys.databases order by name";
        private readonly string ListFunctionsQuery = "SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_TYPE = 'FUNCTION';";

        private readonly string ListKeyUsageQuery = @"SELECT k.table_name,
          k.column_name field_name,
          c.constraint_type,
          ccu.table_name 'references_table',
          ccu.column_name 'references_field',
          k.ordinal_position 'field_position',
          k.CONSTRAINT_NAME
     FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE k
     LEFT JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS c
       ON k.table_name = c.table_name
      AND k.table_schema = c.table_schema
      AND k.table_catalog = c.table_catalog
      AND k.constraint_catalog = c.constraint_catalog
      AND k.constraint_name = c.constraint_name
LEFT JOIN INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS rc
       ON rc.constraint_schema = c.constraint_schema
      AND rc.constraint_catalog = c.constraint_catalog
      AND rc.constraint_name = c.constraint_name
LEFT JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE ccu
       ON rc.unique_constraint_schema = ccu.constraint_schema
      AND rc.unique_constraint_catalog = ccu.constraint_catalog
      AND rc.unique_constraint_name = ccu.constraint_name
    WHERE k.constraint_catalog = DB_NAME()
      AND k.table_name = '{0}'";

        private readonly string ListParametersQuery = "SELECT * FROM INFORMATION_SCHEMA.PARAMETERS WHERE SPECIFIC_NAME = '{0}';";
        private readonly string ListProceduresQuery = "SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_TYPE = 'PROCEDURE';";

        private readonly string ListTableIndexesQuery =
            @"SELECT object_name(i.object_id) AS Table_Name, c.name AS Column_name, i.name AS Key_Name, is_unique, i.type 
FROM sys.indexes i 
JOIN sys.index_columns ic ON i.index_id = ic.index_id AND i.object_id = ic.object_id
JOIN sys.columns c ON ic.column_id = c.column_id AND ic.object_id = c.object_id
JOIN sys.objects o ON i.object_ID = o.object_id
WHERE i.is_primary_key = 0 AND is_unique_constraint = 0 AND o.type = 'U' AND i.name IS NOT NULL AND i.object_id = OBJECT_ID('{0}') ";

        private readonly string ListTablesQuery = @"SELECT TABLE_NAME
    FROM INFORMATION_SCHEMA.TABLES
   WHERE TABLE_TYPE = 'BASE TABLE'
     AND OBJECTPROPERTY(OBJECT_ID(TABLE_NAME), 'IsMsShipped') = 0
ORDER BY TABLE_SCHEMA,
         TABLE_NAME;";

        private readonly string ListTriggersQuery = @"SELECT sys1.name trigger_name,
       sys2.name table_name,
       c.text trigger_body,
       c.encrypted is_encripted,
       CASE
         WHEN OBJECTPROPERTY(sys1.id, 'ExecIsTriggerDisabled') = 1
         THEN 0 ELSE 1
       END trigger_enabled,
       CASE
         WHEN OBJECTPROPERTY(sys1.id, 'ExecIsInsertTrigger') = 1 THEN 'INSERT'
         WHEN OBJECTPROPERTY(sys1.id, 'ExecIsUpdateTrigger') = 1 THEN 'UPDATE'
         WHEN OBJECTPROPERTY(sys1.id, 'ExecIsDeleteTrigger') = 1 THEN 'DELETE'
       END trigger_event,
       CASE WHEN OBJECTPROPERTY(sys1.id, 'ExecIsInsteadOfTrigger') = 1
         THEN 'INSTEAD OF' ELSE 'AFTER'
       END trigger_type
  FROM sysobjects sys1
  JOIN sysobjects sys2 ON sys1.parent_obj = sys2.id
  JOIN syscomments c ON sys1.id = c.id
 WHERE sys1.xtype = 'TR';";

        private readonly string ListViewsQuery = "SELECT TABLE_NAME AS VIEW_NAME, VIEW_DEFINITION FROM INFORMATION_SCHEMA.VIEWS WHERE TABLE_CATALOG = '{0}';";
        private readonly string SelectAllRowsQuery = "SELECT * FROM [{0}];";
        private readonly string SelectColumnInfoQuery = "SELECT * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{0}' AND COLUMN_NAME = '{1}';";
        private readonly string SelectEmptySetQuery = "SELECT * FROM [{0}] WHERE 1 = 0;";
        private readonly string SelectTableInfoQuery = "SELECT * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{0}';";

        public SqlServerQueries(string _DatabaseName)
        {
            DatabaseName = _DatabaseName;
        }

        public string ListTables()
        {
            return string.Format(ListTablesQuery, DatabaseName);
        }

        public string ListViews()
        {
            return string.Format(ListViewsQuery, DatabaseName);
        }

        public string ListColumns(string _TableName)
        {
            return string.Format(ListColumnsQuery, new[] {DatabaseName, _TableName});
        }

        public string SelectEmptySet(string _TableName)
        {
            return string.Format(SelectEmptySetQuery, _TableName);
        }

        public string SelectAllRows(string _TableName)
        {
            return string.Format(SelectAllRowsQuery, _TableName);
        }

        public string SelectTableInfo(string _TableName)
        {
            return string.Format(SelectTableInfoQuery, _TableName);
        }

        public string SelectColumnInfo(string _TableName, string _ColumnName)
        {
            return string.Format(SelectColumnInfoQuery, new[] {_TableName, _ColumnName});
        }

        public string ListTableIndexes(string _TableName)
        {
            return string.Format(ListTableIndexesQuery, _TableName);
        }

        public string ListKeyUsage(string _TableName)
        {
            return string.Format(ListKeyUsageQuery, new[] {_TableName});
        }

        public string ListFunctions()
        {
            return ListFunctionsQuery;
        }

        public string ListProcedures()
        {
            return ListProceduresQuery;
        }

        public string ListParameters(string _RoutineName)
        {
            return string.Format(ListParametersQuery, _RoutineName);
        }

        public string ListTriggers()
        {
            return ListTriggersQuery;
        }

        public string ListDatabases()
        {
            return ListDatabasesQuery;
        }
    }
}