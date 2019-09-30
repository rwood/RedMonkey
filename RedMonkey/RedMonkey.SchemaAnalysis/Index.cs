using System;

namespace RedMonkey.SchemaAnalysis
{
    [Serializable]
    public class Index : BaseObject<Index>
    {
        public Index(string _Name, Table _Table, bool _Unique, bool _Clustered) : base(_Name)
        {
            Columns = new Collection<Column>();
            Clustered = _Clustered;
            Table = _Table;
            Unique = _Unique;
        }

        public Table Table { get; set; }
        public Collection<Column> Columns { get; set; }
        public bool Unique { get; set; }
        public bool Clustered { get; set; }

        public override bool IsEmpty()
        {
            return Name == null ||
                   Name.Length < 1 ||
                   Table == null ||
                   Columns.IsEmpty();
        }

        public override Index Complement(Index obj)
        {
            if (Name != obj.Name)
                return this;
            return null;
        }

        public override Index Union(Index obj)
        {
            return this;
        }

        public override bool IsEqual(Index obj)
        {
            return Name.ToLower() == obj.Name.ToLower() &&
                   Table.Name.ToLower() == obj.Table.Name.ToLower() &&
                   Unique == obj.Unique &&
                   Clustered == obj.Clustered &&
                   Columns == obj.Columns;
        }
    }
}