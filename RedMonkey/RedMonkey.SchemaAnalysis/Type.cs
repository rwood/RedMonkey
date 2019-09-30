using System;

namespace RedMonkey.SchemaAnalysis
{
    [Serializable]
    public class DbType
    {
        public DbType(Type type, long length, long precision, Column column)
        {
            Type = type;
            Length = length;
            Precision = precision;
            Column = column;
        }

        public Type Type { get; set; }
        public long Length { get; set; }
        public long Precision { get; set; }
        public Column Column { get; set; }
    }
}