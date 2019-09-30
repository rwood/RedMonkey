using System.IO;

namespace RedMonkey.SchemaAnalysis
{
    public abstract class Writer
    {
        public Writer(Database database)
        {
            Database = database;
        }

        public Database Database { get; }

        public void WriteScript(Stream stream)
        {
            foreach (var table in Database.Tables.Values) ScriptTable(table, stream);
        }

        protected void ScriptTable(Table table, Stream stream)
        {
            var script = "CREATE TABLE " + table.Name + " ( ";
            foreach (var column in table.Columns.Values) script += GetColumnDefinition(column) + ", ";
                script = script.Substring(0, script.Length - 2);
            script += ")";
        }

        protected abstract string GetColumnDefinition(Column column);
    }
}