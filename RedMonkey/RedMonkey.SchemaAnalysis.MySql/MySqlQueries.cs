using System;

namespace RedMonkey.SchemaAnalysis.MySql
{
    public class MySqlQueries : IQueries
    {
        private readonly string DatabaseName = string.Empty;

        private readonly string ListColumnsQuery =
            "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = '{0}' AND TABLE_NAME = '{1}' ORDER BY ORDINAL_POSITION ASC;";

        private readonly string ListDatabasesQuery = "SHOW Databases";
        private readonly string ListFunctionsQuery = "SELECT * FROM mysql.proc WHERE db ='{0}' and type = 'FUNCTION';";
        private readonly string ListKeyUsageQuery = "SELECT * FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA = '{0}' AND TABLE_NAME = '{1}';";
        private readonly string ListProceduresQuery = "SELECT * FROM mysql.proc WHERE db ='{0}' and type = 'PROCEDURE';";

        private readonly string ListTableIndexesQuery = "SHOW INDEX FROM `{0}`;";

        private readonly string ListTablesQuery = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '{0}' AND TABLE_TYPE = 'BASE TABLE';";
        private readonly string ListTriggersQuery = "SELECT * FROM INFORMATION_SCHEMA.TRIGGERS WHERE TRIGGER_SCHEMA = '{0}'";
        private readonly string ListViewsQuery = "SELECT TABLE_NAME AS VIEW_NAME, VIEW_DEFINITION FROM INFORMATION_SCHEMA.VIEWS WHERE TABLE_SCHEMA = '{0}';";
        private readonly string SelectAllRowsQuery = "SELECT * FROM `{0}`;";
        private readonly string SelectColumnInfoQuery = "SHOW COLUMNS FROM `{0}` WHERE Field = '{1}';";
        private readonly string SelectEmptySetQuery = "SELECT * FROM `{0}` WHERE 1 = 0;";
        private readonly string SelectTableInfoQuery = "SHOW COLUMNS FROM `{0}`;";

        public MySqlQueries(string _DatabaseName)
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
            return ListDatabasesQuery;
        }
    }
}