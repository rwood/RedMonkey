using System;

namespace RedMonkey.SchemaAnalysis
{
    [Serializable]
    public class Column : BaseObject<Column>
    {
        public Column(string _Name, Table _Table) : base(_Name)
        {
            Table = _Table;
        }

        public DbType Type { get; set; }
        public Table Table { get; set; }

        public override bool IsEmpty()
        {
            return Type == null || Table == null || Name == null || Name.Length < 1;
        }

        public override Column Complement(Column obj)
        {
            if (Name != obj.Name)
                return this;
            return null;
        }

        public override Column Union(Column obj)
        {
            return this;
        }

        public override bool IsEqual(Column obj)
        {
            var a = Name.ToLower() == obj.Name.ToLower();
            var b = Table.Name.ToLower() == obj.Table.Name.ToLower();
            var c = Type == Type;
            return a && b && c;
        }
    }
}