using System;

namespace RedMonkey.SchemaAnalysis
{
    [Serializable]
    public class ColumnData : BaseObject<ColumnData>
    {
        public Column Column;
        public object Data;

        public ColumnData() : base(string.Empty)
        {
        }

        public override bool IsEmpty()
        {
            return Column == null && Data == null;
        }

        public override bool IsEqual(ColumnData obj)
        {
            return Column.IsEqual(obj.Column) && Data == obj.Data;
        }

        public override ColumnData Complement(ColumnData obj)
        {
            return null;
        }

        public override ColumnData Union(ColumnData obj)
        {
            return this;
        }
    }
}