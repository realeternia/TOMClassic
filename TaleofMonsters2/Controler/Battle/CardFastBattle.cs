using System.Drawing;
using TaleofMonsters.Controler.Battle.Data.MemMap;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Controler.Battle.Data.Players;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.DataType.Cards.Monsters;

namespace TaleofMonsters.Controler.Battle
{
    internal class CardFastBattle
    {
        static CardFastBattle instance;
        public static CardFastBattle Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CardFastBattle();
                }
                return instance;
            }
        }

        private int roundMark;

        public CardFastBattleResult StartGame(int left, int right, int leftWeapon)//初始化游戏
        {
            BattleManager.Instance.Init();
            BattleManager.Instance.EffectQueue.SetFast();
            BattleManager.Instance.FlowWordQueue.SetFast();
            BattleManager.Instance.PlayerManager.LeftPlayer = new IdlePlayer(true);
            BattleManager.Instance.PlayerManager.RightPlayer = new IdlePlayer(false);
            BattleManager.Instance.MemMap =new MemRowColumnMap("default", 9);

            roundMark = 0;
            int size = BattleManager.Instance.MemMap.CardSize;
            BattleManager.Instance.BattleInfo.Round = 0;
            LiveMonster newMon = new LiveMonster(World.WorldInfoManager.GetCardFakeId(), 1, new Monster(left),
                new Point(3 * size, 2 * size), true);
            //if (leftWeapon > 0)
            //{
            //    newMon.AddWeapon(leftWeapon, 1, 0);
            //}
           
            LiveMonster newMon2 = new LiveMonster(World.WorldInfoManager.GetCardFakeId(), 1, new Monster(right),
                new Point(6 * size, 2 * size), false);
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
               BattleManager.Instance.MonsterQueue.NextAction();
               
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
                if (BattleManager.Instance.BattleInfo.Round>=10)//10回合以上认为超时
                {
                    result = CardFastBattleResult.Draw;
                    break;
                }
                if (roundMark % 100 == 0)
                {
                    BattleManager.Instance.BattleInfo.Round++;
                }
            }
        }
    }

    enum CardFastBattleResult
    {
        LeftWin,
        RightWin,
        Draw
    }
}
