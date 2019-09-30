using System;
using System.Collections;
using System.Collections.Generic;

namespace RedMonkey.SchemaAnalysis
{
    [Serializable]
    public class CompList<T> : BaseObject<CompList<T>>, IList<T>
        where T : BaseObject<T>
    {
        private readonly List<T> m_Collection = new List<T>();

        public CompList()
            : base(null)
        {
        }

        public T this[int index]
        {
            get => m_Collection[index];
            set => m_Collection[index] = value;
        }

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
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

        public override bool IsEqual(CompList<T> obj)
        {
            if (m_Collection.Count != obj.Count)
                return false;
            for (var i = 0; i < Count; i++)
                if (!this[i].Equals(obj[i]))
                    return false;
            return true;
        }

        public override CompList<T> Complement(CompList<T> obj)
        {
            var A = this;
            var B = obj;
            var C = new CompList<T>();
            var maxSize = A.Count;
            if (B.Count > maxSize)
                maxSize = B.Count;
            for (var i = 0; i < maxSize; i++)
                if (A.Count > i && B.Count > i && A[i] != B[i])
                    C.Add(A[i] - B[i]);
                else if (A.Count > i)
                    C.Add(A[i]);
                else if (B.Count > i)
                    C.Add(B[i]);
            return C;
        }

        public override CompList<T> Union(CompList<T> obj)
        {
            var A = this;
            var B = obj;
            var C = new CompList<T>();
            var maxSize = A.Count;
            if (B.Count > maxSize)
                maxSize = B.Count;
            for (var i = 0; i < maxSize; i++)
                if (A.Count > i && B.Count > i && A[i] != B[i])
                    C.Add(A[i] + B[i]);
                else if (A.Count > i)
                    C.Add(A[i]);
                else if (B.Count > i)
                    C.Add(B[i]);
            return C;
        }

        #region IList<T> Members

        public int IndexOf(T item)
        {
            return m_Collection.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            m_Collection.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            m_Collection.RemoveAt(index);
        }

        #endregion

        #region ICollection<T> Members

        public void Add(T item)
        {
            m_Collection.Add(item);
        }

        public void Clear()
        {
            m_Collection.Clear();
        }

        public bool Contains(T item)
        {
            return m_Collection.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            m_Collection.CopyTo(array, arrayIndex);
        }

        public int Count => m_Collection.Count;

        public bool IsReadOnly => false;

        public bool Remove(T item)
        {
            return m_Collection.Remove(item);
        }

        #endregion
    }
}