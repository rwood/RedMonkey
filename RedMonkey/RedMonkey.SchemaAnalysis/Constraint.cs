using System;
using RedMonkey.Extensions;

namespace RedMonkey.SchemaAnalysis
{
    [Serializable]
    public enum ConstraintTypes
    {
        Nullable,
        Default,
        Unique,
        PrimaryKey,
        Check,
        ForeignKey
    }

    public interface IConstraint
    {
        string Name { get; set; }
        ConstraintTypes Type { get; set; }
    }

    [Serializable]
    public class PrimaryConstraint : BaseObject<PrimaryConstraint>, IConstraint
    {
        public PrimaryConstraint(string _Name) : base(_Name)
        {
            Columns = new Collection<Column>();
            Type = ConstraintTypes.PrimaryKey;
        }

        public Collection<Column> Columns { get; set; }
        public ConstraintTypes Type { get; set; }

        public override bool IsEqual(PrimaryConstraint obj)
        {
            return Columns == obj.Columns && Name.ToLower() == obj.Name.ToLower() && Type == obj.Type;
        }

        public override bool IsEmpty()
        {
            return Columns.IsEmpty() || string.IsNullOrEmpty(Name);
        }

        public override PrimaryConstraint Complement(PrimaryConstraint obj)
        {
            if (Name != obj.Name)
                return this;
            return null;
        }

        public override PrimaryConstraint Union(PrimaryConstraint obj)
        {
            return this;
        }
    }

    [Serializable]
    public class NullableConstraint : BaseObject<NullableConstraint>, IConstraint
    {
        public NullableConstraint(string _Name, Column _Column) : base(_Name)
        {
            Column = _Column;
            Type = ConstraintTypes.Nullable;
        }

        public Column Column { get; set; }
        public ConstraintTypes Type { get; set; }

        public override bool IsEqual(NullableConstraint obj)
        {
            return Column.Name.ToLower() == obj.Column.Name.ToLower() && Name.ToLower() == obj.Name.ToLower() && Type == obj.Type;
        }

        public override bool IsEmpty()
        {
            return string.IsNullOrEmpty(Name) || Column == null;
        }

        public override NullableConstraint Complement(NullableConstraint obj)
        {
            if (Name != obj.Name)
                return this;
            return null;
        }

        public override NullableConstraint Union(NullableConstraint obj)
        {
            return this;
        }
    }

    [Serializable]
    public class UniqueConstraint : BaseObject<UniqueConstraint>, IConstraint
    {
        public UniqueConstraint(string _Name) : base(_Name)
        {
            Type = ConstraintTypes.Unique;
            Columns = new Collection<Column>();
        }

        public Collection<Column> Columns { get; set; }
        public ConstraintTypes Type { get; set; }

        public override bool IsEqual(UniqueConstraint obj)
        {
            return Columns == obj.Columns && Name.ToLower() == obj.Name.ToLower();
        }

        public override bool IsEmpty()
        {
            return Columns.IsEmpty() || string.IsNullOrEmpty(Name);
        }

        public override UniqueConstraint Complement(UniqueConstraint obj)
        {
            if (Name != obj.Name)
                return this;
            return null;
        }

        public override UniqueConstraint Union(UniqueConstraint obj)
        {
            return this;
        }
    }

    [Serializable]
    public class DefaultConstraint : BaseObject<DefaultConstraint>, IConstraint
    {
        public DefaultConstraint(string _Name, Column _Column, object _Value) : base(_Name)
        {
            Type = ConstraintTypes.Default;
            Column = _Column;
            Value = _Value;
        }

        public Column Column { get; set; }
        public object Value { get; set; }
        public ConstraintTypes Type { get; set; }

        public override bool IsEqual(DefaultConstraint obj)
        {
            return Column.Name == obj.Column.Name &&
                   Name.ToLower() == obj.Name.ToLower() &&
                   Value.ToStringSafe(string.Empty) == obj.Value.ToStringSafe(string.Empty);
        }

        public override bool IsEmpty()
        {
            return string.IsNullOrEmpty(Name) || Column == null || Value == null;
        }

        public override DefaultConstraint Complement(DefaultConstraint obj)
        {
            if (Name != obj.Name)
                return this;
            return null;
        }

        public override DefaultConstraint Union(DefaultConstraint obj)
        {
            return this;
        }
    }

    [Serializable]
    public class CheckConstraint : BaseObject<CheckConstraint>, IConstraint
    {
        public CheckConstraint(string _Name, Column _Column, string _Operator, string _Value, bool _Literal) : base(_Name)
        {
            Column = _Column;
            Type = ConstraintTypes.Check;
            Operator = _Operator;
            Value = _Value;
            Literal = _Literal;
        }

        public Column Column { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
        public bool Literal { get; set; }
        public ConstraintTypes Type { get; set; }

        public override bool IsEqual(CheckConstraint obj)
        {
            return Name.ToLower() == obj.Name.ToLower() &&
                   Column.Name.ToLower() == obj.Column.Name.ToLower() &&
                   Operator == obj.Operator &&
                   Value == obj.Value &&
                   Literal == obj.Literal;
        }

        public override bool IsEmpty()
        {
            return string.IsNullOrEmpty(Name) ||
                   Column == null ||
                   string.IsNullOrEmpty(Operator) ||
                   string.IsNullOrEmpty(Value);
        }

        public override CheckConstraint Complement(CheckConstraint obj)
        {
            if (Name != obj.Name)
                return this;
            return null;
        }

        public override CheckConstraint Union(CheckConstraint obj)
        {
            return this;
        }
    }

    [Serializable]
    public class ForeignKeyConstraint : BaseObject<ForeignKeyConstraint>, IConstraint
    {
        public ForeignKeyConstraint(string _Name, Column _Column, Column _ForeignColumn) : base(_Name)
        {
            Column = _Column;
            Type = ConstraintTypes.ForeignKey;
            ForeignColumn = _ForeignColumn;
        }

        public Column Column { get; set; }
        public Column ForeignColumn { get; set; }
        public ConstraintTypes Type { get; set; }

        public override bool IsEqual(ForeignKeyConstraint obj)
        {
            return Name.ToLower() == obj.Name.ToLower() &&
                   Column.Name.ToLower() == obj.Column.Name.ToLower() &&
                   ForeignColumn.Name.ToLower() == obj.ForeignColumn.Name.ToLower();
        }

        public override bool IsEmpty()
        {
            return string.IsNullOrEmpty(Name) ||
                   Column == null ||
                   ForeignColumn == null;
        }

        public override ForeignKeyConstraint Complement(ForeignKeyConstraint obj)
        {
            if (Name != obj.Name)
                return this;
            return null;
        }

        public override ForeignKeyConstraint Union(ForeignKeyConstraint obj)
        {
            return this;
        }
    }
}