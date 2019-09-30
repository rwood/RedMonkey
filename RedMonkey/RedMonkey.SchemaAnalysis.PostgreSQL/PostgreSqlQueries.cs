using System;

namespace RedMonkey.SchemaAnalysis.PostgreSQL
{
    public class PostgreSqlQueries : IQueries
    {
        private readonly string DatabaseName = string.Empty;
        private readonly string GetTypeNameQuery = "SELECT typname FROM pg_type WHERE oid = {0};";
        private readonly string ListColumnsQuery = "SELECT column_name FROM information_schema.columns WHERE table_name = '{1}' ORDER BY ordinal_position;;";
        private readonly string ListDatabaseQuery = "SELECT pg_database.datname as \"Database\" FROM pg_database ORDER BY \"Database\";";

        private readonly string ListFunctionsQuery = @"SELECT proname, 
proargtypes,
proargnames,
prosrc AS Body, 
tp.typname AS Return_Type
FROM pg_proc pr
JOIN pg_type tp ON tp.oid = pr.prorettype 
WHERE 
pr.proisagg = FALSE 
AND tp.typname <> 'trigger' 
AND pr.pronamespace IN (SELECT oid FROM pg_namespace WHERE nspname NOT LIKE 'pg_%' AND nspname != 'information_schema' );";

        private readonly string ListKeyUsageQuery = @"SELECT c.conname AS constraint_name, 
CASE c.contype WHEN 'c' THEN 'CHECK' WHEN 'f' THEN 'FOREIGN KEY' WHEN 'p' THEN 'PRIMARY KEY' WHEN 'u' THEN 'UNIQUE' END AS constraint_type,
CASE WHEN c.condeferrable = 'f' THEN 0 ELSE 1 END AS is_deferrable, CASE WHEN c.condeferred = 'f' THEN 0 ELSE 1 END AS is_deferred,
t.relname AS table_name, 
a.attname AS column_name,
CASE confupdtype WHEN 'a' THEN 'NO ACTION' WHEN 'r' THEN 'RESTRICT' WHEN 'c' THEN 'CASCADE' WHEN 'n' THEN 'SET NULL' 
	WHEN 'd' THEN 'SET DEFAULT' END AS on_update, CASE confdeltype WHEN 'a' THEN 'NO ACTION' WHEN 'r' THEN 'RESTRICT' 
	WHEN 'c' THEN 'CASCADE' WHEN 'n' THEN 'SET NULL' WHEN 'd' THEN 'SET DEFAULT' END AS on_delete, CASE confmatchtype 
	WHEN 'u' THEN 'UNSPECIFIED' WHEN 'f' THEN 'FULL' WHEN 'p' THEN 'PARTIAL' END AS match_type, 
t2.relname AS references_table, 
a2.attname AS references_column
FROM pg_constraint c 
LEFT JOIN pg_class t ON c.conrelid = t.oid 
LEFT JOIN pg_class t2 ON c.confrelid = t2.oid 
LEFT JOIN pg_attribute a ON a.attnum IN (SELECT * FROM generate_series(1, array_upper(c.conkey,1))) AND a.attrelid = t.oid
LEFT JOIN pg_attribute a2 ON a2.attnum IN (SELECT * FROM generate_series(1, array_upper(c.conkey,1))) AND a2.attrelid = t2.oid
WHERE t.relname = '{0}' ";

        private readonly string ListProceduresQuery = "";

        private readonly string ListTableIndexesQuery =
            "SELECT t.relname AS Key_Name, a.attname AS Column_Name, a.attnum FROM pg_index c LEFT JOIN pg_class t ON c.indrelid = t.oid LEFT JOIN pg_attribute a ON a.attrelid = t.oid AND a.attnum = ANY(indkey) WHERE t.relname = '{0}';";

        private readonly string ListTablesQuery =
            "SELECT table_name FROM information_schema.tables WHERE table_type = 'BASE TABLE' AND table_schema NOT IN ('pg_catalog', 'information_schema');";

        private readonly string ListTriggersQuery =
            "SELECT DISTINCT * FROM information_schema.triggers WHERE trigger_schema NOT IN ('pg_catalog', 'information_schema'); ";

        private readonly string ListViewsQuery =
            "SELECT table_name AS VIEW_NAME FROM information_schema.tables WHERE table_type = 'VIEW' AND table_schema NOT IN ('pg_catalog', 'information_schema') AND table_name !~ '^pg_';";

        private readonly string SelectAllRowsQuery = "SELECT * FROM {0};";

        private readonly string SelectColumnInfoQuery =
            "SELECT ordinal_position, column_name, data_type, column_default, is_nullable, character_maximum_length, numeric_precision FROM information_schema.columns WHERE table_name = '{0}' AND column_name = '{1}' ORDER BY ordinal_position; ";

        private readonly string SelectEmptySetQuery = "SELECT * FROM {0} WHERE 1 = 0;";

        private readonly string SelectTableInfoQuery =
            "SELECT ordinal_position, column_name, data_type, column_default, is_nullable, character_maximum_length, numeric_precision FROM information_schema.columns WHERE table_name = '{0}' ORDER BY ordinal_position;";

        public PostgreSqlQueries(string _DatabaseName)
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
            return string.Format(ListKeyUsageQuery, new[] {DatabaseName, _TableName});
        }

        public string ListFunctions()
        {
            return string.Format(ListFunctionsQuery, DatabaseName);
        }

        public string ListProcedures()
        {
            return string.Format(ListProceduresQuery, DatabaseName);
        }

        public string ListTriggers()
        {
            return string.Format(ListTriggersQuery, DatabaseName);
        }

        public string ListParameters(string _RoutineName)
        {
            throw new Exception("MySql doesn not support Parameter lists until version 6.0.");
        }

        public string ListDatabases()
        {
            return ListDatabaseQuery;
        }

        public string GetTypeName(string _TypeID)
        {
            return string.Format(GetTypeNameQuery, _TypeID);
        }
    }
}