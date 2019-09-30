using System;
using System.Data;
using System.Data.Common;
using System.Text;
using MySql.Data.MySqlClient;
using RedMonkey.Extensions;

namespace RedMonkey.SchemaAnalysis.MySql
{
    public class MySqlPrimer : Primer
    {
        public MySqlPrimer(string server, string database, string user, string password)
        {
            Queries = new MySqlQueries(database);
            Builder = new MySqlConnectionStringBuilder();
            DbName = database;
            ((MySqlConnectionStringBuilder) Builder).Server = server;
            ((MySqlConnectionStringBuilder) Builder).Database = database;
            ((MySqlConnectionStringBuilder) Builder).UserID = user;
            ((MySqlConnectionStringBuilder) Builder).Password = password;
        }

        protected override string DbName { get; set; }

        protected override DatabaseType DbType => DatabaseType.MySql;

        protected override IDbConnection InstantiateConnection(DbConnectionStringBuilder _Builder)
        {
            return new MySqlConnection(_Builder.ConnectionString);
        }

        protected override void MapReaderToPrimaryConstraint(Table table, IDataReader reader)
        {
            if (reader["CONSTRAINT_NAME"].ToStringSafe(string.Empty) != "PRIMARY")
                return;
            var columnName = reader["COLUMN_NAME"].ToStringSafe(string.Empty);
            var constraintName = reader["CONSTRAINT_NAME"].ToStringSafe(string.Empty);
            if (!table.PrimaryConstraints.ContainsKey(constraintName)) table.PrimaryConstraints.Add(constraintName, new PrimaryConstraint(constraintName));
            table.PrimaryConstraints[constraintName].Columns.Add(columnName, table.Columns[columnName]);
        }

        protected override void MapReaderToDefaultConstraint(Table table, IDataReader reader)
        {
            var field = reader["Field"].ToStringSafe(string.Empty);
            var def = reader["Default"];
            if (!(def is DBNull))
                table.DefaultConstraints.Add("DEFAULT_" + field, new DefaultConstraint("DEFAULT_" + field, table.Columns[field], def));
        }

        protected override void MapReaderToForeignConstraint(Table table, IDataReader reader)
        {
            if (reader["REFERENCED_TABLE_NAME"] is DBNull || reader["REFERENCED_COLUMN_NAME"] is DBNull)
                return;
            var refTableName = reader["REFERENCED_TABLE_NAME"].ToStringSafe(string.Empty);
            var refColumName = reader["REFERENCED_COLUMN_NAME"].ToStringSafe(string.Empty);
            var colName = reader["COLUMN_NAME"].ToStringSafe(string.Empty);
            var constraintName = reader["CONSTRAINT_NAME"].ToStringSafe(string.Empty);
            table.ForeignKeyConstraints.Add(
                constraintName,
                new ForeignKeyConstraint(constraintName, table.Columns[colName], table.Database.Tables[refTableName].Columns[refColumName]));
        }

        protected override void MapReaderToUniqueConstraint(Table table, IDataReader reader)
        {
            if (!(reader["REFERENCED_TABLE_NAME"] is DBNull))
                return;
            var columnName = reader["COLUMN_NAME"].ToStringSafe(string.Empty);
            var constraintName = reader["CONSTRAINT_NAME"].ToStringSafe(string.Empty);
            if (constraintName.ToUpper() == "PRIMARY")
                return;
            if (!table.UniqueConstraints.ContainsKey(constraintName)) table.UniqueConstraints.Add(constraintName, new UniqueConstraint(constraintName));
            table.UniqueConstraints[constraintName].Columns.Add(columnName, table.Columns[columnName]);
        }

        protected override void MapReaderToIndex(Table table, IDataReader reader)
        {
            var keyName = reader["Key_Name"].ToStringSafe(string.Empty);
            if (keyName.ToUpper() == "PRIMARY" || table.UniqueConstraints.ContainsKey(keyName) || table.ForeignKeyConstraints.ContainsKey(keyName))
                return;
            var colName = reader["Column_Name"].ToStringSafe(string.Empty);
            var unique = !reader["Non_unique"].ToBool();
            if (!table.Indexes.ContainsKey(keyName))
                table.Indexes.Add(keyName, new Index(keyName, table, unique, false));
            table.Indexes[keyName].Columns.Add(colName, table.Columns[colName]);
        }

        protected override void MapReaderToFunction(Database db, IDataReader reader)
        {
            var name = reader["name"].ToStringSafe(string.Empty);
            var parameters = Encoding.Default.GetString((byte[]) reader["param_list"]);
            var returns = reader["returns"].ToStringSafe(string.Empty);
            var body = Encoding.Default.GetString((byte[]) reader["body"]);
            db.Functions.Add(name, new Function(name, returns, parameters, body, db));
        }

        protected override void MapReaderToTrigger(Database db, IDataReader reader)
        {
            var name = reader["TRIGGER_NAME"].ToStringSafe(string.Empty);
            var triggerEvent = reader["EVENT_MANIPULATION"].ToStringSafe(string.Empty);
            var tableName = reader["EVENT_OBJECT_TABLE"].ToStringSafe(string.Empty);
            var statement = reader["ACTION_STATEMENT"].ToStringSafe(string.Empty);
            var actionTiming = reader["ACTION_TIMING"].ToStringSafe(string.Empty);
            db.Triggers.Add(
                name,
                new Trigger(
                    name,
                    db.Tables[tableName],
                    db,
                    statement,
                    actionTiming.ToEnum<TriggerActionTime>(),
                    triggerEvent.ToEnum<TriggerEvent>()));
        }

        protected override void MapReaderToProcedure(Database db, IDataReader reader)
        {
            var name = reader["name"].ToStringSafe(string.Empty);
            var parameters = Encoding.Default.GetString((byte[]) reader["param_list"]);
            var returns = reader["returns"].ToStringSafe(string.Empty);
            var body = Encoding.Default.GetString((byte[]) reader["body"]);
            db.Procedures.Add(name, new Procedure(name, parameters, body, db));
        }
    }
}