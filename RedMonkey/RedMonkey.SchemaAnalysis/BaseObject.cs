using System;

namespace RedMonkey.SchemaAnalysis
{
    [Serializable]
    public abstract class BaseObject<T> :IBaseObject, IEquatable<BaseObject<T>> where T : BaseObject<T>
    {
        public bool Equals(BaseObject<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is BaseObject<T> other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (Notes != null ? Notes.GetHashCode() : 0);
            }
        }

        public BaseObject(string _Name)
        {
            Notes = "";
            Name = _Name;
        }
        
        public abstract bool IsEmpty();
         public abstract bool IsEqual(T obj);
         public abstract T Complement(T obj);
         public abstract T Union(T obj);
 
         public T Intersect(T obj)
         {
             var a = (T) this;
             var b = obj;
             return a.Union(b).Complement(a.Complement(b).Union(b.Complement(a)));
         }

        public string Name { get; set; }
        public string Notes { get; set; }

        public void AddNote(string _Note)
        {
            Notes += _Note + Environment.NewLine;
        }

        public static T operator +(BaseObject<T> left, T right)
        {
            if (Equals(left, null) && Equals(right, null))
                return null;
            if (!Equals(left, null) && Equals(right, null))
                return (T) left;
            if (Equals(left, null) && !Equals(right, null))
                return right;
            return left.Union(right);
        }

        public static T operator -(BaseObject<T> left, T right)
        {
            if (Equals(left, null) && Equals(right, null))
                return null;
            if (!Equals(left, null) && Equals(right, null))
                return (T) left;
            if (Equals(left, null) && !Equals(right, null))
                return null;
            return left.Complement(right);
        }

        public static bool operator ==(BaseObject<T> left, T right)
        {
            if (Equals(left, null) && Equals(right, null))
                return true;
            if (Equals(left, null) && !Equals(right, null))
                return false;
            if (!Equals(left, null) && Equals(right, null))
                return false;
            return left.IsEqual(right);
        }

        public static bool operator !=(BaseObject<T> left, T right)
        {
            return !(left == right);
        }
    }
}