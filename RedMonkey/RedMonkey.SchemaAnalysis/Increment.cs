using System;

namespace RedMonkey.SchemaAnalysis
{
    [Serializable]
    public class Increment : BaseObject<Increment>
    {
        public Increment(Column _Column, int _Seed, int _Step) : base(null)
        {
            Column = _Column;
            Seed = _Seed;
            Step = _Step;
        }

        public int Seed { get; set; }
        public int Step { get; set; }
        public Column Column { get; set; }

        public override bool IsEmpty()
        {
            return Column == null;
        }

        public override Increment Complement(Increment obj)
        {
            if (Name != obj.Name)
                return this;
            return null;
        }

        public override Increment Union(Increment obj)
        {
            return this;
        }

        public override bool IsEqual(Increment obj)
        {
            return Seed == obj.Seed &&
                   Step == obj.Step &&
                   Name == obj.Name &&
                   Column == obj.Column;
        }
    }
}