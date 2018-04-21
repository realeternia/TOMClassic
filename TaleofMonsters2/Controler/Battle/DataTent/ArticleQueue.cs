using System.Collections.Generic;
using TaleofMonsters.Controler.Battle.Data.MemArticle;

namespace TaleofMonsters.Controler.Battle.DataTent
{
    internal class ArticleQueue
    {
        private List<Article> queue = new List<Article>();

        public List<Article> Enumerator
        {
            get { return queue; }
        }

        #region 模仿数组迭代
        public Article this[int index]
        {
            get { return queue[index]; }
            set { queue[index] = value; }
        }

        public int Count
        {
            get { return queue.Count; }
        }

        public void Add(Article effect)
        {
            queue.Add(effect);
        }

        public void RemoveDye()
        {
            if (queue.Count > 0)
                queue.RemoveAll(art => art.IsDying);
        }

        public void RemoveAt(int index)
        {
            queue.RemoveAt(index);
        }

        public void Clear()
        {
            queue.Clear();
        }
        #endregion


    }

}
