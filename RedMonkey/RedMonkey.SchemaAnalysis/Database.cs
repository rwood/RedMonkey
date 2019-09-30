using System;

namespace RedMonkey.SchemaAnalysis
{
    [Serializable]
    public class Database : BaseObject<Database>
    {
        private Collection<Function> m_Functions = new Collection<Function>();
        private Collection<Procedure> m_Procedures = new Collection<Procedure>();
        private Collection<Table> m_Tables = new Collection<Table>();
        private Collection<Trigger> m_Triggers = new Collection<Trigger>();
        private Collection<View> m_Views = new Collection<View>();

        public Database(string _Name) : base(_Name)
        {
        }

        public Collection<Table> Tables
        {
            get => m_Tables;
            set => m_Tables = value;
        }

        public Collection<View> Views
        {
            get => m_Views;
            set => m_Views = value;
        }

        public Collection<Function> Functions
        {
            get => m_Functions;
            set => m_Functions = value;
        }

        public Collection<Procedure> Procedures
        {
            get => m_Procedures;
            set => m_Procedures = value;
        }

        public Collection<Trigger> Triggers
        {
            get => m_Triggers;
            set => m_Triggers = value;
        }

        public DatabaseType DbType { get; set; }
        public bool PartialRead { get; set; }

        public override bool IsEmpty()
        {
            return Tables.IsEmpty() && Views.IsEmpty() && Functions.IsEmpty() && Procedures.IsEmpty() && Triggers.IsEmpty();
        }

        public override bool IsEqual(Database obj)
        {
            return Name.ToLower() == obj.Name.ToLower() &&
                   Tables == obj.Tables &&
                   Views == obj.Views &&
                   Functions == obj.Functions &&
                   Procedures == obj.Procedures &&
                   Triggers == obj.Triggers;
        }

        public override Database Complement(Database obj)
        {
            var A = this;
            var B = obj;
            var C = new Database(Name + "_Less_" + obj.Name);
            C.Tables = A.Tables - B.Tables;

            C.Views = A.Views - B.Views;
            C.Functions = A.Functions - B.Functions;
            C.Procedures = A.Procedures - B.Procedures;
            C.Triggers = A.Triggers - B.Triggers;

            ReconnectChildren(C);
            return C;
        }

        private void ReconnectChildren(Database obj)
        {
            foreach (var k in obj.Tables.Keys)
            {
                var t = obj.Tables[k];
                t.Database = obj;
            }

            foreach (var k in Views.Keys)
            {
                var v = Views[k];
                v.Database = obj;
            }

            foreach (var k in Functions.Keys)
            {
                var f = Functions[k];
                f.Database = obj;
            }

            foreach (var k in Procedures.Keys)
            {
                var p = Procedures[k];
                p.Database = obj;
            }

            foreach (var k in Triggers.Keys)
            {
                var t = Triggers[k];
                t.Database = obj;
            }
        }

        public override Database Union(Database obj)
        {
            var A = this;
            var B = obj;
            var C = new Database(A.Name);
            C.Tables = A.Tables + B.Tables;

            C.Views = A.Views + B.Views;
            C.Functions = A.Functions + B.Functions;
            C.Procedures = A.Procedures + B.Procedures;
            C.Triggers = A.Triggers + B.Triggers;

            ReconnectChildren(C);
            return C;
        }
    }
}