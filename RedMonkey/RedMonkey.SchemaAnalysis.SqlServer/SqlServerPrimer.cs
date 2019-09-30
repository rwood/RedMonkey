using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using RedMonkey.Extensions;

namespace RedMonkey.SchemaAnalysis.SqlServer
{
    public class SqlServerPrimer : Primer
    {
        public SqlServerPrimer(string _Server, string _Database, string _User, string _Password)
        {
            Queries = new SqlServerQueries(_Database);
            Builder = new SqlConnectionStringBuilder();
            DbName = _Database;
            ((SqlConnectionStringBuilder) Builder).DataSource = _Server;
            ((SqlConnectionStringBuilder) Builder).InitialCatalog = _Database;
            ((SqlConnectionStringBuilder) Builder).UserID = _User;
            ((SqlConnectionStringBuilder) Builder).Password = _Password;
        }

        protected override string DbName { get; set; }

        protected override DatabaseType DbType => DatabaseType.SqlServer;

        protected override IDbConnection InstantiateConnection(DbConnectionStringBuilder _Builder)
        {
            return new SqlConnection(_Builder.ConnectionString);
        }

        protected override void MapReaderToPrimaryConstraint(Table table, IDataReader reader)
        {
            if (reader["constraint_type"].ToStringSafe(string.Empty) != "PRIMARY KEY")
                return;
            var columnName = reader["field_name"].ToStringSafe(string.Empty);
            var constraintName = reader["CONSTRAINT_NAME"].ToStringSafe(string.Empty);
            if (!table.PrimaryConstraints.ContainsKey("PRIMARY")) table.PrimaryConstraints.Add("PRIMARY", new PrimaryConstraint("PRIMARY"));
            table.PrimaryConstraints["PRIMARY"].Columns.Add(columnName, table.Columns[columnName]);
        }

        protected override void MapReaderToDefaultConstraint(Table table, IDataReader reader)
        {
            var field = reader["COLUMN_NAME"].ToStringSafe(string.Empty);
            var def = reader["COLUMN_DEFAULT"];
            if (def is string columnDefault && !string.IsNullOrEmpty(columnDefault))
                while (columnDefault.StartsWith("(") || columnDefault.EndsWith(")"))
                {
                    if (columnDefault.StartsWith("("))
                        def = columnDefault.Substring(1);
                    if (columnDefault.EndsWith(")"))
                        def = columnDefault.Substring(0, columnDefault.Length - 1);
                }

            if (!(def is DBNull))
                table.DefaultConstraints.Add("DEFAULT_" + field, new DefaultConstraint("DEFAULT_" + field, table.Columns[field], def));
        }

        protected override void MapReaderToForeignConstraint(Table table, IDataReader reader)
        {
            if (reader["REFERENCES_TABLE"] is DBNull || reader["REFERENCES_FIELD"] is DBNull)
                return;
            var refTableName = reader["REFERENCES_TABLE"].ToStringSafe(string.Empty);
            var refColumName = reader["REFERENCES_FIELD"].ToStringSafe(string.Empty);
            var colName = reader["FIELD_NAME"].ToStringSafe(string.Empty);
            var constraintName = reader["CONSTRAINT_NAME"].ToStringSafe(string.Empty);
            if (table.ForeignKeyConstraints.ContainsKey(constraintName))
            {
                var count = 1;
                while (true)
                {
                    if (!table.ForeignKeyConstraints.ContainsKey(constraintName + "_" + count))
                    {
                        table.ForeignKeyConstraints.Add(
                            constraintName + "_" + count,
                            new ForeignKeyConstraint(
                                constraintName + "_" + count,
                                table.Columns[colName],
                                table.Database.Tables[refTableName].Columns[refColumName]));
                        break;
                    }

                    count++;
                }
            }
            else
            {
                table.ForeignKeyConstraints.Add(
                    constraintName,
                    new ForeignKeyConstraint(constraintName, table.Columns[colName], table.Database.Tables[refTableName].Columns[refColumName]));
            }
        }

        protected override void MapReaderToUniqueConstraint(Table table, IDataReader reader)
        {
            if (reader["constraint_type"].ToStringSafe(string.Empty) != "UNIQUE")
                return;
            var columnName = reader["field_name"].ToStringSafe(string.Empty);
            var constraintName = reader["CONSTRAINT_NAME"].ToStringSafe(string.Empty);
            if (!table.UniqueConstraints.ContainsKey(constraintName)) table.UniqueConstraints.Add(constraintName, new UniqueConstraint(constraintName));
            table.UniqueConstraints[constraintName].Columns.Add(columnName, table.Columns[columnName]);
        }

        protected override void MapReaderToIndex(Table table, IDataReader reader)
        {
            var keyName = reader["Key_Name"].ToStringSafe(string.Empty);
            var colName = reader["Column_Name"].ToStringSafe(string.Empty);
            var unique = Convert.ToBoolean(reader["is_unique"]);
            var clustered = Convert.ToInt32(reader["type"]) == 1;
            if (!table.Indexes.ContainsKey(keyName))
                table.Indexes.Add(keyName, new Index(keyName, table, unique, false));
            table.Indexes[keyName].Columns.Add(colName, table.Columns[colName]);
        }

        protected override void MapReaderToFunction(Database db, IDataReader reader)
        {
            var name = reader["ROUTINE_NAME"].ToStringSafe(string.Empty);
            var body = reader["ROUTINE_DEFINITION"].ToStringSafe(string.Empty);
            var index = body.ToUpper().IndexOf("BEGIN");
            if (index < 0)
                index = body.ToUpper().IndexOf("AS");
            body = body.Substring(index);
            var returns = string.Empty;
            if (!(reader["DATA_TYPE"] is DBNull))
            {
                returns = reader["DATA_TYPE"].ToStringSafe(string.Empty);
                if (!(reader["CHARACTER_MAXIMUM_LENGTH"] is DBNull)) returns += "(" + reader["CHARACTER_MAXIMUM_LENGTH"].ToStringSafe(string.Empty) + ")";
            }

            var function = new Function(name, returns, "", body, db);
            using (var preader = GetReader(Queries.ListParameters(name)))
            {
                while (preader.Read())
                {
                    if (preader["PARAMETER_MODE"].ToStringSafe(string.Empty) == "OUT")
                        continue;
                    function.Params += preader["PARAMETER_NAME"].ToStringSafe(string.Empty);
                    function.Params += " " + preader["DATA_TYPE"].ToStringSafe(string.Empty);
                    if (!(preader["CHARACTER_MAXIMUM_LENGTH"] is DBNull))
                        function.Params += "(" + preader["CHARACTER_MAXIMUM_LENGTH"].ToStringSafe(string.Empty) + ")";
                    function.Params += ", ";
                }

                preader.Close();
                if (function.Params.Length > 2)
                    function.Params = function.Params.Substring(0, function.Params.Length - ", ".Length);
            }

            db.Functions.Add(name, function);
        }

        protected override void MapReaderToProcedure(Database db, IDataReader reader)
        {
            var name = reader["ROUTINE_NAME"].ToStringSafe(string.Empty);
            var body = reader["ROUTINE_DEFINITION"].ToStringSafe(string.Empty);
            var index = body.ToUpper().IndexOf("BEGIN");
            if (index < 0)
                index = body.ToUpper().IndexOf("AS");
            body = body.Substring(index);
            var returns = string.Empty;
            if (!(reader["DATA_TYPE"] is DBNull))
            {
                returns = reader["DATA_TYPE"].ToStringSafe(string.Empty);
                if (!(reader["CHARACTER_MAXIMUM_LENGTH"] is DBNull)) returns += "(" + reader["CHARACTER_MAXIMUM_LENGTH"].ToStringSafe(string.Empty) + ")";
            }

            var Procedure = new Procedure(name, "", body, db);
            using (var preader = GetReader(Queries.ListParameters(name)))
            {
                while (preader.Read())
                {
                    if (preader["PARAMETER_MODE"].ToStringSafe(string.Empty) == "OUT")
                        continue;
                    Procedure.Params += preader["PARAMETER_NAME"].ToStringSafe(string.Empty);
                    Procedure.Params += " " + preader["DATA_TYPE"].ToStringSafe(string.Empty);
                    if (!(preader["CHARACTER_MAXIMUM_LENGTH"] is DBNull))
                        Procedure.Params += "(" + preader["CHARACTER_MAXIMUM_LENGTH"].ToStringSafe(string.Empty) + ")";
                    Procedure.Params += ", ";
                }

                preader.Close();
                if (Procedure.Params.Length > 2)
                    Procedure.Params = Procedure.Params.Substring(0, Procedure.Params.Length - ", ".Length);
            }

            db.Procedures.Add(name, Procedure);
        }

        protected override void MapReaderToTrigger(Database db, IDataReader reader)
        {
            var name = reader["TRIGGER_NAME"].ToStringSafe(string.Empty);
            var triggerEvent = reader["TRIGGER_EVENT"].ToStringSafe(string.Empty);
            var tableName = reader["TABLE_NAME"].ToStringSafe(string.Empty);
            var statement = reader["TRIGGER_BODY"].ToStringSafe(string.Empty);
            var actionTiming = reader["TRIGGER_TYPE"].ToStringSafe(string.Empty);
            if (db.Triggers.ContainsKey(name))
                db.Triggers[name].Definition += statement;
            else
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
    }
}