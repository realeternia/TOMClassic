using System;
using System.Drawing;
using TaleofMonsters.Controler.Battle.Data;
using TaleofMonsters.Controler.Battle.Data.MemArticle;
using TaleofMonsters.Controler.Battle.Data.MemMap;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Controler.Battle.DataTent;
using TaleofMonsters.Core;
using TaleofMonsters.Datas.Others;

namespace TaleofMonsters.Controler.Battle.Tool
{
    internal class BattleManager
    {
        static BattleManager instance;
        public static BattleManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new BattleManager();
                return instance;
            }
        }

        public EventMsgQueue EventMsgQueue { get; private set; }
        public EffectQueue EffectQueue { get; private set; }
        public FlowWordQueue FlowWordQueue { get; private set; }
        public MonsterQueue MonsterQueue { get; private set; }
        public BattleStatisticData StatisticData { get; private set; }//统计数据
        public BattleRuleData RuleData { get; private set; }//规则数据
        public PlayerManager PlayerManager { get; private set; }
        public MissileQueue MissileQueue { get; private set; }
        public ArticleQueue ArticleQueue { get; private set; } //场景物件
        public MemRowColumnMap MemMap { get; set; }
        public RelicHolder RelicHolder { get; private set; }

        public bool IsNight;
        public int RoundMark;//目前一个roundmark代表0.05s
        public float Round;//当前的回合数，超过固定值就可以抽牌

        public BattleManager()
        {
            Init();
        }

        public void Init()
        {
            EventMsgQueue= new EventMsgQueue();
            EffectQueue = new EffectQueue();
            FlowWordQueue = new FlowWordQueue();
            MonsterQueue = new MonsterQueue();
            StatisticData = new BattleStatisticData();
            RuleData = new BattleRuleData();
            PlayerManager = new PlayerManager();
            MissileQueue = new MissileQueue();
            ArticleQueue = new ArticleQueue();
            RelicHolder = new RelicHolder();
            IsNight = false;
        }

        public void OnMatchStart()
        {
            StatisticData.StartTime = DateTime.Now;
            StatisticData.EndTime = DateTime.Now;

            ArticleQueue.Add(new Article(ArticleBook.GetRandomArticleId(1), MemMap.GetRandomCellMiddle()));
        }

        public void Next()
        {
            RoundMark++;

            FlowWordQueue.Next();
            EffectQueue.Next();
            MissileQueue.Next();

            if (RoundMark % 4 == 0) //200ms
            {
                float pastRound = (float)200 / GameConstants.RoundTime;

                MonsterQueue.NextAction(pastRound); //1回合

                PlayerManager.Update(false, pastRound, StatisticData.Round);

                Round += pastRound;
                if (Round >= 1)
                {
                    Round = 0;
                    StatisticData.Round++;
                    OnRound(StatisticData.Round);
                }
            }

            ArticleQueue.RemoveDye();
        }

        private void OnRound(int roundId)
        {
            PlayerManager.CheckRoundCard();
            if(roundId % 2 == 0 && ArticleQueue.Count < 2)
                ArticleQueue.Add(new Article(ArticleBook.GetRandomArticleId(2), MemMap.GetRandomCellCompete()));
        }

        public void OnEnterCell(int cellId, int monId)
        {
            foreach (var article in ArticleQueue.Enumerator)
            {
                if (article.CellId == cellId)
                {
                    article.Effect(MonsterQueue.GetMonsterByUniqueId(monId));
                    article.IsDying = true;
                }
            }
        }

        public void Draw(Graphics g, MagicRegion magicRegion, CardVisualRegion visualRegion, int mouseX, int mouseY, bool isMouseIn)
        {
            MemMap.Draw(g);//画地图
            visualRegion.Draw(g);
            if (magicRegion.Active && isMouseIn)
                magicRegion.Draw(g, RoundMark, mouseX, mouseY);
            for (int i = 0; i < ArticleQueue.Count; i++)
                ArticleQueue[i].Draw(g, RoundMark);//画物件
            for (int i = 0; i < MonsterQueue.Count; i++)
            {
                LiveMonster pickMon = MonsterQueue[i];
                Color color = Color.White;
                if (isMouseIn)
                    color = magicRegion.GetMonsterColor(pickMon, mouseX, mouseY);
                pickMon.DrawOnBattle(g, color);//画怪物
            }

            for (int i = 0; i < MissileQueue.Count; i++)
                MissileQueue[i].Draw(g);//画导弹

            for (int i = 0; i < EffectQueue.Count; i++)
                EffectQueue[i].Draw(g);//画特效
            for (int i = 0; i < FlowWordQueue.Count; i++)
                FlowWordQueue[i].Draw(g);//画飘字

        }
    }
}