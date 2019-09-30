using System;
using System.Data;

namespace RedMonkey.SchemaAnalysis
{
    [Serializable]
    public class Table : BaseObject<Table>
    {
        public Table(string name, Database database) : base(name)
        {
            Database = database;
        }

        public Database Database { get; set; }
        public DataTable Schema { get; set; }

        public CompList<CompList<ColumnData>> TableData { get; set; } = new CompList<CompList<ColumnData>>();

        public Collection<Column> Columns { get; set; } = new Collection<Column>();

        public Collection<UniqueConstraint> UniqueConstraints { get; set; } = new Collection<UniqueConstraint>();

        public Collection<CheckConstraint> CheckConstraints { get; set; } = new Collection<CheckConstraint>();

        public Collection<ForeignKeyConstraint> ForeignKeyConstraints { get; set; } = new Collection<ForeignKeyConstraint>();

        public Collection<DefaultConstraint> DefaultConstraints { get; set; } = new Collection<DefaultConstraint>();

        public Collection<NullableConstraint> NullableConstraints { get; set; } = new Collection<NullableConstraint>();

        public Collection<PrimaryConstraint> PrimaryConstraints { get; set; } = new Collection<PrimaryConstraint>();

        public Collection<Increment> Increments { get; set; } = new Collection<Increment>();

        public Collection<Index> Indexes { get; set; } = new Collection<Index>();

        public Collection<Trigger> Triggers { get; set; } = new Collection<Trigger>();

        public override bool IsEmpty()
        {
            return Columns.IsEmpty();
        }

        public override bool IsEqual(Table obj)
        {
            return Name.ToLower() == obj.Name.ToLower() &&
                   Columns == obj.Columns &&
                   UniqueConstraints == obj.UniqueConstraints &&
                   CheckConstraints == obj.CheckConstraints &&
                   ForeignKeyConstraints == obj.ForeignKeyConstraints &&
                   DefaultConstraints == obj.DefaultConstraints &&
                   NullableConstraints == obj.NullableConstraints &&
                   PrimaryConstraints == obj.PrimaryConstraints &&
                   Increments == obj.Increments &&
                   Indexes == obj.Indexes &&
                   Triggers == obj.Triggers &&
                   TableData == obj.TableData;
        }

        public override Table Complement(Table obj)
        {
            var a = this;
            var b = obj;
            var c = new Table(a.Name, null)
            {
                Columns = a.Columns - b.Columns,
                TableData = a.TableData - b.TableData,
                UniqueConstraints = a.UniqueConstraints - b.UniqueConstraints,
                CheckConstraints = a.CheckConstraints - b.CheckConstraints,
                ForeignKeyConstraints = a.ForeignKeyConstraints - b.ForeignKeyConstraints,
                DefaultConstraints = a.DefaultConstraints - b.DefaultConstraints,
                NullableConstraints = a.NullableConstraints - b.NullableConstraints,
                PrimaryConstraints = a.PrimaryConstraints - b.PrimaryConstraints,
                Increments = a.Increments - b.Increments,
                Indexes = a.Indexes - b.Indexes,
                Triggers = a.Triggers - b.Triggers
            };
            ReconnectChildren(c);
            return c;
        }

        public override Table Union(Table obj)
        {
            var a = this;
            var b = obj;
            var c = new Table(a.Name, null)
            {
                Columns = a.Columns + b.Columns,
                TableData = a.TableData + b.TableData,
                UniqueConstraints = a.UniqueConstraints + b.UniqueConstraints,
                CheckConstraints = a.CheckConstraints + b.CheckConstraints,
                ForeignKeyConstraints = a.ForeignKeyConstraints + b.ForeignKeyConstraints,
                DefaultConstraints = a.DefaultConstraints + b.DefaultConstraints,
                NullableConstraints = a.NullableConstraints + b.NullableConstraints,
                PrimaryConstraints = a.PrimaryConstraints + b.PrimaryConstraints,
                Increments = a.Increments + b.Increments,
                Indexes = a.Indexes + b.Indexes,
                Triggers = a.Triggers + b.Triggers
            };
            ReconnectChildren(c);
            return c;
        }

        public static void ReconnectChildren(Table obj)
        {
            foreach (var i in obj.Columns.Values) i.Table = obj;
            foreach (var row in obj.TableData)
            foreach (var field in row)
                field.Column = obj.Columns[field.Name];
            //foreach (var i in obj.UniqueConstraints.Values) { i.Table = obj; }
            //foreach (var i in obj.CheckConstraints.Values) { i.Table = obj; }
            //foreach (var i in obj.ForeignKeyConstraints.Values) { i.Table = obj; }
            //foreach (var i in obj.DefaultConstraints.Values) { i.Table = obj; }
            //foreach (var i in obj.NullableConstraints.Values) { i.Table = obj; }
            //foreach (var i in obj.PrimaryConstraints.Values) { i.Table = obj; }
            //foreach (var i in obj.Increments.Values) { i.Table = obj; }
            foreach (var i in obj.Indexes.Values) i.Table = obj;
            foreach (var i in obj.Triggers.Values) i.Table = obj;
        }
    }
}