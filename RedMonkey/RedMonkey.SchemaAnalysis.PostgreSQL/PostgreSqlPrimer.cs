using System;
using System.Data;
using System.Data.Common;
using Npgsql;
using RedMonkey.Extensions;

namespace RedMonkey.SchemaAnalysis.PostgreSQL
{
    public class PostgreSqlPrimer : Primer
    {
        public PostgreSqlPrimer(string _Server, string _Database, string _User, string _Password)
        {
            Queries = new PostgreSqlQueries(_Database);
            Builder = new NpgsqlConnectionStringBuilder();
            DbName = _Database;
            ((NpgsqlConnectionStringBuilder) Builder).Host = _Server;
            ((NpgsqlConnectionStringBuilder) Builder).Database = _Database;
            ((NpgsqlConnectionStringBuilder) Builder).Username = _User;
            ((NpgsqlConnectionStringBuilder) Builder).Password = _Password;
        }

        protected override string DbName { get; set; }

        protected override DatabaseType DbType => DatabaseType.PostgreSql;

        protected override IDbConnection InstantiateConnection(DbConnectionStringBuilder _Builder)
        {
            return new NpgsqlConnection(_Builder.ConnectionString);
        }

        protected override void MapReaderToPrimaryConstraint(Table table, IDataReader reader)
        {
            if (reader["constraint_type"].ToStringSafe(string.Empty) != "PRIMARY KEY")
                return;
            var columnName = reader["COLUMN_NAME"].ToStringSafe(string.Empty);
            var constraintName = reader["CONSTRAINT_NAME"].ToStringSafe(string.Empty);
            if (!table.PrimaryConstraints.ContainsKey(constraintName)) table.PrimaryConstraints.Add(constraintName, new PrimaryConstraint(constraintName));
            table.PrimaryConstraints[constraintName].Columns.Add(columnName, table.Columns[columnName]);
        }

        protected override void MapReaderToDefaultConstraint(Table table, IDataReader reader)
        {
            var field = reader["column_name"].ToStringSafe(string.Empty);
            var def = reader["column_default"];
            if (def.GetType() == typeof(string))
                while (((string) def).StartsWith("(") || ((string) def).EndsWith(")"))
                {
                    if (((string) def).StartsWith("("))
                        def = ((string) def).Substring(1);
                    if (((string) def).EndsWith(")"))
                        def = ((string) def).Substring(0, def.ToStringSafe().Length);
                }

            if (!(def is DBNull) && def.ToStringSafe(string.Empty).Length > 0)
                table.DefaultConstraints.Add("DEFAULT_" + field, new DefaultConstraint("DEFAULT_" + field, table.Columns[field], def));
        }

        protected override void MapReaderToForeignConstraint(Table table, IDataReader reader)
        {
            if (reader["constraint_type"].ToStringSafe(string.Empty) != "FOREIGN KEY")
                return;
            var refTableName = reader["references_table"].ToStringSafe(string.Empty);
            var refColumName = reader["references_column"].ToStringSafe(string.Empty);
            var colName = reader["column_name"].ToStringSafe(string.Empty);
            var constraintName = reader["constraint_name"].ToStringSafe(string.Empty);
            table.ForeignKeyConstraints.Add(
                constraintName,
                new ForeignKeyConstraint(constraintName, table.Columns[colName], table.Database.Tables[refTableName].Columns[refColumName]));
        }

        protected override void MapReaderToUniqueConstraint(Table table, IDataReader reader)
        {
            if (reader["constraint_type"].ToStringSafe(string.Empty) != "UNIQUE")
                return;
            var columnName = reader["column_name"].ToStringSafe(string.Empty);
            var constraintName = reader["constraint_name"].ToStringSafe(string.Empty);
            if (!table.UniqueConstraints.ContainsKey(constraintName)) table.UniqueConstraints.Add(constraintName, new UniqueConstraint(constraintName));
            table.UniqueConstraints[constraintName].Columns.Add(columnName, table.Columns[columnName]);
        }

        protected override void MapReaderToIndex(Table table, IDataReader reader)
        {
            var keyName = reader["key_name"].ToStringSafe(string.Empty);
            var colName = reader["Column_Name"].ToStringSafe(string.Empty);
            var unique = !reader["Non_unique"].ToBool();
            if (!table.Indexes.ContainsKey(keyName))
                table.Indexes.Add(keyName, new Index(keyName, table, unique, false));
            table.Indexes[keyName].Columns.Add(colName, table.Columns[colName]);
        }

        protected override void MapReaderToFunction(Database db, IDataReader reader)
        {
            throw new NotImplementedException();
        }

        protected override void MapReaderToTrigger(Database db, IDataReader reader)
        {
            throw new NotImplementedException();
        }

        protected override void MapReaderToProcedure(Database db, IDataReader reader)
        {
            //PostgreSql does not support procedures in the traditional sense.  Use Functions.
        }
    }
}