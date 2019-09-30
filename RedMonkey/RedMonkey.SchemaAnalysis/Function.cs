using System;

namespace RedMonkey.SchemaAnalysis
{
    [Serializable]
    public class Function : BaseObject<Function>
    {
        public Function(string _Name, string _Returns, string _Params, string _Definition, Database _Database) : base(_Name)
        {
            Definition = _Definition;
            Params = _Params;
            Returns = _Returns;
            Database = _Database;
        }

        public string Definition { get; set; }
        public string Returns { get; set; }
        public string Params { get; set; }
        public Database Database { get; set; }

        public override bool IsEmpty()
        {
            return string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Definition);
        }

        public override Function Complement(Function obj)
        {
            if (Name != obj.Name)
                return this;
            return null;
        }

        public override Function Union(Function obj)
        {
            return this;
        }

        public override bool IsEqual(Function obj)
        {
            return Name == obj.Name &&
                   Definition == obj.Definition &&
                   Params == obj.Params &&
                   Returns == obj.Returns;
        }
    }
}