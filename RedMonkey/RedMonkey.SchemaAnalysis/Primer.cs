using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using RedMonkey.Extensions;

namespace RedMonkey.SchemaAnalysis
{
    public abstract class Primer
    {
        protected DbConnectionStringBuilder Builder;
        protected IQueries Queries;
        private readonly Dictionary<IDbConnection, IDataReader> _pool = new Dictionary<IDbConnection, IDataReader>();
        protected abstract string DbName { get; set; }
        protected abstract DatabaseType DbType { get; }

        protected abstract IDbConnection InstantiateConnection(DbConnectionStringBuilder builder);
        protected abstract void MapReaderToPrimaryConstraint(Table table, IDataReader reader);
        protected abstract void MapReaderToDefaultConstraint(Table table, IDataReader reader);
        protected abstract void MapReaderToForeignConstraint(Table table, IDataReader reader);
        protected abstract void MapReaderToUniqueConstraint(Table table, IDataReader reader);
        protected abstract void MapReaderToIndex(Table table, IDataReader reader);
        protected abstract void MapReaderToFunction(Database db, IDataReader reader);
        protected abstract void MapReaderToTrigger(Database db, IDataReader reader);
        protected abstract void MapReaderToProcedure(Database db, IDataReader reader);

        protected IDbConnection GetActiveConnection()
        {
            foreach (var con in _pool.Keys)
                if (_pool[con] == null || _pool[con].IsClosed)
                {
                    if (con.State == ConnectionState.Closed)
                        con.Open();
                    return con;
                }

            var connection = InstantiateConnection(Builder);
            connection.Open();
            return connection;
        }

        protected IDataReader GetReader(string query)
        {
            var connection = GetActiveConnection();
            var command = connection.CreateCommand();
            command.CommandText = query;
            var reader = command.ExecuteReader();
            if (_pool.ContainsKey(connection))
                _pool[connection] = reader;
            else
                _pool.Add(connection, reader);
            return reader;
        }

        public Database ReadDatabase()
        {
            var db = new Database(DbName);
            db.DbType = DbType;
            FillTables(db);
            //FillTableData(db);
            //TablesColumnsReadComplete(db, new EventArgs());
            FillViews(db);
            FillForeignKeyConstraints(db);
            FillIndexes(db);
            FillFunctions(db);
            FillProcedures(db);
            FillTriggers(db);
            db.PartialRead = false;
            return db;
        }

        private void FillTableData(Database db)
        {
            foreach (var table in db.Tables.Values)
            {
                var query = Queries.SelectAllRows(table.Name);
                using (var reader = GetReader(query))
                {
                    while (reader.Read())
                    {
                        var row = new CompList<ColumnData>();
                        foreach (DataRow dr in table.Schema.Rows)
                        {
                            var field = new ColumnData();
                            field.Name = dr["ColumnName"].ToStringSafe(string.Empty);
                            field.Data = reader[field.Name];
                            field.Column = table.Columns[field.Name];
                            row.Add(field);
                        }

                        table.TableData.Add(row);
                    }
                }
            }
        }

        protected void FillTables(Database db)
        {
            var query = Queries.ListTables();
            using (var reader = GetReader(query))
            {
                while (reader.Read())
                {
                    var tableName = reader["TABLE_NAME"].ToString();
                    var table = new Table(tableName, db);
                    FillColumns(table);
                    FillPrimaryConstraints(table);
                    FillDefaultConstraint(table);
                    FillUniqueConstraints(table);
                    FillCheckConstraint(table);
                    db.Tables.Add(tableName, table);
                }

                reader.Close();
            }
        }

        protected void FillViews(Database db)
        {
            var query = Queries.ListViews();
            using (var reader = GetReader(query))
            {
                while (reader.Read())
                {
                    var viewName = reader["VIEW_NAME"].ToStringSafe(string.Empty);
                    var viewDef = reader["VIEW_DEFINITION"].ToStringSafe(string.Empty);
                    var view = new View(viewName, viewDef, db);
                    db.Views.Add(viewName, view);
                }

                reader.Close();
            }
        }

        protected void FillColumns(Table table)
        {
            using (var reader = GetReader(Queries.SelectEmptySet(table.Name)))
            {
                table.Schema = reader.GetSchemaTable();
                Debug.Assert(table.Schema?.Rows != null, "table.Schema?.Rows != null");
                foreach (DataRow dr in table.Schema?.Rows)
                {
                    var columnName = dr["ColumnName"].ToStringSafe(string.Empty);
                    var column = new Column(columnName, table);
                    var type = Type.GetType(dr["DataType"].ToStringSafe(string.Empty));
                    var length = dr["ColumnSize"].ToLong();
                    var precision = dr["NumericPrecision"].ToLong();
                    var tmpType = new DbType(type, length, precision, column);
                    column.Type = tmpType;
                    table.Columns.Add(columnName, column);
                    if (dr["AllowDBNull"].ToBool())
                        table.NullableConstraints.Add("NULLABLE_" + columnName, new NullableConstraint("NULLABLE_" + columnName, column));
                    if (dr["IsAutoIncrement"].ToBool()) table.Increments.Add("INCREMENT_" + columnName, new Increment(column, 1, 1));
                }

                reader.Close();
            }
        }

        protected void FillCheckConstraint(Table table)
        {
            //Check Constraints are currently not supported.
        }

        protected void FillFunctions(Database db)
        {
            using (var reader = GetReader(Queries.ListFunctions()))
            {
                while (reader.Read()) MapReaderToFunction(db, reader);
                reader.Close();
            }
        }

        protected void FillProcedures(Database db)
        {
            using (var reader = GetReader(Queries.ListProcedures()))
            {
                while (reader.Read()) MapReaderToProcedure(db, reader);
                reader.Close();
            }
        }

        protected void FillTriggers(Database db)
        {
            using (var reader = GetReader(Queries.ListTriggers()))
            {
                while (reader.Read()) MapReaderToTrigger(db, reader);
                reader.Close();
            }
        }

        protected void FillIndexes(Database db)
        {
            foreach (var key in db.Tables.Keys)
            {
                var table = db.Tables[key];
                using (var reader = GetReader(Queries.ListTableIndexes(table.Name)))
                {
                    while (reader.Read()) MapReaderToIndex(table, reader);
                    reader.Close();
                }
            }
        }

        protected void FillUniqueConstraints(Table table)
        {
            using (var reader = GetReader(Queries.ListKeyUsage(table.Name)))
            {
                while (reader.Read()) MapReaderToUniqueConstraint(table, reader);
                reader.Close();
            }
        }

        protected void FillForeignKeyConstraints(Database db)
        {
            foreach (var key in db.Tables.Keys)
            {
                var table = db.Tables[key];
                using (var reader = GetReader(Queries.ListKeyUsage(table.Name)))
                {
                    while (reader.Read()) MapReaderToForeignConstraint(table, reader);
                    reader.Close();
                }
            }
        }

        protected void FillDefaultConstraint(Table table)
        {
            using (var reader = GetReader(Queries.SelectTableInfo(table.Name)))
            {
                while (reader.Read()) MapReaderToDefaultConstraint(table, reader);
                reader.Close();
            }
        }

        protected void FillPrimaryConstraints(Table table)
        {
            using (var reader = GetReader(Queries.ListKeyUsage(table.Name)))
            {
                while (reader.Read()) MapReaderToPrimaryConstraint(table, reader);
                reader.Close();
            }
        }
    }
}