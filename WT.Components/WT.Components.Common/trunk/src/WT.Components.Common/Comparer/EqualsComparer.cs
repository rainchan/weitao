using System.Collections.Generic;

namespace WT.Components.Common.Comparer
{
    public delegate bool Compare<T>(T x, T y);
    public class EqualsComparer<T> : IEqualityComparer<T>
    {
        private Compare<T> _equalsComparer;

        public EqualsComparer(Compare<T> equalsComparer)
        {
            this._equalsComparer = equalsComparer;
        }

        public bool Equals(T x, T y)
        {
            if (null != this._equalsComparer)
                return this._equalsComparer(x, y);
            else
                return false;
        }

        public int GetHashCode(T obj)
        {
            return obj.ToString().GetHashCode();
        }
    }
}
