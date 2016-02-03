using System.Collections.Generic;

namespace NarlonLib.Core
{
    public class SimpleSet<TKey>
    {
        private List<TKey> items = new List<TKey>();

        public bool Has(TKey item)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Equals(item))
                {
                    return true;
                }
            }
            return false;
        }

        public void Add(TKey item)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Equals(item))
                {
                    return;
                }
            }
            items.Add(item);
        }
    }
}
