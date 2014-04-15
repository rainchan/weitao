namespace WT.Components.FastDfs.FDFSClient
{
    internal sealed class Container<K, V>
    {
        public K Key;
        public V Value;

        public Container(K key, V value)
        {
            Key = key;
            Value = value;
        }
    }

    internal sealed class Container<K, V, S>
    {
        public K Key;
        public V Value;
        public S State;

        public Container(K key, V value, S state)
        {
            Key = key;
            Value = value;
            State = state;
        }
    }

    internal sealed class Container<K, V, S, E>
    {
        public K Key;
        public V Value;
        public S State;
        public E Extra;

        public Container(K key, V value, S state, E extra)
        {
            Key = key;
            Value = value;
            State = state;
            Extra = extra;
        }
    }

    internal sealed class Container<K, V, S, E, X>
    {
        public K Key;
        public V Value;
        public S State;
        public E Extra;
        public X Spare;

        public Container(K key, V value, S state, E extra, X spare)
        {
            Key = key;
            Value = value;
            State = state;
            Extra = extra;
            Spare = spare;
        }
    }
}
