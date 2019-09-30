using System;

namespace RedMonkey.SchemaAnalysis
{
    [Serializable]
    public enum TriggerEvent
    {
        Delete,
        Update,
        Insert
    }

    [Serializable]
    public enum TriggerActionTime
    {
        Before,
        After
    }

    [Serializable]
    public class Trigger : BaseObject<Trigger>
    {
        public Trigger(string _Name, Table _Table, Database _Database, string _Definition, TriggerActionTime _ActionTime, TriggerEvent _Event) : base(_Name)
        {
            Table = _Table;
            Definition = _Definition;
            ActionTime = _ActionTime;
            Event = _Event;
            Database = _Database;
        }

        public string Definition { get; set; }
        public TriggerActionTime ActionTime { get; set; }
        public TriggerEvent Event { get; set; }
        public Table Table { get; set; }
        public Database Database { get; set; }

        public override bool IsEmpty()
        {
            return string.IsNullOrEmpty(Definition) ||
                   Table == null;
        }

        public override Trigger Complement(Trigger obj)
        {
            if (Name != obj.Name)
                return this;
            return null;
        }

        public override Trigger Union(Trigger obj)
        {
            return this;
        }

        public override bool IsEqual(Trigger obj)
        {
            return Name == obj.Name &&
                   Definition == obj.Definition &&
                   ActionTime == obj.ActionTime &&
                   Event == obj.Event &&
                   Table == obj.Table;
        }
    }
}