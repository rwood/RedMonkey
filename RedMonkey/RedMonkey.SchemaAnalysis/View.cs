using System;

namespace RedMonkey.SchemaAnalysis
{
    [Serializable]
    public class View : BaseObject<View>
    {
        public View(string name, string definition, Database database) : base(name)
        {
            Database = database;
            Definition = definition;
        }

        public Database Database { get; set; }
        public string Definition { get; set; }

        public override bool IsEmpty()
        {
            return string.IsNullOrEmpty(Name) ||
                   Definition == null ||
                   Definition.Length < 1;
        }

        public override View Complement(View obj)
        {
            if (Name != obj.Name)
                return this;
            return null;
        }

        public override View Union(View obj)
        {
            return this;
        }

        public override bool IsEqual(View obj)
        {
            return Name == obj.Name && Definition == obj.Definition;
        }
    }
}