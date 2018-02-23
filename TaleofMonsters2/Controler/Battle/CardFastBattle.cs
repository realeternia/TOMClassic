using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Controler.Battle.Data.MemMap;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Controler.Battle.Data.Players;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.DataType.Cards.Monsters;

namespace TaleofMonsters.Controler.Battle
{
    internal class CardFastBattle
    {
        private static CardFastBattle instance;
        public static CardFastBattle Instance
        {
            get { return instance ?? (instance = new CardFastBattle()); }
        }

        private int roundMark;

        public CardFastBattleResult StartGame(int left, int right, int leftWeapon)//初始化游戏
        {
            BattleManager.Instance.Init();
            BattleManager.Instance.EffectQueue.SetFast();
            BattleManager.Instance.FlowWordQueue.SetFast();
            BattleManager.Instance.PlayerManager.LeftPlayer = new IdlePlayer(true);
            BattleManager.Instance.PlayerManager.RightPlayer = new IdlePlayer(false);
            BattleManager.Instance.MemMap =new MemRowColumnMap("default", TileConfig.Indexer.DefaultTile);

            roundMark = 0;
            int size = BattleManager.Instance.MemMap.CardSize;
            BattleManager.Instance.StatisticData.Round = 0;
            LiveMonster newMon = new LiveMonster(1, new Monster(left), new Point(3 * size, 2 * size), true);
            //if (leftWeapon > 0)
            //{
            //    newMon.AddWeapon(leftWeapon, 1, 0);
            //}
           
            LiveMonster newMon2 = new LiveMonster(1, new Monster(right), new Point(6 * size, 2 * size), false);
            BattleManager.Instance.MonsterQueue.Add(newMon);
            BattleManager.Instance.MonsterQueue.Add(newMon2);

            CardFastBattleResult result;
            TimerProc(out result);

            return result;
        }

        private void TimerProc(out CardFastBattleResult result)
        {
            while (true)
            {
                roundMark++;
                BattleManager.Instance.MonsterQueue.NextAction(0.01f);
               
                if (BattleManager.Instance.MonsterQueue.LeftCount == 0 ||BattleManager.Instance.MonsterQueue.RightCount == 0)
                {
                    if (BattleManager.Instance.MonsterQueue.LeftCount == 0 &&BattleManager.Instance.MonsterQueue.RightCount == 0)
                    {
                        result = CardFastBattleResult.Draw;
                    }
                    else
                    {
                        result =BattleManager.Instance.MonsterQueue.LeftCount > 0 ? CardFastBattleResult.LeftWin : CardFastBattleResult.RightWin;        
                    }
                
                    break;
                }
                if (BattleManager.Instance.StatisticData.Round>=10)//10回合以上认为超时
                {
                    result = CardFastBattleResult.Draw;
                    break;
                }
                if (roundMark % 100 == 0)
                {
                    BattleManager.Instance.StatisticData.Round++;
                }
            }
        }
    }

    internal enum CardFastBattleResult
    {
        LeftWin,
        RightWin,
        Draw
    }
}
