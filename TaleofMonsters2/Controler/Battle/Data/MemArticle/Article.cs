using System;
using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Controler.Battle.Data.MemFlow;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.Datas.Effects;
using TaleofMonsters.Datas.Effects.Facts;
using TaleofMonsters.Datas.Others;

namespace TaleofMonsters.Controler.Battle.Data.MemArticle
{
    internal class Article
    {
        private int configId;
        public int CellId { get; private set; }
        private int posX;
        private int posY;

        public bool IsDying { get; set; }

        public Article(int id, int cid)
        {
            configId = id;
            var cellData = BattleManager.Instance.MemMap.GetCell(cid);
            CellId = cid;
            posX = cellData.X;
            posY = cellData.Y;
        }

        public void Effect(LiveMonster lm)
        {
            var config = ConfigData.GetArticleConfig(configId);
            if (config.AddLp > 0)
                lm.OwnerPlayer.AddLp(config.AddLp);
            if (config.AddMp > 0)
                lm.OwnerPlayer.AddMp(config.AddMp);
            if (config.AddPp > 0)
                lm.OwnerPlayer.AddPp(config.AddPp);

            BattleManager.Instance.EffectQueue.Add(new MonsterBindEffect(EffectBook.GetEffect(config.Effect), lm, false));
            BattleManager.Instance.FlowWordQueue.Add(new FlowWord(config.Name, new Point(posX, posY), "Lime"));
        }

        public void Draw(Graphics g, int round)
        {
            var articleImg = ArticleBook.GetArticleImage(configId);
            if (articleImg != null)
            {
                int size = BattleManager.Instance.MemMap.CardSize;
                int ewid = size / 2;
                ewid = (int) (ewid*(Math.Sin((float)round/5)*0.15 + 0.75));

                var x = posX - (float)ewid / 2 + (float)size / 2;//+size/2是为了平移到中心位置
                var y = posY - (float)ewid / 2 + (float)size / 2;

                g.DrawImage(articleImg, x, y, ewid, ewid);
            }
        }
    }
}