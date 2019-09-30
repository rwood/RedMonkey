using System;

namespace RedMonkey.SchemaAnalysis
{
    [Serializable]
    public class Procedure : BaseObject<Procedure>
    {
        public Procedure(string name, string parameters, string definition, Database database) : base(name)
        {
            Definition = definition;
            Params = parameters;
            Database = database;
        }

        public string Definition { get; set; }
        public string Params { get; set; }
        public Database Database { get; set; }

        public override bool IsEmpty()
        {
            return string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Definition);
        }

        public override Procedure Complement(Procedure obj)
        {
            if (Name != obj.Name)
                return this;
            return null;
        }

        public override Procedure Union(Procedure obj)
        {
            return this;
        }

        public override bool IsEqual(Procedure obj)
        {
            return Name == obj.Name &&
                   Definition == obj.Definition &&
                   Params == obj.Params;
        }
    }
}