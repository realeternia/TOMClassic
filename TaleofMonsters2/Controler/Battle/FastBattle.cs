using System;
using TaleofMonsters.Controler.Battle.Components;
using TaleofMonsters.Controler.Battle.Data.MemMap;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.Core;

namespace TaleofMonsters.Controler.Battle
{
    internal class FastBattle
    {
        static FastBattle instance;
        public static FastBattle Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FastBattle();
                }
                return instance;
            }
        }

        public bool LeftWin
        {
            get { return leftWin; }
        }

        private bool gameEnd;
        private int roundMark;
        private bool leftWin;

        public void StartGame(int left, int right, string map, int tile)//初始化游戏
        {
            gameEnd = false;
            BattleManager.Instance.Init();
            BattleManager.Instance.EffectQueue.SetFast();
            BattleManager.Instance.FlowWordQueue.SetFast();
            BattleManager.Instance.PlayerManager.Init(left, right,1);//temp
            BattleManager.Instance.MemMap = new MemRowColumnMap(map, tile);

            using (CardList cl = new CardList())
            {
                cl.MaxCards = GameConstants.CardSlotMaxCount;
                BattleManager.Instance.PlayerManager.LeftPlayer.CardsDesk = cl;
                BattleManager.Instance.PlayerManager.LeftPlayer.InitialCards();
            }
            using (CardList cl = new CardList())
            {
                cl.MaxCards = GameConstants.CardSlotMaxCount;
                BattleManager.Instance.PlayerManager.RightPlayer.CardsDesk = cl;
                BattleManager.Instance.PlayerManager.RightPlayer.InitialCards();
            }

            roundMark = 0;
            BattleManager.Instance.BattleInfo.StartTime = DateTime.Now;

            TimerProc();
        }

        private void TimerProc()
        {
            while (true)
            {
                roundMark++;
                if (roundMark % 2 == 0)
                {
                   BattleManager.Instance.MonsterQueue.NextAction();
                    if (BattleManager.Instance.MonsterQueue.LeftCount == 0 ||BattleManager.Instance.MonsterQueue.RightCount == 0)
                    {
                        EndGame();
                        break;
                    }
                }
                if (roundMark % 4 == 0)
                {
                    float pastTime = (float)200 / GameConstants.RoundTime;
                    BattleManager.Instance.PlayerManager.Update(false, pastTime, BattleManager.Instance.BattleInfo.Round);
                    if (roundMark % 250 == 0)
                    {
                        BattleManager.Instance.PlayerManager.CheckRoundCard();
                    }
                }
                if (roundMark % 10 == 0)
                {
                    AIStrategy.AIProc(BattleManager.Instance.PlayerManager.RightPlayer);
                    AIStrategy.AIProc(BattleManager.Instance.PlayerManager.LeftPlayer);
                }
                BattleManager.Instance.BattleInfo.Round = roundMark * 50 / GameConstants.RoundTime + 1;//50ms
            }
        }

        private void EndGame()
        {
            if (!gameEnd)
            {
                gameEnd = true;
                leftWin =BattleManager.Instance.MonsterQueue.LeftCount > 0;
                BattleManager.Instance.BattleInfo.EndTime = DateTime.Now;
                BattleManager.Instance.PlayerManager.Clear();
            }
        }
    }
}
