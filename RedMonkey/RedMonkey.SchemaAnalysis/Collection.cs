using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RedMonkey.SchemaAnalysis
{
    [Serializable]
    public class Collection<T> : BaseObject<Collection<T>>, IDictionary<string, T>
        where T : BaseObject<T>
    {
        private readonly Dictionary<string, T> m_Collection = new Dictionary<string, T>();

        public Collection() : base(null)
        {
        }

        public T this[string key]
        {
            get
            {
                if (m_Collection.ContainsKey(key.ToLower()))
                    return m_Collection[key.ToLower()];
                return null;
            }
            set
            {
                if (m_Collection.ContainsKey(key.ToLower()))
                    m_Collection[key.ToLower()] = value;
                else
                    m_Collection.Add(key.ToLower(), value);
            }
        }

        #region IEnumerable<KeyValuePair<string,T>> Members

        public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
        {
            return m_Collection.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_Collection.GetEnumerator();
        }

        #endregion

        public override bool IsEmpty()
        {
            return m_Collection.Count < 1;
        }

        public override bool IsEqual(Collection<T> obj)
        {
            var usedKeys = new List<string>();
            foreach (var k in Keys)
            {
                usedKeys.Add(k.ToLower());
                if (!obj.ContainsKey(k.ToLower()) || this[k.ToLower()] != obj[k.ToLower()])
                    return false;
            }

            foreach (var k in obj.Keys)
            {
                if (usedKeys.Contains(k.ToLower()))
                    continue;
                if (!ContainsKey(k.ToLower()) || obj[k.ToLower()] != this[k.ToLower()])
                    return false;
            }

            return true;
        }

        public override Collection<T> Complement(Collection<T> obj)
        {
            var A = this;
            var B = obj;
            var C = new Collection<T>();
            foreach (var k in A.Keys)
                if (A[k.ToLower()] != B[k.ToLower()])
                {
                    if (B.ContainsKey(k.ToLower()))
                        C.AddNote(typeof(T).Name + ": " + A[k.ToLower()].Name + " did not equal " + B[k.ToLower()].Name);
                    else
                        C.AddNote(typeof(T).Name + ": B does not contain " + k.ToLower());
                    C.Add(k.ToLower(), A[k.ToLower()] - B[k.ToLower()]);
                }

            return C;
        }

        public override Collection<T> Union(Collection<T> obj)
        {
            var A = this;
            var B = obj;
            var C = new Collection<T>();
            foreach (var k in A.Keys) C.Add(k.ToLower(), A[k.ToLower()] + B[k.ToLower()]);
            foreach (var k in B.Keys)
                if (!C.ContainsKey(k.ToLower()))
                    C.Add(k.ToLower(), B[k.ToLower()]);
            return C;
        }

        #region IDictionary<string,T> Members

        public void Add(string key, T value)
        {
            if (value == null)
                return;
            if (!m_Collection.ContainsKey(key.ToLower()))
                m_Collection.Add(key.ToLower(), value);
        }

        public bool ContainsKey(string key)
        {
            return m_Collection.ContainsKey(key.ToLower());
        }

        public ICollection<string> Keys => m_Collection.Keys;

        public bool Remove(string key)
        {
            return m_Collection.Remove(key.ToLower());
        }

        public bool TryGetValue(string key, out T value)
        {
            return m_Collection.TryGetValue(key.ToLower(), out value);
        }

        public ICollection<T> Values => m_Collection.Values;

        #endregion

        #region ICollection<KeyValuePair<string,T>> Members

        public void Add(KeyValuePair<string, T> item)
        {
            ((ICollection<KeyValuePair<string, T>>) m_Collection).Add(item);
        }

        public void Clear()
        {
            m_Collection.Clear();
        }

        public bool Contains(KeyValuePair<string, T> item)
        {
            return m_Collection.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, T>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<string, T>>) m_Collection).CopyTo(array, arrayIndex);
        }

        public int Count => m_Collection.Count;

        public bool IsReadOnly => ((ICollection<KeyValuePair<string, T>>) m_Collection).IsReadOnly;

        public bool Remove(KeyValuePair<string, T> item)
        {
            return ((ICollection<KeyValuePair<string, T>>) m_Collection).Remove(item);
        }

        #endregion
    }
}