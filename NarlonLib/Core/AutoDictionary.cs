using System.Collections.Generic;

namespace NarlonLib.Core
{
    public class AutoDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();

        public TValue this[TKey index]
        {
            get
            {
                if (dict.ContainsKey(index))
                    return dict[index];
                return default(TValue);
            }
            set
            {
                if (dict.ContainsKey(index))
                    dict[index] = value;
                else
                    dict.Add(index, value);
            }
        }

        public TKey[] Keys()
        {
            List<TKey> datas = new List<TKey>();
            foreach (TKey key in dict.Keys)
            {
                datas.Add(key);
            }
            return datas.ToArray();
        }

        public TValue[] Values()
        {
            List<TValue> datas = new List<TValue>();
            foreach (TKey key in dict.Keys)
            {
                datas.Add(dict[key]);
            }
            return datas.ToArray();
        }

        public void Clear()
        {
            dict.Clear();
        }

        public int Count
        {
            get { return dict.Count; }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return dict.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return dict.GetEnumerator();
        }
    }
}
