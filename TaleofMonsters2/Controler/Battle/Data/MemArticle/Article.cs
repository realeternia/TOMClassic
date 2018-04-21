using System.Drawing;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.Datas.Others;

namespace TaleofMonsters.Controler.Battle.Data.MemArticle
{
    public class Article
    {
        private int configId;
        private int cellId;
        private int posX;
        private int posY;

        public Article(int id, int cid)
        {
            configId = id;
            var cellData = BattleManager.Instance.MemMap.GetCell(cid);
            cellId = cid;
            posX = cellData.X;
            posY = cellData.Y;
        }

        public void Draw(Graphics g, int round)
        {
            var articleImg = ArticleBook.GetArticleImage(configId);
            if (articleImg != null)
            {
                int size = BattleManager.Instance.MemMap.CardSize;
                int ewid = articleImg.Width * size / 100;
                int eheg = articleImg.Height * size / 100;

                var x = posX - (float)ewid / 2 + (float)size / 2;//+size/2是为了平移到中心位置
                var y = posY - (float)eheg / 2 + (float)size / 2;

                g.DrawImage(articleImg, x, y, ewid, eheg);
            }
        }
    }
}