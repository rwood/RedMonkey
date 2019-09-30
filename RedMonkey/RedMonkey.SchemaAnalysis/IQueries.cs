namespace RedMonkey.SchemaAnalysis
{
    public interface IQueries
    {
        string ListColumns(string _TableName);
        string ListFunctions();
        string ListKeyUsage(string _TableName);
        string ListProcedures();
        string ListTableIndexes(string _TableName);
        string ListTables();
        string ListTriggers();
        string ListViews();
        string SelectColumnInfo(string _TableName, string _ColumnName);
        string SelectEmptySet(string _TableName);
        string SelectAllRows(string _TableName);
        string SelectTableInfo(string _TableName);
        string ListParameters(string _RoutineName);
        string ListDatabases();
    }
}